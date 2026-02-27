using BusinessObjects;
using Services;

namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Product Management System ===");
            Console.WriteLine();

            var productService = new ProductService();
            var categoryService = new CategoryService();
            var accountService = new AccountService();

            try
            {
                // ============= CATEGORY TESTS =============
                Console.WriteLine("========== CATEGORY TESTS ==========");
                Console.WriteLine();

                Console.WriteLine("--- All Categories ---");
                var categories = categoryService.GetAllCategories();
                foreach (var category in categories)
                {
                    Console.WriteLine($"ID: {category.CategoryId}, Name: {category.CategoryName}, " +
                        $"Description: {category.Description}");
                }
                Console.WriteLine();

                Console.WriteLine("--- Get Category by ID (1) ---");
                var singleCategory = categoryService.GetCategoryById(1);
                if (singleCategory != null)
                {
                    Console.WriteLine($"ID: {singleCategory.CategoryId}, Name: {singleCategory.CategoryName}");
                }
                Console.WriteLine();

                // ============= ACCOUNT TESTS =============
                Console.WriteLine("========== ACCOUNT TESTS ==========");
                Console.WriteLine();

                Console.WriteLine("--- Test Login (Admin) ---");
                try
                {
                    var account = accountService.Login("admin", "123");
                    Console.WriteLine($"Login successful!");
                    Console.WriteLine($"Member ID: {account.MemberId}");
                    Console.WriteLine($"Full Name: {account.FullName}");
                    Console.WriteLine($"Email: {account.EmailAddress}");
                    Console.WriteLine($"Role: {(accountService.IsAdmin(account) ? "Admin" : "Member")}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Login failed: {ex.Message}");
                }
                Console.WriteLine();

                Console.WriteLine("--- Test Invalid Login ---");
                try
                {
                    var invalidAccount = accountService.Login("invalid", "wrong");
                    Console.WriteLine("Login successful!");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Login failed (Expected): {ex.Message}");
                }
                Console.WriteLine();

                // ============= PRODUCT TESTS =============
                Console.WriteLine("========== PRODUCT TESTS ==========");
                Console.WriteLine();

                // Test 1: Get all products
                Console.WriteLine("--- All Products ---");
                var products = productService.GetAllProducts();
                foreach (var product in products)
                {
                    var categoryName = categoryService.GetCategoryName(product.CategoryId);
                    Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}, " +
                        $"Category: {categoryName}, Price: {product.UnitPrice:C}, Stock: {product.UnitsInStock}");
                }
                Console.WriteLine();

                // Test 2: Search product by name
                Console.WriteLine("--- Search Products (containing 'a') ---");
                var searchResults = productService.SearchProductsByName("a");
                foreach (var product in searchResults)
                {
                    Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}");
                }
                Console.WriteLine();

                // Test 3: Get product by ID
                Console.WriteLine("--- Get Product by ID (1) ---");
                var singleProduct = productService.GetProductById(1);
                if (singleProduct != null)
                {
                    Console.WriteLine($"ID: {singleProduct.ProductId}, Name: {singleProduct.ProductName}, " +
                        $"Price: {singleProduct.UnitPrice:C}");
                }
                else
                {
                    Console.WriteLine("Product not found");
                }
                Console.WriteLine();

                // Test 4: Add new product
                Console.WriteLine("--- Add New Product ---");
                var newProduct = new Product
                {
                    ProductId = 100,
                    ProductName = "Test Product",
                    CategoryId = 1,
                    UnitPrice = 99.99m,
                    UnitsInStock = 50
                };
                productService.AddProduct(newProduct);
                Console.WriteLine("Product added successfully!");
                Console.WriteLine();

                // Test 5: Update product
                Console.WriteLine("--- Update Product ---");
                var productToUpdate = productService.GetProductById(100);
                if (productToUpdate != null)
                {
                    productToUpdate.ProductName = "Updated Test Product";
                    productToUpdate.UnitPrice = 149.99m;
                    productService.UpdateProduct(productToUpdate);
                    Console.WriteLine("Product updated successfully!");
                }
                Console.WriteLine();

                // Test 6: Delete product
                Console.WriteLine("--- Delete Product ---");
                productService.DeleteProduct(100);
                Console.WriteLine("Product deleted successfully!");
                Console.WriteLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
