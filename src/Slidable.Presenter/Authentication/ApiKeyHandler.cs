using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Slidable.Presenter.Authentication
{
    public class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
    {
        private static readonly char[] ApiKeyStart = "API-Key ".ToCharArray();
        private readonly ApiKeyCheck _apiKeyCheck;

        public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger,
            encoder, clock)
        {
            _apiKeyCheck = new ApiKeyCheck(options);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(HandleAuthenticate());
        }

        private AuthenticateResult HandleAuthenticate()
        {
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!authorizationHeader.AsReadOnlySpan(0, 8).SequenceEqual(new ReadOnlySpan<char>(ApiKeyStart)))
            {
                return AuthenticateResult.NoResult();
            }

            if (_apiKeyCheck.Check(authorizationHeader.AsReadOnlySpan(8), out string user))
            {
                var claimsIdentity = new ClaimsIdentity();
                claimsIdentity.AddClaim(new Claim("user", new string(user)));
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, ApiKeyAuthenticationDefaults.AuthenticationScheme));
            }

            return AuthenticateResult.NoResult();
        }

        public static ReadOnlySpan<char> GetUserName(string header, int colon)
        {
            return header.AsReadOnlySpan(8, colon - 8);
        }

        public static ReadOnlySpan<char> GetKey(string header, int colon)
        {
            return header.AsReadOnlySpan(colon + 1);
        }
    }
}