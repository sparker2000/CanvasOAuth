# CanvasOAuth

This GitHub repo contains extension methods to add Canvas LMS OAuth to your application.

## NuGet Package

A NuGet package was published to easily add this library to your projects.  

You will find this package [here](https://www.nuget.org/packages/CanvasOAuth/).

# Canvas LMS external login setup in ASP.NET Core

This tutorial shows you how to enable users to sign in with their Canvas account using an ASP.NET Core MVC application with Identity.

## Create the Canvas OAuth 2.0 Client ID and secret

* Follow guidance in [How do I add a developer API key for an account](https://community.canvaslms.com/t5/Admin-Guide/How-do-I-add-a-developer-API-key-for-an-account/ta-p/259) (Canvas documentation).
* Once the **Developer Key** is created, the **Client ID** and **Secret** can be accessed in the **Details** column.

## Store the Canvas client ID and secret

Add the [`CanvasOAuth`](https://www.nuget.org/packages/CanvasOAuth/) NuGet package to the app.

Store sensitive settings such as the Canvas client ID and secret values with [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows). For this sample, use the following steps:

1. Initialize the project for secret storage per the instructions at [Enable secret storage](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage).
1. Store the sensitive settings in the local secret store with the secret keys `Authentication:Canvas:ClientId`, `Authentication:Canvas:ClientSecret`, `Authentication:Canvas:CanvasUrl`:

    ```dotnetcli
    dotnet user-secrets set "Authentication:Canvas:ClientId" "<client-id>"
    dotnet user-secrets set "Authentication:Canvas:ClientSecret" "<client-secret>"
    dotnet user-secrets set "Authentication:Canvas:CanvasUrl" "<canvas-url>"
    ```

In environment variables, a `:` separator may not work on all platforms.

## Configure Canvas authentication

Add the Authentication service to the `Program`:

```csharp
builder.Services
    .AddAuthentication()
    .AddCanvas(o =>
    {
        o.ClientId = builder.Configuration["Canvas:ClientId"];
        o.ClientSecret = builder.Configuration["Canvas:ClientSecret"];

        o.AuthorizationEndpoint = $"{builder.Configuration["Canvas:CanvasUrl"]}login/oauth2/auth";
        o.TokenEndpoint = $"{builder.Configuration["Canvas:CanvasUrl"]}login/oauth2/token";
        o.UserInformationEndpoint = $"{builder.Configuration["Canvas:CanvasUrl"]}api/v1/users/self";
    });
```

## Sign in with Canvas

* Run the app and select **Log in**. An option to sign in with Canvas appears.
* Select the **Canvas** button, which redirects to Canvas for authentication.
* After entering your Canvas credentials, you are redirected back to the web site.

## Change the default callback URI

The URI segment `/signin-canvas` is set as the default callback of the Canvas authentication provider.

## Troubleshooting

* If the sign-in doesn't work and you aren't getting any errors, switch to development mode to make the issue easier to debug.
* If Identity isn't configured by calling `services.AddIdentity` in `ConfigureServices`, attempting to authenticate results in *ArgumentException: The 'SignInScheme' option must be provided*. The project template used in this tutorial ensures Identity is configured.
* If the site database has not been created by applying the initial migration, you get *A database operation failed while processing the request* error. Select **Apply Migrations** to create the database, and refresh the page to continue past the error.
