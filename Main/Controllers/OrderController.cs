using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/order/")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository orderRepository;

    public OrderController(IOrderRepository orderRepository)
    {
        this.orderRepository = orderRepository;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult Create([FromBody] Order order)
    {
        if (order == null) return Results.BadRequest("Object is null.");

        bool response = orderRepository.CreateOrder(order);
        if (!response) return Results.BadRequest("Object wasn't added.");

        return Results.Ok("Object added successfully.");
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IResult Get()
    {
        IEnumerable<Order> orders = orderRepository.GetOrders();
        return Results.Ok(orders);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult GetById([FromRoute] int id)
    {
        if (id <= 0) return Results.BadRequest("Id is invalid");

        Order? order = orderRepository.GetOrderById(id);
        if (order == null) return Results.NotFound("Object was not found");

        return Results.Ok(order);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult Update([FromBody] Order order)
    {
        if (order == null) return Results.BadRequest("Object is null.");

        bool response = orderRepository.UpdateOrder(order);
        if (!response) return Results.NotFound("Object was not found.");

        return Results.Ok("Object was updated.");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult Delete([FromRoute] int id)
    {
        if (id <= 0) return Results.BadRequest("Id is invalid");

        bool response = orderRepository.DeleteOrder(id);
        if (!response) return Results.NotFound("Object was not found");

        return Results.Ok("Object was deleted");
    }


    [HttpGet("supplier-status")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IResult GetOrdersBySupplierAndStatus([FromQuery] int supplierId, [FromQuery] OrderStatus status)
    {
        if (supplierId <= 0) return Results.BadRequest("Invalid supplierId");

        var orders = orderRepository.GetOrdersBySupplierAndStatus(supplierId, status);

        if (orders == null) return Results.NotFound("No orders found for the specified supplier and status.");

        return Results.Ok(orders);
    }

    [HttpGet("by-date")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IResult GetOrdersByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate) return Results.BadRequest("Start date cannot be later than end date.");

        var orders = orderRepository.GetOrdersByDate(startDate, endDate);

        if (!orders.Any()) return Results.NotFound("No orders found in the specified date range.");

        return Results.Ok(orders);
    }
}
