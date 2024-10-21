using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/product/")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository productRepository;

    public ProductController(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult Create([FromBody] Product product)
    {
        if (product == null) return Results.BadRequest("Object is null.");

        bool response = productRepository.CreateProduct(product);
        if (!response) return Results.BadRequest("Product with the same name already exists.");

        return Results.Ok("Product added successfully.");
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IResult Get()
    {
        IEnumerable<Product> products = productRepository.GetProducts();
        return Results.Ok(products);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult GetById([FromRoute] int id)
    {
        if (id <= 0) return Results.BadRequest("Id is invalid");

        Product? product = productRepository.GetProductById(id);
        if (product == null) return Results.NotFound("Product was not found");

        return Results.Ok(product);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult Update([FromBody] Product product)
    {
        if (product == null) return Results.BadRequest("Object is null.");

        bool response = productRepository.UpdateProduct(product);
        if (!response) return Results.NotFound("Product was not found.");

        return Results.Ok("Product was updated.");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult Delete([FromRoute] int id)
    {
        if (id <= 0) return Results.BadRequest("Id is invalid");

        bool response = productRepository.DeleteProduct(id);
        if (!response) return Results.NotFound("Product was not found");

        return Results.Ok("Product was deleted");
    }

    [HttpGet("by-category-name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult GetProductsByCategoryName([FromQuery] string categoryName)
    {
        if (categoryName == null) return Results.BadRequest("Category name must be provided.");
        var products = productRepository.GetProductsByCategory(categoryName);
        if (products == null) return Results.NotFound($"No products found for category '{categoryName}'.");
        return Results.Ok(products);
    }

    [HttpGet("max-quantity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult GetProductsByMaxQuantity([FromQuery] int maxQuantity)
    {
        if (maxQuantity <= 0) return Results.BadRequest("Invalid quantity value.");

        var products = productRepository.GetProductsByMaxQuantity(maxQuantity);

        if (products == null) return Results.NotFound("No products found with such quantity.");
        return Results.Ok(products);
    }

    [HttpGet("{id}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult GetProductDetailsById(int id)
    {
        if (id <= 0) return Results.BadRequest("Invalid product ID.");

        var productDetails = productRepository.GetProductDetailsById(id);
        if (productDetails == null) return Results.NotFound($"Product with ID {id} not found.");

        return Results.Ok(productDetails);
    }

    [HttpGet("mostOrdered")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult GetMostOrderedProducts([FromQuery] int minOrders)
    {
        if (minOrders <= 0) return Results.BadRequest("minOrders must be greater than 0");

        var products = productRepository.GetMostOrderedProducts(minOrders);
        if (!products.Any()) return Results.NotFound("No products found with the specified order count.");

        return Results.Ok(products);
    }
}
