using CanvasOAuth;
using ExampleMVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication()
    .AddCanvas(o =>
    {
        o.ClientId = builder.Configuration["Authentication:Canvas:ClientId"];
        o.ClientSecret = builder.Configuration["Authentication:Canvas:ClientSecret"];

        o.AuthorizationEndpoint = $"{builder.Configuration["Authentication:Canvas:CanvasUrl"]}login/oauth2/auth";
        o.TokenEndpoint = $"{builder.Configuration["Authentication:Canvas:CanvasUrl"]}login/oauth2/token";
        o.UserInformationEndpoint = $"{builder.Configuration["Authentication:Canvas:CanvasUrl"]}api/v1/users/self";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
