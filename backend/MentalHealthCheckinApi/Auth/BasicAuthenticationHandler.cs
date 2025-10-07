using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace MentalHealthCheckinApi.Auth;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private static readonly Dictionary<string, (string Password, string Role, Guid UserId)> Users = new()
    {
        ["bob"] = ("password123", "manager", Guid.Parse("22222222-2222-2222-2222-222222222222")),
        ["alice"] = ("password123", "employee", Guid.Parse("11111111-1111-1111-1111-111111111111"))
    };

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TimeProvider timeProvider)
        : base(options, logger, encoder)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.Fail("Invalid auth scheme"));

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? "");
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            if (credentials.Length != 2)
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));

            var username = credentials[0];
            var password = credentials[1];

            if (!Users.TryGetValue(username, out var info) || info.Password != password)
                return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, info.Role),
                new Claim(ClaimTypes.NameIdentifier, info.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
        }
    }
}
