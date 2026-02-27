using BusinessObjects;

namespace Services
{
    public interface ICategoryService
    {
        List<Category> GetAllCategories();
        Category? GetCategoryById(int categoryId);
        string GetCategoryName(int categoryId);
    }
}
