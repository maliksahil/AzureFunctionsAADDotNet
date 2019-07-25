using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using DotNetAuthorizeFunction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;

namespace DotNetAuthorizeFunction
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeAttribute : FunctionInvocationFilterAttribute
    {
        private IAuthenticationService authenticationService;

        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            HttpContext context = GetHttpContext(executingContext);

            authenticationService = (IAuthenticationService)context.RequestServices.GetService(typeof(IAuthenticationService));

            ClaimsPrincipal claimsPrincipal = await authenticationService?.ValidateTokenAsync(context.Request);
            bool isAuthenticated = claimsPrincipal != null;

            if (!isAuthenticated)
            {
                HttpResponse response = context.Response;

                // set unauthorized at response start, overwriting the default 500 error
                response.OnStarting(state =>
                    {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.CompletedTask;
                    }, null);

                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        private HttpContext GetHttpContext(FunctionExecutingContext context)
        {
            var requestArg = context.Arguments.Values.FirstOrDefault(x => x is HttpRequest);

            if (requestArg is HttpRequest request)
                return request.HttpContext;

            return null;
        }

    }
}