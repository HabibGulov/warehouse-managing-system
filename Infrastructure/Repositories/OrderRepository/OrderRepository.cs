using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

public sealed class OrderRepository : IOrderRepository
{
    private readonly string _pathData;

    public OrderRepository(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", "true");
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Orders));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public IEnumerable<Order> GetOrders()
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            return document.Root!.Element(XmlElements.Orders)!.Elements(XmlElements.Order)
                .Select(x => new Order
                {
                    Id = (int)x.Element(XmlElements.OrderId)!,
                    ProductId = (int)x.Element(XmlElements.ProductId)!,
                    Quantity = (int)x.Element(XmlElements.Quantity)!,
                    OrderDate = (DateTime)x.Element(XmlElements.OrderDate)!,
                    SupplierId = (int)x.Element(XmlElements.SupplierId)!,
                    Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), (string)x.Element(XmlElements.Status)!)
                }).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Order? GetOrderById(int id)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            var order = xDocument.Root!.Element(XmlElements.Orders)!.Elements(XmlElements.Order)
                .Where(x => (int)x.Element(XmlElements.OrderId)! == id)
                .Select(x => new Order
                {
                    Id = (int)x.Element(XmlElements.OrderId)!,
                    ProductId = (int)x.Element(XmlElements.ProductId)!,
                    Quantity = (int)x.Element(XmlElements.Quantity)!,
                    OrderDate = (DateTime)x.Element(XmlElements.OrderDate)!,
                    SupplierId = (int)x.Element(XmlElements.SupplierId)!,
                    Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), (string)x.Element(XmlElements.Status)!)
                }).FirstOrDefault();
            return order;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public bool CreateOrder(Order order)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            int maxId = document.Element(XmlElements.DataSource)!.Element(XmlElements.Orders)!.Elements(XmlElements.Order)
                .Select(x => (int)x.Element(XmlElements.OrderId)!).DefaultIfEmpty(0).Max();

            XElement xmlXElement = new XElement(XmlElements.Order,
                new XElement(XmlElements.OrderId, maxId + 1),
                new XElement(XmlElements.ProductId, order.ProductId),
                new XElement(XmlElements.Quantity, order.Quantity),
                new XElement(XmlElements.OrderDate, order.OrderDate),
                new XElement(XmlElements.SupplierId, order.SupplierId),
                new XElement(XmlElements.Status, order.Status.ToString()));

            document.Element(XmlElements.DataSource)!.Element(XmlElements.Orders)!.Add(xmlXElement);
            document.Save(_pathData);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public bool UpdateOrder(Order order)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            var orderToUpdate = xDocument.Root!.Element(XmlElements.Orders)!.Elements(XmlElements.Order)
                .FirstOrDefault(x => (int)x.Element(XmlElements.OrderId)! == order.Id);
            if (orderToUpdate != null)
            {
                orderToUpdate.SetElementValue(XmlElements.ProductId, order.ProductId);
                orderToUpdate.SetElementValue(XmlElements.Quantity, order.Quantity);
                orderToUpdate.SetElementValue(XmlElements.OrderDate, order.OrderDate);
                orderToUpdate.SetElementValue(XmlElements.SupplierId, order.SupplierId);
                orderToUpdate.SetElementValue(XmlElements.Status, order.Status.ToString());

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

    public bool DeleteOrder(int id)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            var orderToDelete = document.Root!.Element(XmlElements.Orders)!.Elements(XmlElements.Order)
                .FirstOrDefault(x => (int)x.Element(XmlElements.OrderId)! == id);
            if (orderToDelete != null)
            {
                orderToDelete.Remove();
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
    public IEnumerable<Order> GetOrdersBySupplierAndStatus(int supplierId, OrderStatus status)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);

            var orders = xDocument.Element(XmlElements.DataSource)!
                                .Element(XmlElements.Orders)!
                                .Elements(XmlElements.Order)
                                .Where(o => (int)o.Element(XmlElements.SupplierId)! == supplierId &&
                                            Enum.TryParse<OrderStatus>((string)o.Element("status")!, out var orderStatus) &&
                                            orderStatus == status)
                                .Select(o => new Order
                                {
                                    Id = (int)o.Element("id")!,
                                    ProductId = (int)o.Element("productId")!,
                                    Quantity = (int)o.Element("quantity")!,
                                    OrderDate = (DateTime)o.Element("orderDate")!,
                                    SupplierId = (int)o.Element("supplierId")!,
                                    Status = Enum.Parse<OrderStatus>((string)o.Element("status")!)
                                });

            return orders.ToList();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return new List<Order>();
        }
    }
    public IEnumerable<Order> GetOrdersByDate(DateTime startDate, DateTime endDate)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);

            var orders = from order in xDocument.Element(XmlElements.DataSource)?.Element(XmlElements.Orders)?.Elements(XmlElements.Order)
                         let orderDate = DateTime.Parse(order.Element(XmlElements.OrderDate)!.Value)
                         where orderDate >= startDate && orderDate <= endDate
                         select new Order
                         {
                             Id = (int)order.Element(XmlElements.OrderId)!,
                             ProductId = (int)order.Element(XmlElements.ProductId)!,
                             Quantity = (int)order.Element(XmlElements.Quantity)!,
                             OrderDate = orderDate,
                             SupplierId = (int)order.Element(XmlElements.SupplierId)!,
                             Status = Enum.Parse<OrderStatus>(order.Element(XmlElements.Status)!.Value)
                         };

            return orders;
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
            return new List<Order>();
        } 
    }
}

file class XmlElements
{
    public const string PathData = "PathData";
    public const string DataSource = "source";
    public const string Orders = "orders";
    public const string Order = "order";
    public const string OrderId = "id";
    public const string ProductId = "productId";
    public const string Quantity = "quantity";
    public const string OrderDate = "orderDate";
    public const string SupplierId = "supplierId";
    public const string Status = "status";
}
