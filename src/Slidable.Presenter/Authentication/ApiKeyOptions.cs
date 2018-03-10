using System;
using Microsoft.AspNetCore.Authentication;

namespace Slidable.Presenter.Authentication
{
    public class ApiKeyOptions : AuthenticationSchemeOptions
    {
        public string ApiKeyHashPhrase { get; set; }
    }

    public static class ApiKeyExtensions
    {
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyOptions> configureOptions) =>
            builder.AddScheme<ApiKeyOptions, ApiKeyHandler>(ApiKeyAuthenticationDefaults.AuthenticationScheme, configureOptions);
    }
}