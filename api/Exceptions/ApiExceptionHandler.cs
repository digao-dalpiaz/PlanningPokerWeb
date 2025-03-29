using Microsoft.AspNetCore.Diagnostics;

namespace api.Exceptions
{
    public class ApiExceptionHandler
    {
        public static void ConfigurarExceptionHandler(WebApplication app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var ex = context.Features.Get<IExceptionHandlerPathFeature>();

                    if (ex.Error is ValidacaoException)
                    {
                        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    }
                    await context.Response.WriteAsync(ex.Error.Message);
                });
            });
        }
    }
}
