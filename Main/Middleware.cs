public class AgeCheckMiddleware
{
    private readonly RequestDelegate _next;

    public AgeCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Age", out var ageHeader) && int.TryParse(ageHeader, out int age))
        {
            if (age >= 18)
            {
                await _next(context); 
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access Denied. You must be at least 18 years old.");
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Age information is required.");
        }
    }
}