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
                PruductType = p.PruductType
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

        public async Task<List<NovelResDto>> GetAllNovels(int page, int pageSize)
        {
            var query = dbContext.Products
                .Include(p => p.BookDetails)
                .Where(p => p.PruductType == "novel");

            var data = await query
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
                    Category = p.BookDetails.Category
                })
                .ToListAsync();

            return data;
        }

        public async Task<List<SchoolItemResDto>> GetAllSclItems(int page, int pageSize)
        {
            var query = dbContext.Products
                .Where(p => p.PruductType == "school item");

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new SchoolItemResDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity
                })
                .ToListAsync();

            return data;
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