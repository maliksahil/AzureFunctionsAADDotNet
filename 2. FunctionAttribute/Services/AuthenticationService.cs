using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DotNetAuthorizeFunction.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace DotNetAuthorizeFunction.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        // set true if you want validation to always pass for debugging
        private readonly bool DEBUG = false;
        
        private readonly IConfigurationManager<OpenIdConnectConfiguration> configManager;
        private readonly ILogger log;
        private readonly Settings settings;

        // TODO: ADD ILOGGER
        public AuthenticationService(IOptions<Settings> options, ILoggerFactory loggerFactory)
        {
            settings = options.Value;
            log = loggerFactory.CreateLogger<AuthenticationService>();

            var documentRetriever = new HttpDocumentRetriever
            {
                RequireHttps = settings.Issuer.StartsWith("https://")
            };

            configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                settings.MetadataUrl,
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(HttpRequest req)
        {
            string token = ExtractToken(req);
            if (token == null)
            {
                log.LogInformation("UNAUTHORIZED. No valid token provided.");
                return null;
            }

            // for debugging
            if (DEBUG && token.Length > 0) return new ClaimsPrincipal();

            ClaimsPrincipal result = null;

            var config = await configManager.GetConfigurationAsync(CancellationToken.None);

            var validationParam = new TokenValidationParameters()
            {
                ValidAudiences = new [] { settings.Audience, settings.ClientId },
                ValidIssuer = settings.Issuer,
                IssuerSigningKeys = config.SigningKeys
            };

            try 
            {
                var handler = new JwtSecurityTokenHandler();
                result = handler.ValidateToken(token, validationParam, out var validatedToken);
                log.LogInformation("Authenticated successfully.");
                return result;
            }
            catch (SecurityTokenException e)
            {
                log.LogInformation($"UNAUTHORIZED. Reason:\n{e.Message}\n");
                return null;
            }
            catch (Exception e)
            {
                log.LogError($"ERROR:\n{e.Message}");
                return null;
            }

        }

        private string ExtractToken(HttpRequest req)
        {
            string authorizationHeader = req?.Headers["Authorization"];
            string[] parts = authorizationHeader?.Split(null) ?? new string[0];

            // is properly-formatted bearer token
            if (parts.Length == 2 && parts[0].Equals("Bearer"))
                return parts[1];

            return null;
        }
    }
}