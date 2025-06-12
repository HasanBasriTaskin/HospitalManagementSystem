using static System.Net.Mime.MediaTypeNames;

namespace Api.MiddleWares
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleWare> logger;

        public ExceptionMiddleWare(RequestDelegate _next, ILogger<ExceptionMiddleWare> _logger)
        {
            this.next = _next;
            this.logger = _logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next.Invoke(httpContext);
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message);
            }
        }
    }
}
