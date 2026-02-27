using BusinessObjects;

namespace Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAllCategories();
        Category? GetCategoryById(int categoryId);
    }
}
