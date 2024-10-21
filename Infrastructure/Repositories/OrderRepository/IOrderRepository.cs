public interface IOrderRepository
{
    IEnumerable<Order> GetOrders();
    Order? GetOrderById(int id);
    bool CreateOrder(Order order);
    bool UpdateOrder(Order order);
    bool DeleteOrder(int id);
    IEnumerable<Order> GetOrdersBySupplierAndStatus(int supplierId, OrderStatus status);
    IEnumerable<Order> GetOrdersByDate(DateTime startDate, DateTime endDate);
}