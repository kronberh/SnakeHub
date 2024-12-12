using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SnakeHub.Services;
using System.Security.Claims;

namespace SnakeHub.Handlers
{
    public class JwtAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory loggerFactory, System.Text.Encodings.Web.UrlEncoder encoder, ISystemClock clock, JwtService jwt) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder, clock)
    {
        private readonly JwtService _jwt = jwt;
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Cookies.TryGetValue("AuthToken", out string? token) || string.IsNullOrEmpty(token))
            {
                return Task.FromResult(AuthenticateResult.Fail("No token found."));
            }
            try
            {
                ClaimsPrincipal claimsPrincipal = _jwt.GetClaimsFromJwtToken(token);
                AuthenticationTicket ticket = new(claimsPrincipal, Scheme.Name);
                string newToken = _jwt.GenerateJwtToken(claimsPrincipal);
                Context.Response.Cookies.Delete("AuthToken");
                Context.Response.Cookies.Append("AuthToken", newToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                });
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                Context.Response.Cookies.Delete("AuthToken");
                return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
            }
        }
    }
}
