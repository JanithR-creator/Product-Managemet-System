using Microsoft.AspNetCore.Mvc;
using ProductService.Model.Dtos.RequestDtos;
using ProductService.Model.Dtos.ResponseDtos;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<int> SaveProducts(string provider);
        void DeleteAllProducts();

        Task<List<NovelResDto>> GetAllNovels(int page, int pageSize);

        Task<List<SchoolItemResDto>> GetAllSclItems(int page, int pageSize);
    }
}
