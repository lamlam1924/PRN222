using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class ProductDAO
    {
        private static ProductDAO? instance = null;
        private static readonly object instanceLock = new object();

        private ProductDAO() { }

        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Product> GetAllProducts()
        {
            var listProducts = new List<Product>();
            try
            {
                using var context = new MyStoreContext();
                listProducts = context.Products.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }

        public Product? GetProductById(int productId)
        {
            Product? product = null;
            try
            {
                using var context = new MyStoreContext();
                product = context.Products.FirstOrDefault(p => p.ProductId == productId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        public void AddProduct(Product product)
        {
            try
            {
                using var context = new MyStoreContext();
                context.Products.Add(product);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                using var context = new MyStoreContext();
                context.Entry(product).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteProduct(int productId)
        {
            try
            {
                using var context = new MyStoreContext();
                var product = context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    context.Products.Remove(product);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Product> SearchProductsByName(string name)
        {
            var listProducts = new List<Product>();
            try
            {
                using var context = new MyStoreContext();
                listProducts = context.Products
                    .Where(p => p.ProductName.Contains(name))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }
    }
}
