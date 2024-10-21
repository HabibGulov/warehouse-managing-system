public interface ISupplierRepository
{
    IEnumerable<Supplier> GetSuppliers(); 
    Supplier? GetSupplierById(int id);    
    bool CreateSupplier(Supplier supplier); 
    bool UpdateSupplier(Supplier supplier); 
    bool DeleteSupplier(int id);  
    IEnumerable<Supplier> GetSuppliersWithMinProductQuantity(int minQuantity);
}
