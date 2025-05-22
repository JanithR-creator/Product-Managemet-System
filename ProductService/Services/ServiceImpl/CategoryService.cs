using ProductService.Data;
using ProductService.Model.Entity;

namespace ProductService.Services.ServiceImpl
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext appDbContext;

        public CategoryService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public void AddCategories()
        {
            List<Category> categories = new List<Category>()
            {
                new Category(){Name="Science fiction novel"},
                new Category(){Name="Mystery novel"},
                new Category(){Name="Romance novel"},
                new Category(){Name="Single Rule"},
                new Category(){Name="Square Rule"},
                new Category(){Name="Water Color"},
                new Category(){Name="Pen"},
                new Category(){Name="Calculator"}
            };

            appDbContext.Categories.AddRange(categories);
            appDbContext.SaveChanges();
        }
    }
}
