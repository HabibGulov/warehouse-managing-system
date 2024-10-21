using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/category/")]

public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IResult Create([FromBody] Category category)
    {
        if (category == null) return Results.BadRequest("Object is null.");
        bool response = categoryRepository.CreateCategory(category);
        if (response == false) return Results.BadRequest("Object wasn't added.");
        return Results.Ok("Object added succesfully.");
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IResult Get()
    {
        IEnumerable<Category> categories = categoryRepository.GetCategories();
        return Results.Ok(categories);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult Update([FromBody] Category category)
    {
        if (category == null) return Results.BadRequest("Object is null.");
        bool response = categoryRepository.UpdateCategory(category);
        if (response == false) return Results.NotFound("Object was not found.");
        return Results.Ok("Object was updated.");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult Delete([FromRoute] int id)
    {
        if (id == 0) return Results.BadRequest("Id is invalid");
        bool response = categoryRepository.DeleteCategory(id);
        if (response == false) return Results.NotFound("Object was not found");
        return Results.Ok("Object was deleted");
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult GetById([FromRoute] int id)
    {
        if (id <= 0) return Results.BadRequest("Id is invalid");
        Category? category = categoryRepository.GetCategoryById(id);
        if (category == null) return Results.NotFound("Object was not found");
        return Results.Ok(category);
    }

    [HttpGet("withProductCount")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IResult GetCategoriesWithProductCount()
    {
        var categories = categoryRepository.GetCategoriesWithProductCount();

        if (categories == null) return Results.NotFound("No categories found.");
        
        return Results.Ok(categories);
    }

}