public class ProductDetails
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string CategoryName { get; set; } = null!;
    public string SupplierName { get; set; } = null!;
    public string SupplierContactPerson { get; set; } = null!;
}