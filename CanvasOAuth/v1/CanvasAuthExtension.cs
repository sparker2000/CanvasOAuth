using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CanvasOAuth
{
    /// <summary>
    /// Configuration options for Canvas OAuth
    /// </summary>
    [Obsolete("For use with v1 of CanvasOAuth")]
    public class CanvasAuthOptions
    {
        /// <summary>
        /// Public identifier for app. (See Developer Keys Canvas Documentation)
        /// </summary>
        /// <example>10430000000074084</example>
        public string ClientId { get; set; } = "";

        /// <summary>
        /// Secret known only by the application and the authorization server. (See Developer Keys Canvas Documentation)
        /// </summary>
        /// <example>8C6DPAhTqWvsJxxVFpbpuJTAKX5tZYKHp3fzHp8EDuwQBgDJDRBR95sbq9BDuTQf</example>
        public string ClientSecret { get; set; } = "";

        /// <summary>
        /// The base url of the Canvas LMS portal
        /// </summary>
        /// <example>https://my-company.instructure.com/</example>
        public string CanvasUrl { get; set; } = "";

        /// <summary>
        /// After the user signs in, an authorization code will be sent to a callback
        /// in this app. The OAuth middleware will intercept it
        /// </summary>
        /// <example>"/signin"</example>
        public string CallbackPath { get; set; } = "/signin";
    }

    [Obsolete("For use with v1 of CanvasOAuth")]
    public static class CanvasAuthExtension
    {
        /// <summary>
        /// Adds OAuth 2.0 based authentication to <see cref="IServiceCollection"/> using Canvas Authentication Scheme.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="canvasOptions">The <see cref="CanvasAuthOptions"/>.</param>
        /// <returns>A reference to <see cref="AuthenticationBuilder"/> after the operation has completed.</returns>
        /// <term>example, code</term>
        /// <description>
        /// These are used for code examples
        /// </description>

        /// <remarks>
        /// <code>
        /// // Add in Program.cs
        /// builder.Services.AddCanvasOAuth(new CanvasAuthOptions
        /// {
        ///     CanvasUrl = builder.Configuration["Canvas:CanvasUrl"],
        ///     ClientId = builder.Configuration["Canvas:ClientId"],
        ///     ClientSecret = builder.Configuration["Canvas:ClientSecret"]
        /// });
        ///
        /// app.UseAuthentication();
        /// app.UseAuthorization();
        /// </code>
        /// </remarks>
        [Obsolete("For use with v1 of CanvasOAuth")]
        public static AuthenticationBuilder AddCanvasOAuth(this IServiceCollection serviceCollection, CanvasAuthOptions canvasOptions)
        {
            return serviceCollection.AddAuthentication(options =>
            {
                // If an authentication cookie is present, use it to get authentication information
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                // If authentication is required, and no cookie is present, use Canvas (configured below) to sign in
                options.DefaultChallengeScheme = "Canvas";
            })
            .AddCookie() // cookie authentication middleware first
            .AddOAuth("Canvas", options => // Oauth authentication middleware is second
            {
                // When a user needs to sign in, they will be redirected to the authorize endpoint
                options.AuthorizationEndpoint = $"{canvasOptions.CanvasUrl}login/oauth2/auth";

                // The OAuth middleware will send the ClientId, ClientSecret, and the
                // authorization code to the token endpoint, and get an access token in return
                options.ClientId = canvasOptions.ClientId;
                options.ClientSecret = canvasOptions.ClientSecret;
                options.TokenEndpoint = $"{canvasOptions.CanvasUrl}login/oauth2/token";

                // After the user signs in, an authorization code will be sent to a callback
                // in this app. The OAuth middleware will intercept it
                options.CallbackPath = new PathString(canvasOptions.CallbackPath);

                // Below we call the UserInformationEndpoint endpoint to get information about the user
                options.UserInformationEndpoint = $"{canvasOptions.CanvasUrl}api/v1/users/self";

                // Describe how to map the user info we receive to user claims
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        // Get user info from the UserInformationEndpoint endpoint and use it to populate user claims
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        context.RunClaimActions(user.RootElement);
                    }
                };
            });
        }
    }
}
