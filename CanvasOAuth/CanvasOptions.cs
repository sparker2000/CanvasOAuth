using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace CanvasOAuth
{
    public class CanvasOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="CanvasOptions"/>.
        /// </summary>
        public CanvasOptions()
        {
            CallbackPath = new PathString("/signin-canvas");
            Scope.Add("url:GET|/api/v1/users/:id");

            // Describe how to map the user info we receive to user claims
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            ClaimActions.MapJsonKey(ClaimTypes.Uri, "avatar_url");
        }
    }
}