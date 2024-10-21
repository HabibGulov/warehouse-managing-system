using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly string _pathData;

    public SupplierRepository(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", "true");
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Suppliers));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public IEnumerable<Supplier> GetSuppliers()
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            return document.Root!.Element(XmlElements.Suppliers)!.Elements(XmlElements.Supplier)
                .Select(x => new Supplier
                {
                    Id = (int)x.Element(XmlElements.SupplierId)!,
                    Name = (string)x.Element(XmlElements.SupplierName)!,
                    ContactPerson = (string)x.Element(XmlElements.SupplierContactPerson)!,
                    Email = (string)x.Element(XmlElements.SupplierEmail)!,
                    Phone = (string)x.Element(XmlElements.SupplierPhone)!
                }).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Supplier? GetSupplierById(int id)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            var supplier = document.Root!.Element(XmlElements.Suppliers)!.Elements(XmlElements.Supplier)
                .Where(x => (int)x.Element(XmlElements.SupplierId)! == id)
                .Select(x => new Supplier
                {
                    Id = (int)x.Element(XmlElements.SupplierId)!,
                    Name = (string)x.Element(XmlElements.SupplierName)!,
                    ContactPerson = (string)x.Element(XmlElements.SupplierContactPerson)!,
                    Email = (string)x.Element(XmlElements.SupplierEmail)!,
                    Phone = (string)x.Element(XmlElements.SupplierPhone)!
                }).FirstOrDefault();
            return supplier;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public bool CreateSupplier(Supplier supplier)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            int maxId = document.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!.Elements(XmlElements.Supplier)
                .Select(x => (int)x.Element(XmlElements.SupplierId)!).DefaultIfEmpty(0).Max();

            bool isNameExists = document.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!
                .Elements(XmlElements.Supplier).Any(x => (string)x.Element(XmlElements.SupplierName)! == supplier.Name);
            if (isNameExists) return false;

            XElement xmlXElement = new XElement(XmlElements.Supplier,
                new XElement(XmlElements.SupplierId, maxId + 1),
                new XElement(XmlElements.SupplierName, supplier.Name),
                new XElement(XmlElements.SupplierContactPerson, supplier.ContactPerson),
                new XElement(XmlElements.SupplierEmail, supplier.Email),
                new XElement(XmlElements.SupplierPhone, supplier.Phone));

            document.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!.Add(xmlXElement);
            document.Save(_pathData);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public bool UpdateSupplier(Supplier supplier)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            var supplierToUpdate = document.Root!.Element(XmlElements.Suppliers)!.Elements(XmlElements.Supplier)
                .FirstOrDefault(x => (int)x.Element(XmlElements.SupplierId)! == supplier.Id);
            if (supplierToUpdate != null)
            {
                supplierToUpdate.SetElementValue(XmlElements.SupplierName, supplier.Name);
                supplierToUpdate.SetElementValue(XmlElements.SupplierContactPerson, supplier.ContactPerson);
                supplierToUpdate.SetElementValue(XmlElements.SupplierEmail, supplier.Email);
                supplierToUpdate.SetElementValue(XmlElements.SupplierPhone, supplier.Phone);

                document.Save(_pathData);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public bool DeleteSupplier(int id)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            var supplierToDelete = document.Root!.Element(XmlElements.Suppliers)!.Elements(XmlElements.Supplier)
                .FirstOrDefault(x => (int)x.Element(XmlElements.SupplierId)! == id);
            if (supplierToDelete != null)
            {
                supplierToDelete.Remove();
                document.Save(_pathData);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public IEnumerable<Supplier> GetSuppliersWithMinProductQuantity(int minQuantity)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);

            var suppliersWithProducts = from supplier in xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Suppliers)?.Elements(XmlElements.Supplier)
                                        join product in xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Products)?.Elements(XmlElements.Product)!
                                        on (int)supplier.Element(XmlElements.SupplierId)! equals (int)product.Element(XmlElements._SupplierId)!
                                        where (int)product.Element(XmlElements.Quantity)! >= minQuantity
                                        select new Supplier
                                        {
                                            Id = (int)supplier.Element(XmlElements.SupplierId)!,
                                            Name = supplier.Element(XmlElements.SupplierName)!.Value!,
                                            ContactPerson = supplier.Element(XmlElements.SupplierContactPerson)!.Value!,
                                            Email = supplier.Element(XmlElements.SupplierEmail)!.Value!,
                                            Phone = supplier.Element(XmlElements.SupplierPhone)!.Value
                                        };

            return suppliersWithProducts;
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
            return new List<Supplier>();
        } 
    }
}

file class XmlElements
{
    public const string PathData = "PathData";
    public const string DataSource = "source";
    public const string Suppliers = "suppliers";
    public const string Supplier = "supplier";
    public const string SupplierId = "id";
    public const string SupplierName = "name";
    public const string SupplierContactPerson = "contact_person";
    public const string SupplierEmail = "email";
    public const string SupplierPhone = "phone";
    public const string Products = "products";
    public const string Product = "product";
    public const string _SupplierId = "supplierId";
    public const string Quantity = "quantity";
}
