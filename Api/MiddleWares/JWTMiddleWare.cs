using System.Net.Http;

namespace Api.MiddleWares
{
    public class JWTMiddleWare
    {
        private readonly RequestDelegate _next;

        public JWTMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["JWTCookie"];
            if (token != null)
            {
                context.Request.Headers.Add("Authorization", "Bearer " + token);
            }

            await _next(context);
        }
    }
}