using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotNetAuthorizeFunction.Services
{
    public interface IAuthenticationService
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(HttpRequest httpRequest);
    }
}