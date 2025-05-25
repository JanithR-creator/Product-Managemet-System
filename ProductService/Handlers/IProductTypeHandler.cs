using ProductService.Data;
using ProductService.Model.Dtos.RequestDtos;
using ProductService.Model.Entity;

namespace ProductService.Hanlers
{
    public interface IProductTypeHandler
    {
        bool CanHandle(string productType);
        Task HandleAsync(ProductReqDto dto, Product savedEntity, AppDbContext dbContext);
    }
}
