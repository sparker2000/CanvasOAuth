﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CanvasOAuth
{
    public static class CanvasExtensions
    {
        /// <summary>
        /// Adds Canvas OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
        /// The default scheme is specified by <see cref="CanvasDefaults.AuthenticationScheme"/>.
        /// <para>
        /// Canvas authentication allows application users to sign in with their Canvas account.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddCanvas(this AuthenticationBuilder builder)
            => builder.AddCanvas(CanvasDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Adds Canvas OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
        /// The default scheme is specified by <see cref="CanvasDefaults.AuthenticationScheme"/>.
        /// <para>
        /// Canvas authentication allows application users to sign in with their Canvas account.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="CanvasOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddCanvas(this AuthenticationBuilder builder, Action<CanvasOptions> configureOptions)
            => builder.AddCanvas(CanvasDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Adds Canvas OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
        /// The default scheme is specified by <see cref="CanvasDefaults.AuthenticationScheme"/>.
        /// <para>
        /// Canvas authentication allows application users to sign in with their Canvas account.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="CanvasOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddCanvas(this AuthenticationBuilder builder, string authenticationScheme, Action<CanvasOptions> configureOptions)
            => builder.AddCanvas(authenticationScheme, CanvasDefaults.DisplayName, configureOptions);

        /// <summary>
        /// Adds Canvas OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
        /// The default scheme is specified by <see cref="CanvasDefaults.AuthenticationScheme"/>.
        /// <para>
        /// Canvas authentication allows application users to sign in with their Canvas account.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">A display name for the authentication handler.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="CanvasOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddCanvas(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<CanvasOptions> configureOptions)
            => builder.AddOAuth<CanvasOptions, CanvasHandler>(authenticationScheme, displayName, configureOptions);
    }
}
