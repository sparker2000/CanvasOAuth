using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace CanvasOAuth
{
    /// <summary>
    /// <see cref="AuthenticationProperties"/> for a Canvas OAuth challenge.
    /// </summary>
    public class CanvasChallengeProperties : OAuthChallengeProperties
    {
        /// <summary>
        /// The parameter key for the "force_login" argument being used for a challenge request.
        /// </summary>
        public static readonly string ForceLoginKey = "force_login";

        /// <summary>
        /// The parameter key for the "prompt" argument being used for a challenge request.
        /// </summary>
        public static readonly string PromptParameterKey = "prompt";

        /// <summary>
        /// The parameter key for the "purpose" argument being used for a challenge request.
        /// </summary>
        public static readonly string PurposeKey = "purpose";

        /// <summary>
        /// The parameter key for the "unique_id" argument being used for a challenge request.
        /// </summary>
        public static readonly string UniqueIdKey = "unique_id";

        /// <summary>
        /// Initializes a new instance of <see cref="CanvasChallengeProperties"/>.
        /// </summary>
        public CanvasChallengeProperties()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CanvasChallengeProperties"/>.
        /// </summary>
        /// <inheritdoc />
        public CanvasChallengeProperties(IDictionary<string, string> items)
            : base(items)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CanvasChallengeProperties"/>.
        /// </summary>
        /// <inheritdoc />
        public CanvasChallengeProperties(IDictionary<string, string> items, IDictionary<string, object> parameters)
            : base(items, parameters)
        {
        }

        /// <summary>
        /// The "force_login" parameter value being used for a challenge request.
        /// </summary>
        public int? ForceLogin
        {
            get => GetParameter<int>(ForceLoginKey);
            set => SetParameter(ForceLoginKey, value);
        }

        /// <summary>
        /// The "purpose" parameter value being used for a challenge request.
        /// </summary>
        public string Purpose
        {
            get => GetParameter<string>(PurposeKey);
            set => SetParameter(PurposeKey, value);
        }

        /// <summary>
        /// The "unique_id" parameter value being used for a challenge request.
        /// </summary>
        public string UniqueId
        {
            get => GetParameter<string>(UniqueIdKey);
            set => SetParameter(UniqueIdKey, value);
        }

        /// <summary>
        /// The "prompt" parameter value being used for a challenge request.
        /// </summary>
        public string Prompt
        {
            get => GetParameter<string>(PromptParameterKey);
            set => SetParameter(PromptParameterKey, value);
        }
    }
}