using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace BioID.RestGrpcForwarder.Auth
{
    /// <summary>
    /// It handles API key based authentication in an ASP.NET Core application. 
    /// </summary>
    /// <param name="options">Represent authentication scheme options.</param>
    /// <param name="logger">Provides logging functionality</param>
    /// <param name="encoder">Encodes URLs safely for HTTP contexts.</param>
    /// <param name="authConfig">Represent <see cref="ApiAuthConfiguration"/>> API authentication configuration settings.</param>
    public class ApiKeyAuthSchemeHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, IOptionsMonitor<ApiAuthConfiguration> authConfig) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        private readonly ApiAuthConfiguration _authConfig = authConfig.CurrentValue;

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(_authConfig.HeaderName,out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing API Key."));
            }

            var providedApiKey = apiKeyHeaderValues.First();
            if (providedApiKey != _authConfig.ApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key provided."));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "ApiUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/json";
            var problemDetails = new ProblemDetails { Status = 401, Title = "Unauthorized", Type = "https://httpstatuses.com/401" };
            await Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
    }
}
