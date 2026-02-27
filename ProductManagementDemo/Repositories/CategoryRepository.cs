using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetAllCategories() 
            => CategoryDAO.Instance.GetAllCategories();

        public Category? GetCategoryById(int categoryId) 
            => CategoryDAO.Instance.GetCategoryById(categoryId);
    }
}
