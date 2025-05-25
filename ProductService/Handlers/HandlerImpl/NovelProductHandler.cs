using ProductService.Data;
using ProductService.Hanlers;
using ProductService.Model.Dtos.RequestDtos;
using ProductService.Model.Entity;

namespace ProductService.Handlers.HandlerImpl
{
    public class NovelProductHandler : IProductTypeHandler
    {
        public bool CanHandle(string productType) => productType == "novel";

        public async Task HandleAsync(ProductReqDto dto, Product savedEntity, AppDbContext dbContext)
        {
            var details = new BookDetails
            {
                ProductId = savedEntity.ProductId,
                Author = dto.Author,
                Publisher = dto.Publisher,
                Category = dto.Category
            };

            dbContext.BookDetails.Add(details);
            await dbContext.SaveChangesAsync();
        }
    }
}
