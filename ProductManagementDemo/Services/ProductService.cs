using BusinessObjects;
using Repositories;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService()
        {
            _productRepository = new ProductRepository();
        }

        public List<Product> GetAllProducts()
        {
            return _productRepository.GetAllProducts();
        }

        public Product? GetProductById(int productId)
        {
            return _productRepository.GetProductById(productId);
        }

        public void AddProduct(Product product)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new ArgumentException("Product name cannot be empty");
            }
            if (product.UnitPrice < 0)
            {
                throw new ArgumentException("Unit price cannot be negative");
            }
            
            _productRepository.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new ArgumentException("Product name cannot be empty");
            }
            if (product.UnitPrice < 0)
            {
                throw new ArgumentException("Unit price cannot be negative");
            }

            _productRepository.UpdateProduct(product);
        }

        public void DeleteProduct(int productId)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            
            _productRepository.DeleteProduct(productId);
        }

        public List<Product> SearchProductsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GetAllProducts();
            }
            
            return _productRepository.SearchProductsByName(name);
        }
    }
}
