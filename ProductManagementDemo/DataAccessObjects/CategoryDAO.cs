using BusinessObjects;

namespace DataAccessObjects
{
    public class CategoryDAO
    {
        private static CategoryDAO? instance = null;
        private static readonly object instanceLock = new object();

        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Category> GetAllCategories()
        {
            var listCategories = new List<Category>();
            try
            {
                using var context = new MyStoreContext();
                listCategories = context.Categories.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listCategories;
        }

        public Category? GetCategoryById(int categoryId)
        {
            Category? category = null;
            try
            {
                using var context = new MyStoreContext();
                category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return category;
        }
    }
}
