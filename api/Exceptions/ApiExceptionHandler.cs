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

                    string msg;
                    if (ex.Error is ValidacaoException)
                    {
                        msg = ex.Error.Message;
                        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    }
                    else
                    {
                        msg = "Erro inesperado na Api do servidor";
                        await DbService.GravarException(context, ex.Error);
                        //por padrão retorna erro 500
                    }
                    
                    await context.Response.WriteAsync(msg);
                });
            });
        }
    }
}
