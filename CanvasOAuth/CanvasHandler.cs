﻿using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CanvasOAuth
{
    /// <summary>
    /// Authentication handler for Canvas's OAuth based authentication.
    /// </summary>
    public class CanvasHandler : OAuthHandler<CanvasOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CanvasHandler"/>.
        /// </summary>
        /// <inheritdoc />
        public CanvasHandler(IOptionsMonitor<CanvasOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        /// <inheritdoc />
        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            // Get the Canvas user
            var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occurred when retrieving Canvas user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync(Context.RequestAborted)))
            {
                var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
                context.RunClaimActions();
                await Events.CreatingTicket(context);
                return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
            }
        }

        /// <inheritdoc />
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            // Canvas Identity Platform Manual:
            // https://canvas.instructure.com/doc/api/file.oauth.html

            var queryStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            /*  Required Parameters  */
            queryStrings.Add("client_id", Options.ClientId);
            queryStrings.Add("response_type", "code");
            queryStrings.Add("redirect_uri", redirectUri);

            /*   Optional Parameters   */
            AddQueryString(queryStrings, properties, CanvasChallengeProperties.ScopeKey, FormatScope, Options.Scope);
            AddQueryString(queryStrings, properties, CanvasChallengeProperties.PurposeKey);
            AddQueryString(queryStrings, properties, CanvasChallengeProperties.ForceLoginKey);
            AddQueryString(queryStrings, properties, CanvasChallengeProperties.UniqueIdKey);
            AddQueryString(queryStrings, properties, CanvasChallengeProperties.PromptParameterKey);

            var state = Options.StateDataFormat.Protect(properties);
            queryStrings.Add("state", state);

            var authorizationEndpoint = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, queryStrings!);
            return authorizationEndpoint;
        }

        private static void AddQueryString<T>(
            IDictionary<string, string> queryStrings,
            AuthenticationProperties properties,
            string name,
            Func<T, string> formatter,
            T defaultValue)
        {
            string value;
            var parameterValue = properties.GetParameter<T>(name);
            if (parameterValue != null)
            {
                value = formatter(parameterValue);
            }
            else if (!properties.Items.TryGetValue(name, out value))
            {
                value = formatter(defaultValue);
            }

            // Remove the parameter from AuthenticationProperties so it won't be serialized into the state
            properties.Items.Remove(name);

            if (value != null)
            {
                queryStrings[name] = value;
            }
        }

        private static void AddQueryString(
            IDictionary<string, string> queryStrings,
            AuthenticationProperties properties,
            string name,
            string defaultValue = null)
            => AddQueryString(queryStrings, properties, name, x => x, defaultValue);
    }
}
