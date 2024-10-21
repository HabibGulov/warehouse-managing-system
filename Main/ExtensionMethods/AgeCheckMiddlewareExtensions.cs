public static class AgeCheckMiddlewareExtensions
{
    public static IApplicationBuilder UseAgeCheck(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AgeCheckMiddleware>();
    }
}