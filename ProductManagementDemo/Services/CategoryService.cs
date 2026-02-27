using BusinessObjects;
using Repositories;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService()
        {
            _categoryRepository = new CategoryRepository();
        }

        public List<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public Category? GetCategoryById(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0");
            }
            
            return _categoryRepository.GetCategoryById(categoryId);
        }

        public string GetCategoryName(int categoryId)
        {
            var category = GetCategoryById(categoryId);
            return category?.CategoryName ?? "Unknown Category";
        }
    }
}
