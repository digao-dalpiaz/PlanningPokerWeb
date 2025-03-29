using Microsoft.AspNetCore.SignalR;

namespace api.Exceptions
{
    public class HubExceptionHandler : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, 
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                if (ex is ValidacaoException) throw new HubException(ex.Message);

                await DbService.GravarException(invocationContext.Context.GetHttpContext(), ex);
                throw new HubException("Erro inesperado no WebSocket do servidor");
            }
        }
    }
}
