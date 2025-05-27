using Common.Events;
using Microsoft.EntityFrameworkCore;
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
        public ProductServiceImpl(AppDbContext dbContext, IEnumerable<IProductTypeHandler> handlers)
        {
            this.dbContext = dbContext;
            this.handlers = handlers;
        }

        public async Task<int> SaveProducts(string provider)
        {
            using var httpClient = new HttpClient();
            var adapterUrl = $"http://adapterservice:80/api/ProductAdapter?provider={provider}"; // service name in Docker

            var products = await httpClient.GetFromJsonAsync<List<ProductReqDto>>(adapterUrl);

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

        private async Task<bool> AdapterCartHandle(string provider, CartReqDto dto)
        {
            using var httpClient = new HttpClient();
            var adapterUrl = $"http://adapterservice:80/api/ProductAdapter?provider={provider}";

            var response = await httpClient.PostAsJsonAsync(adapterUrl, dto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReserveProductStockAsync(ProductCommonEventDto @event)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == @event.ProductId);

            if (product != null && product.Quantity >= @event.Quantity)
            {
                product.Quantity -= @event.Quantity;
                await dbContext.SaveChangesAsync();

                var cartReqDto = new CartReqDto()
                {
                    ProductId = product.ExternalDbId,
                    Quantity = @event.Quantity,
                    UserId = @event.UserId
                };

                var response = await AdapterCartHandle(@event.Provider, cartReqDto);

                return true;
            }

            return false;
        }

        public async Task RestoreProductStockAsync(ProductCommonEventDto @event)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == @event.ProductId);
            if (product != null)
            {
                product.Quantity += @event.Quantity;
                await dbContext.SaveChangesAsync();
            }
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
    }
}