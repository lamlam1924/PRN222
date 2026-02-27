using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        public List<Product> GetAllProducts() => ProductDAO.Instance.GetAllProducts();

        public Product? GetProductById(int productId) => ProductDAO.Instance.GetProductById(productId);

        public void AddProduct(Product product) => ProductDAO.Instance.AddProduct(product);

        public void UpdateProduct(Product product) => ProductDAO.Instance.UpdateProduct(product);

        public void DeleteProduct(int productId) => ProductDAO.Instance.DeleteProduct(productId);

        public List<Product> SearchProductsByName(string name) => ProductDAO.Instance.SearchProductsByName(name);
    }
}
