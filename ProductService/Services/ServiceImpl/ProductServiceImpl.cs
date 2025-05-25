using ProductService.Data;
using ProductService.Hanlers;
using ProductService.Model.Dtos.RequestDtos;
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

        public void DeleteAllProducts()
        {
            var allBookDetails = dbContext.BookDetails.ToList();
            var allProducts = dbContext.Products.ToList();

            dbContext.BookDetails.RemoveRange(allBookDetails);
            dbContext.Products.RemoveRange(allProducts);

            dbContext.SaveChanges();
        }
    }
}