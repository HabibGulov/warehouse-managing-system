public interface IProductRepository
{
    IEnumerable<Product> GetProducts();
    Product? GetProductById(int id);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(int id);
    IEnumerable<Product> GetProductsByCategory(string categoryName);
    IEnumerable<Product> GetProductsByMaxQuantity(int maxQuantity);
    IEnumerable<ProductDetails> GetProductDetailsById(int productId);
    IEnumerable<Product> GetMostOrderedProducts(int minOrders);
}