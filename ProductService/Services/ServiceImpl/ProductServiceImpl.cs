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

        public async Task<ProductResDto> GetProductsAsync(string productType, int page, int pageSize, string? filter = null)
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
                    .Select(p => new NovelResDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Author = p.BookDetails!.Author,
                        Publisher = p.BookDetails.Publisher,
                        Category = p.BookDetails.Category,
                        ImageUrl = p.ImageUrl,
                        ExternalProductID = p.ExternalDbId
                    })
                    .ToListAsync<object>();

                return new ProductResDto
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
                    .Select(p => new SchoolItemResDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        ImageUrl = p.ImageUrl,
                        ExternalProductID = p.ExternalDbId
                    })
                    .ToListAsync<object>();

                return new ProductResDto
                    {
                        Page = page,
                        PageSize = pageSize,
                        Products = items,
                        TotalItems = itemCount
                };
            }

            return new ProductResDto();
        }


        public void CreateInternalProduct(ProductReqDto dto)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                PruductType = dto.PruductType,
                Provider = dto.Provider
            };

            dbContext.Products.Add(newProduct);
            dbContext.SaveChanges();

            if (dto.PruductType == "novel")
            {
                var details = new BookDetails
                {
                    ProductId = newProduct.ProductId,
                    Author = dto.Author,
                    Publisher = dto.Publisher,
                    Category = dto.Category
                };
                dbContext.BookDetails.Add(details);
                dbContext.SaveChanges();
            }
        }
    }
}