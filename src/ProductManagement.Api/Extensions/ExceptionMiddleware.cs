namespace ProductManagement.Api.Extensions;

public static class ExceptionMiddleware
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}