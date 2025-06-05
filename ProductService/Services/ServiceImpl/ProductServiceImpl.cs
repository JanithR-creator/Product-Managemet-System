using Common.Events;
using Microsoft.EntityFrameworkCore;
using ProductService.AdapterEndPointController;
using ProductService.Data;
using ProductService.Hanlers;
using ProductService.Model.Dtos.RequestDtos;
using ProductService.Model.Dtos.ResponseDtos;
using ProductService.Model.Entity;

namespace ProductService.Services.ServiceImpl
{
    public class ProductServiceImpl : IProductService
    {
        private readonly AppDbContext dbContext;
        private readonly IEnumerable<IProductTypeHandler> handlers;
        private readonly IAdapterEnpointHandler adapterEnpointController;
        public ProductServiceImpl(AppDbContext dbContext, IEnumerable<IProductTypeHandler> handlers, IAdapterEnpointHandler adapterEnpointController)
        {
            this.dbContext = dbContext;
            this.handlers = handlers;
            this.adapterEnpointController = adapterEnpointController;
        }
        
        public async Task<int> SaveProducts(string provider)
        {
            var products = await adapterEnpointController.GetProductsListAsync(provider);

            if (products == null || !products.Any())
                return 0;

            var baseProducts = products.Select(p => new Product
            {
                ExternalDbId = p.ProductId,
                Provider = p.Provider,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                PruductType = p.PruductType,
                ImageUrl = p.ImageUrl
            }).ToList();

            dbContext.Products.AddRange(baseProducts); //Add multiple entities efficiently
            await dbContext.SaveChangesAsync(); //freeing up the thread while waiting for DB

            foreach (var product in products)
            {
                var insertProduct = baseProducts.First(p => p.Name == product.Name);
                var handler = handlers.FirstOrDefault(h => h.CanHandle(product.PruductType));

                if (handler != null)
                {
                    await handler.HandleAsync(product, insertProduct, dbContext);
                }
            }

            return products.Count;
        }

        public async Task<bool> ReserveProductStockAsync(ProductCommonEventDto @event)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == @event.ProductId);

            if (product != null && product.Quantity >= @event.Quantity)
            {
                product.Quantity -= @event.Quantity;
                await dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> RestoreProductStockAsync(ProductCommonEventDto @event)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == @event.ProductId);
            if (product != null)
            {
                product.Quantity += @event.Quantity;
                await dbContext.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<bool> UpdateProductStockAsync(ProductCommonEventUpdateDto @event)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == @event.ProductId);
            if (product != null)
            {
                product.Quantity += @event.ChangeQuantity;
                await dbContext.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public void DeleteAllProducts()
        {
            var allBookDetails = dbContext.BookDetails.ToList();
            var allProducts = dbContext.Products.ToList();

            dbContext.BookDetails.RemoveRange(allBookDetails);
            dbContext.Products.RemoveRange(allProducts);

            dbContext.SaveChanges();
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return await dbContext.BookDetails
                .Select(b => b.Category)
                .Distinct()
                .ToListAsync();
        }

        public async Task<ProductPaginateResDto> GetProductsAsync(string productType, int page, int pageSize, string? filter = null)
        {
            IQueryable<Product> query = dbContext.Products;

            if (productType == "novel")
            {
                query = query.Include(p => p.BookDetails)
                             .Where(p => p.PruductType == "novel");

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(p =>
                        (p.BookDetails != null && p.BookDetails.Category == filter) ||
                        (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(filter.ToLower()))
                    );
                }

                var itemCount = query.Count();

                var novels = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductResDto
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
                        Novel = new NovelDetailsResDto
                        {
                            Author = p.BookDetails!.Author,
                            Publisher = p.BookDetails.Publisher,
                            Category = p.BookDetails.Category
                        }
                    })
                    .ToListAsync<object>();

                return new ProductPaginateResDto
                    {
                        Page = page,
                        PageSize = pageSize,
                        Products = novels,
                        TotalItems = itemCount
                };
            }
            else if (productType == "school-item")
            {
                query = query.Where(p => p.PruductType == "school-item");

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(p => p.Name.ToLower().Contains(filter.ToLower()));
                }

                var itemCount = query.Count();

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductResDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        ImageUrl = p.ImageUrl,
                        Provider = p.Provider,
                        ProductType = p.PruductType,
                        ExternalProductID = p.ExternalDbId
                    })
                    .ToListAsync<object>();

                return new ProductPaginateResDto
                    {
                        Page = page,
                        PageSize = pageSize,
                        Products = items,
                        TotalItems = itemCount
                };
            }

            return new ProductPaginateResDto();
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
                Provider = dto.Provider,
                ImageUrl = dto.ImageUrl,
                ExternalDbId = Guid.Empty
            };

            dbContext.Products.Add(newProduct);
            dbContext.SaveChanges();

            if (dto.PruductType == "novel")
            {
                var details = new BookDetails
                {
                    ProductId = newProduct.ProductId,
                    Author = dto.Author ?? string.Empty,
                    Publisher = dto.Publisher ?? string.Empty,
                    Category = dto.Category ?? string.Empty
                };
                dbContext.BookDetails.Add(details);
                dbContext.SaveChanges();
            }
        }

        public void UpdateProductInternalAsync(InternalProductReqDto dto, Guid productId)
        {
            var product = dbContext.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.Quantity = dto.Quantity;
                product.PruductType = dto.PruductType;
                product.Provider = dto.Provider;
                product.ImageUrl = dto.ImageUrl;

                if (dto.PruductType == "novel")
                {
                    var bookDetails = dbContext.BookDetails.FirstOrDefault(b => b.ProductId == product.ProductId);
                    if (bookDetails != null)
                    {
                        bookDetails.Author = dto.Author ?? string.Empty;
                        bookDetails.Publisher = dto.Publisher ?? string.Empty;
                        bookDetails.Category = dto.Category ?? string.Empty;
                    }
                    else
                    {
                        bookDetails = new BookDetails
                        {
                            ProductId = product.ProductId,
                            Author = dto.Author ?? string.Empty,
                            Publisher = dto.Publisher ?? string.Empty,
                            Category = dto.Category ?? string.Empty
                        };
                        dbContext.BookDetails.Add(bookDetails);
                    }
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
            IQueryable<Product> query = dbContext.Products;

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
                    Novel = p.BookDetails != null ? new NovelDetailsResDto
                    {
                        Author = p.BookDetails.Author,
                        Publisher = p.BookDetails.Publisher,
                        Category = p.BookDetails.Category
                    } : null
                }).ToListAsync();

            return products;
        }
    }
}