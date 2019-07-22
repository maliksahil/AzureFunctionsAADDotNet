using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;

using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Linq;
using System.Security.Claims;

namespace DotNetAuthorizeFunction
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeAttribute : FunctionInvocationFilterAttribute
    {
        private readonly string bearerPrefix = "Bearer";

        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            HttpContext context = this.GetHttpContext(executingContext);
            string token = this.ExtractToken(context);
            ClaimsPrincipal claimsPrincipal = await ValidateAccessToken(token);

            var isAuthenticated = claimsPrincipal != null;
            System.Console.Out.WriteLine(isAuthenticated ? "Authorized" : "Unauthorized");

            if (!isAuthenticated)
            {
                // the response to be sent to the caller
                HttpResponse response = context.Response;

                // set unauthorized at response start, overwriting the default 500 error
                response.OnStarting(state =>
                    {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.CompletedTask;
                    }, null);

                // prevents Azure function from running
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

        private string ExtractToken(HttpContext context)
        {
            if (context == null)
                return null;

            string authorizationHeader = context.Request?.Headers?["Authorization"];
            string[] parts = authorizationHeader?.Split(null) ?? new string[0];

            // is properly-formatted bearer token
            if (parts.Length == 2 && parts[0].Equals(bearerPrefix))
                return parts[1];

            return null;
        }


        private static async Task<ClaimsPrincipal> ValidateAccessToken(string accessToken)
        {
            var audience = Constants.audience;
            var clientID = Constants.clientID;
            var tenant = Constants.tenant;
            var tenantid = Constants.tenantid;
            var aadInstance = Constants.aadInstance;
            var authority = Constants.authority;
            var validIssuers = Constants.validIssuers;

            // Debugging purposes only, set this to false for production
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            ConfigurationManager<OpenIdConnectConfiguration> configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());

            OpenIdConnectConfiguration config = null;
            config = await configManager.GetConfigurationAsync();

            ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();

            // Initialize the token validation parameters
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                // App Id URI and AppId of this service application are both valid audiences.
                ValidAudiences = new[] { audience, clientID },

                // Support Azure AD V1 and V2 endpoints.
                ValidIssuers = validIssuers,
                IssuerSigningKeys = config.SigningKeys
            };

            try
            {
                SecurityToken securityToken;
                var claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out securityToken);
                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); // TODO: Make this logging better using ILogger
            }
            return null;
        }
    }

}