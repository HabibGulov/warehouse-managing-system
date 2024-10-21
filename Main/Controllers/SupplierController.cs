using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/supplier/")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierRepository supplierRepository;

    public SupplierController(ISupplierRepository supplierRepository)
    {
        this.supplierRepository = supplierRepository;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult Create([FromBody] Supplier supplier)
    {
        if (supplier == null) return Results.BadRequest("Object is null.");

        bool response = supplierRepository.CreateSupplier(supplier);
        if (!response) return Results.BadRequest("Supplier with the same name already exists.");

        return Results.Ok("Supplier added successfully.");
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IResult Get()
    {
        IEnumerable<Supplier> suppliers = supplierRepository.GetSuppliers();
        return Results.Ok(suppliers);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult GetById([FromRoute] int id)
    {
        if (id <= 0) return Results.BadRequest("Id is invalid");

        Supplier? supplier = supplierRepository.GetSupplierById(id);
        if (supplier == null) return Results.NotFound("Supplier was not found");

        return Results.Ok(supplier);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult Update([FromBody] Supplier supplier)
    {
        if (supplier == null) return Results.BadRequest("Object is null.");

        bool response = supplierRepository.UpdateSupplier(supplier);
        if (!response) return Results.NotFound("Supplier was not found.");

        return Results.Ok("Supplier was updated.");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult Delete([FromRoute] int id)
    {
        if (id <= 0) return Results.BadRequest("Id is invalid");

        bool response = supplierRepository.DeleteSupplier(id);
        if (!response) return Results.NotFound("Supplier was not found");

        return Results.Ok("Supplier was deleted");
    }

    [HttpGet("product-minquantity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult GetSuppliersByMinProductQuantity([FromQuery] int minProductQuantity)
    {
        if (minProductQuantity < 0) return Results.BadRequest("Invalid minimum product quantity.");

        var suppliers = supplierRepository.GetSuppliersWithMinProductQuantity(minProductQuantity);

        if (!suppliers.Any()) return Results.NotFound("No suppliers found with the specified minimum product quantity.");

        return Results.Ok(suppliers);
    }

}
