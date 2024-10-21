using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

public sealed class ProductRepository : IProductRepository
{
    private readonly string _pathData;

    public ProductRepository(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", "true");
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Products));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public IEnumerable<Product> GetProducts()
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            return document.Root!.Element(XmlElements.Products)!.Elements(XmlElements.Product)
                .Select(x => new Product
                {
                    Id = (int)x.Element(XmlElements.ProductId)!,
                    Name = (string)x.Element(XmlElements.ProductName)!,
                    Description = (string)x.Element(XmlElements.ProductDescription)!,
                    Quantity = (int)x.Element(XmlElements.ProductQuantity)!,
                    Price = (decimal)x.Element(XmlElements.ProductPrice)!,
                    CategoryId = (int)x.Element(XmlElements.CategoryId)!
                }).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Product? GetProductById(int id)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            var product = xDocument.Root!.Element(XmlElements.Products)!.Elements(XmlElements.Product)
                .Where(x => (int)x.Element(XmlElements.ProductId)! == id)
                .Select(x => new Product
                {
                    Id = (int)x.Element(XmlElements.ProductId)!,
                    Name = (string)x.Element(XmlElements.ProductName)!,
                    Description = (string)x.Element(XmlElements.ProductDescription)!,
                    Quantity = (int)x.Element(XmlElements.ProductQuantity)!,
                    Price = (decimal)x.Element(XmlElements.ProductPrice)!,
                    CategoryId = (int)x.Element(XmlElements.CategoryId)!
                }).FirstOrDefault();
            return product;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public bool CreateProduct(Product product)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            int maxId = document.Element(XmlElements.DataSource)!.Element(XmlElements.Products)!.Elements(XmlElements.Product)
                .Select(x => (int)x.Element(XmlElements.ProductId)!).DefaultIfEmpty(0).Max();

            bool isNameExists = document.Element(XmlElements.DataSource)!.Element(XmlElements.Products)!
                .Elements(XmlElements.Product).Any(x => (string)x.Element(XmlElements.ProductName)! == product.Name);
            if (isNameExists) return false;

            XElement xmlXElement = new XElement(XmlElements.Product,
                new XElement(XmlElements.ProductId, maxId + 1),
                new XElement(XmlElements.ProductName, product.Name),
                new XElement(XmlElements.ProductDescription, product.Description),
                new XElement(XmlElements.ProductQuantity, product.Quantity),
                new XElement(XmlElements.ProductPrice, product.Price),
                new XElement(XmlElements.CategoryId, product.CategoryId));

            document.Element(XmlElements.DataSource)!.Element(XmlElements.Products)!.Add(xmlXElement);
            document.Save(_pathData);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public bool UpdateProduct(Product product)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            var productToUpdate = xDocument.Root!.Element(XmlElements.Products)!.Elements(XmlElements.Product)
                .FirstOrDefault(x => (int)x.Element(XmlElements.ProductId)! == product.Id);
            if (productToUpdate != null)
            {
                productToUpdate.SetElementValue(XmlElements.ProductName, product.Name);
                productToUpdate.SetElementValue(XmlElements.ProductDescription, product.Description);
                productToUpdate.SetElementValue(XmlElements.ProductQuantity, product.Quantity);
                productToUpdate.SetElementValue(XmlElements.ProductPrice, product.Price);
                productToUpdate.SetElementValue(XmlElements.CategoryId, product.CategoryId);

                xDocument.Save(_pathData);
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

    public bool DeleteProduct(int id)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            var productToDelete = document.Root!.Element(XmlElements.Products)!.Elements(XmlElements.Product)
                .FirstOrDefault(x => (int)x.Element(XmlElements.ProductId)! == id);
            if (productToDelete != null)
            {
                productToDelete.Remove();
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

    public IEnumerable<Product> GetProductsByCategory(string categoryName)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);

            var categoryId = (from category in xDocument.Element(XmlElements.DataSource)!
                                              .Element(XmlElements.Categories)!
                                              .Elements(XmlElements.Category)
                              where (string)category.Element(XmlElements.CategoryName)! == categoryName
                              select (int)category.Element(XmlElements.CategoryId)!).FirstOrDefault();

            if (categoryId == 0)
            {
                return Enumerable.Empty<Product>();
            }

            return (from product in xDocument.Element(XmlElements.DataSource)!
                                          .Element(XmlElements.Products)!
                                          .Elements(XmlElements.Product)
                    where (int)product.Element(XmlElements.CategoryId)! == categoryId
                    select new Product
                    {
                        Id = (int)product.Element(XmlElements.ProductId)!,
                        Name = (string)product.Element(XmlElements.ProductName)!,
                        Description = (string)product.Element(XmlElements.ProductDescription)!,
                        Quantity = (int)product.Element(XmlElements.ProductQuantity)!,
                        Price = (decimal)product.Element(XmlElements.ProductPrice)!
                    }
                    into sortedProducts
                    orderby sortedProducts.Price descending
                    select sortedProducts).ToList();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            return Enumerable.Empty<Product>();
        }
    }

    public IEnumerable<Product> GetProductsByMaxQuantity(int maxQuantity)
    {
        XDocument xDocument = XDocument.Load(_pathData);

        var products = xDocument.Element(XmlElements.DataSource)!
                                .Element(XmlElements.Products)!
                                .Elements(XmlElements.Product)
                                .Where(p => (int)p.Element(XmlElements.ProductQuantity)! < maxQuantity)
                                .Select(p => new Product
                                {
                                    Id = (int)p.Element(XmlElements.ProductId)!,
                                    Name = (string)p.Element(XmlElements.ProductName)!,
                                    Description = (string)p.Element(XmlElements.ProductDescription)!,
                                    Quantity = (int)p.Element(XmlElements.ProductQuantity)!,
                                    Price = (decimal)p.Element(XmlElements.ProductPrice)!,
                                    CategoryId = (int)p.Element(XmlElements.CategoryId)!
                                });
        return products;
    }

    public IEnumerable<ProductDetails> GetProductDetailsById(int productId)
    {
        try
        {
            XDocument xDocument = XDocument.Load("path_to_xml_file");

            var productDetails = from product in xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Products)?.Elements(XmlElements.Product)
                                 where (int)product.Element(XmlElements.ProductId)! == productId
                                 join category in xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Categories)?.Elements(XmlElements.Category)!
                                 on (int)product.Element(XmlElements.CategoryId)! equals (int)category.Element(XmlElements._CategoryId)!
                                 join order in xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Orders)?.Elements(XmlElements.Order)!
                                 on (int)product.Element(XmlElements.ProductId)! equals (int)order.Element(XmlElements._ProductId)!
                                 join supplier in xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Suppliers)?.Elements(XmlElements.Supplier)!
                                 on (int)order.Element(XmlElements.SupplierId)! equals (int)supplier.Element(XmlElements._SupplierId)!
                                 select new ProductDetails
                                 {
                                     ProductId = (int)product.Element(XmlElements.ProductId)!,
                                     ProductName = product.Element(XmlElements.ProductName)?.Value!,
                                     ProductDescription = product.Element(XmlElements.ProductDescription)?.Value!,
                                     Price = (decimal)product.Element(XmlElements.ProductPrice)!,
                                     Quantity = (int)product.Element(XmlElements.ProductQuantity)!,
                                     CategoryName = category.Element(XmlElements.CategoryName)?.Value!,
                                     SupplierName = supplier.Element(XmlElements.SupplierName)?.Value!,
                                     SupplierContactPerson = supplier.Element("contact_person")?.Value!
                                 };

            return productDetails;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return new List<ProductDetails>();
        }
    }
    public IEnumerable<Product> GetMostOrderedProducts(int minOrders)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);

            var orders = from order in xDocument.Root!.Element(XmlElements.Orders)!.Elements(XmlElements.Order)
                         group order by (int)order.Element(XmlElements._ProductId)! into orderGroup
                         where orderGroup.Count() > minOrders
                         select orderGroup.Key;

            var products = from product in xDocument.Root.Element(XmlElements.Products)!.Elements(XmlElements.Product)
                           where orders.Contains((int)product.Element(XmlElements.ProductId)!)
                           select new Product
                           {
                               Id = (int)product.Element(XmlElements.ProductId)!,
                               Name = product.Element(XmlElements.ProductName)!.Value,
                               Description = product.Element(XmlElements.ProductDescription)!.Value,
                               Quantity = (int)product.Element(XmlElements.ProductQuantity)!,
                               Price = decimal.Parse(product.Element(XmlElements.ProductPrice)!.Value),
                               CategoryId = (int)product.Element(XmlElements.CategoryId)!
                           };

            return products;
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
            return new List<Product>();
        }
    }
}

file class XmlElements
{
    public const string PathData = "PathData";
    public const string DataSource = "source";
    public const string Products = "products";
    public const string Product = "product";
    public const string ProductId = "id";
    public const string ProductName = "name";
    public const string ProductDescription = "description";
    public const string ProductQuantity = "quantity";
    public const string ProductPrice = "price";
    public const string CategoryId = "categoryId";
    public const string Categories = "categories";
    public const string Category = "category";
    public const string CategoryName = "name";
    public const string SupplierId = "supplierId";
    public const string _CategoryId = "Id";
    public const string Orders = "orders";
    public const string Order = "order";
    public const string _ProductId = "productId";
    public const string Suppliers = "suppliers";
    public const string Supplier = "supplier";
    public const string _SupplierId = "id";
    public const string SupplierName = "name";
}
