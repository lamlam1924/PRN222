using BusinessObjects;

namespace Services
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product? GetProductById(int productId);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productId);
        List<Product> SearchProductsByName(string name);
    }
}
