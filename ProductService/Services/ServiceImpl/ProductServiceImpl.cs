using Microsoft.EntityFrameworkCore;
using ProductService.AdapterEndPointController;
using ProductService.Data;
using ProductService.Model.Dtos.RequestDtos;
using ProductService.Model.Dtos.ResponseDtos;
using ProductService.Model.Entity;
using System.Reflection;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ProductService.Services.ServiceImpl
{
    public class ProductServiceImpl : IProductService
    {
        private readonly AppDbContext dbContext;
        private readonly IAdapterEnpointHandler adapterEnpointController;
        public ProductServiceImpl(AppDbContext dbContext, IAdapterEnpointHandler adapterEnpointController)
        {
            this.dbContext = dbContext;
            this.adapterEnpointController = adapterEnpointController;
        }

        private static readonly HashSet<string> BaseProductFields = new()
        {
            "ProductId", "Name", "Description", "Quantity", "Price", "Provider", "PruductType", "ImageUrl"
        };

        public async Task<int> SaveProducts()
        {
            var products = await adapterEnpointController.GetProductsListAsync();

            if (products == null || !products.Any())
                return 0;

            var externalIds = products.Select(p => p.ProductId).ToList();
            var existingProducts = await dbContext.Products
                .Where(p => externalIds.Contains(p.ExternalDbId))
                .ToListAsync();

            var existingProductsDict = existingProducts.ToDictionary(p => p.ExternalDbId, p => p);

            var newProducts = new List<Product>();
            var updatedProducts = new List<Product>();

            foreach (var dto in products)
            {
                if (existingProductsDict.TryGetValue(dto.ProductId, out var existingProduct))
                {
                    existingProduct.Quantity = dto.Quantity;
                    updatedProducts.Add(existingProduct);
                }
                else
                {
                    var newProduct = new Product
                    {
                        ExternalDbId = dto.ProductId,
                        Provider = dto.Provider,
                        Name = dto.Name,
                        Description = dto.Description,
                        Price = dto.Price,
                        Quantity = dto.Quantity,
                        PruductType = dto.PruductType,
                        ImageUrl = dto.ImageUrl
                    };
                    newProducts.Add(newProduct);
                }
            }

            if (newProducts.Any())
            {
                dbContext.Products.AddRange(newProducts);
            }

            if (updatedProducts.Any())
            {
                dbContext.Products.UpdateRange(updatedProducts);
            }

            await dbContext.SaveChangesAsync();

            var productDetails = new List<ProductDetails>();
            foreach (var dto in products)
            {
                Product baseProduct;
                if (existingProductsDict.TryGetValue(dto.ProductId, out var existingProduct))
                {
                    baseProduct = existingProduct;
                }
                else
                {
                    baseProduct = newProducts.First(p => p.ExternalDbId == dto.ProductId);
                }

                var metadata = new Dictionary<string, object>();
                var props = dto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in props)
                {
                    var propName = prop.Name;
                    if (BaseProductFields.Contains(propName))
                        continue;

                    var value = prop.GetValue(dto);
                    if (value != null)
                    {
                        metadata[propName] = value;
                    }
                }

                if (!existingProductsDict.ContainsKey(dto.ProductId))
                {
                    productDetails.Add(new ProductDetails
                    {
                        ProductId = baseProduct.ProductId,
                        MetadataJson = JsonSerializer.Serialize(metadata)
                    });
                }
            }

            if (productDetails.Any())
            {
                dbContext.ProductDetails.AddRange(productDetails);
                await dbContext.SaveChangesAsync();
            }

            return products.Count;
        }
        public async Task<bool> ReserveProductStockAsync(Guid productId, int quantity)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product != null && product.Quantity >= quantity)
            {
                product.Quantity -= quantity;
                await dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public void DeleteAllProducts()
        {
            var allBookDetails = dbContext.ProductDetails.ToList();
            var allProducts = dbContext.Products.ToList();

            dbContext.ProductDetails.RemoveRange(allBookDetails);
            dbContext.Products.RemoveRange(allProducts);

            dbContext.SaveChanges();
        }
        public async Task<List<string>> GetAllCategoriesAsync()
        {
            var productDetailsList = await dbContext.ProductDetails
                .Select(pd => pd.MetadataJson)
                .ToListAsync();

            var categories = productDetailsList
                .Select(json =>
                {
                    try
                    {
                        var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                        return metadata != null && metadata.ContainsKey("Category")
                            ? metadata["Category"]?.ToString()
                            : null;
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();

            return categories!;
        }
        public async Task<ProductPaginateResDto> GetProductsAsync(int page, int pageSize, string? filter = null)
        {
            IQueryable<Product> query = dbContext.Products
                .Include(p => p.ProductDetails)
                .Where(p => p.ProductDetails != null);

            if (!string.IsNullOrEmpty(filter))
            {
                var loweredFilter = filter.ToLower();
                query = query.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(loweredFilter)) ||
                    (p.ProductDetails != null && !string.IsNullOrEmpty(p.ProductDetails.MetadataJson) && p.ProductDetails.MetadataJson.ToLower().Contains(loweredFilter)));
            }

            int totalItems = await query.CountAsync();

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var products = result.Select(p => new ProductResDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                ImageUrl = p.ImageUrl,
                Provider = p.Provider,
                ProductType = p.PruductType,
                ExternalProductID = p.ExternalDbId,
                Metadata = (p.ProductDetails != null && !string.IsNullOrEmpty(p.ProductDetails.MetadataJson))
                    ? JsonSerializer.Deserialize<Dictionary<string, object>>(p.ProductDetails.MetadataJson) ?? new Dictionary<string, object>()
                    : new Dictionary<string, object>()
            }).Cast<object>().ToList();

            return new ProductPaginateResDto
            {
                Page = page,
                PageSize = pageSize,
                Products = products,
                TotalItems = totalItems
            };
        }
        public void CreateInternalProduct(InternalProductReqDto dto)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                PruductType = dto.PruductType,
                Provider = "internal",
                ImageUrl = dto.ImageUrl,
                ExternalDbId = Guid.Empty,
                ProductDetails = new ProductDetails
                {
                    MetadataJson = JsonSerializer.Serialize(dto.Metadata ?? new Dictionary<string, object>())
                }
            };

            dbContext.Products.Add(newProduct);
            dbContext.SaveChanges();
        }
        public void UpdateProductInternalAsync(InternalProductReqDto dto, Guid productId)
        {
            var product = dbContext.Products
                .Include(p => p.ProductDetails)
                .FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.Quantity = dto.Quantity;
                product.PruductType = dto.PruductType;
                product.Provider = "internal";
                product.ImageUrl = dto.ImageUrl;

                if (product.ProductDetails != null)
                {
                    product.ProductDetails.MetadataJson = JsonSerializer.Serialize(dto.Metadata ?? new Dictionary<string, object>());
                }
                else
                {
                    product.ProductDetails = new ProductDetails
                    {
                        ProductId = product.ProductId,
                        MetadataJson = JsonSerializer.Serialize(dto.Metadata ?? new Dictionary<string, object>())
                    };
                }

                dbContext.SaveChanges();
            }
        }
        public void DeleteInternalProductAsync(Guid productId)
        {
            var product = dbContext.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                dbContext.Products.Remove(product);
                dbContext.SaveChanges();
            }
        }
        public async Task<List<string>> GetAllProvidersAsync()
        {
            return await dbContext.Products
                .Select(p => p.Provider)
                .Distinct()
                .ToListAsync();
        }
        public async Task<List<ProductResDto>> GetAllProductsAsync(string? provider = null, string? filter = null)
        {
            IQueryable<Product> query = dbContext.Products
                .Include(p => p.ProductDetails)
                .Where(p => p.ProductDetails != null);

            if (!string.IsNullOrEmpty(provider))
            {
                query = query.Where(p => p.Provider == provider);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                var loweredFilter = filter.ToLower();
                query = query.Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(loweredFilter));
            }

            var products = await query
                .Select(p => new ProductResDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    ImageUrl = p.ImageUrl,
                    ExternalProductID = p.ExternalDbId,
                    Provider = p.Provider,
                    ProductType = p.PruductType,
                    Metadata = (p.ProductDetails != null && !string.IsNullOrEmpty(p.ProductDetails.MetadataJson))
                    ? JsonSerializer.Deserialize<Dictionary<string, object>>(p.ProductDetails.MetadataJson, new JsonSerializerOptions()) ?? new Dictionary<string, object>()
                    : new Dictionary<string, object>()
                }).ToListAsync();

            return products;
        }
    }
}