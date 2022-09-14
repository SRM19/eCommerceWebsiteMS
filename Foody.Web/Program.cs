using Foody.Web.Services;
using Foody.Web.Services.IServices;
using Foody.Web.Utils;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<IProductService, ProductService>();

//*Add product service to dependency injection
builder.Services.AddScoped<IProductService, ProductService>();

//*Initialize service URL
Constants.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];

builder.Services.AddControllersWithViews();

//specify authentication as openidconnect and configure openidconnect(add required nuget packages)

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookies", c => c.ExpireTimeSpan = TimeSpan.FromMinutes(10))
.AddOpenIdConnect("oidc", options =>
 {
     options.Authority = builder.Configuration["ServiceUrls:IdentityAPI"];
     options.GetClaimsFromUserInfoEndpoint = true;
     options.ClientId = builder.Configuration["Auth:ClientId"];
     options.ClientSecret = builder.Configuration["Auth:ClientSecret"];
     options.ResponseType = builder.Configuration["Auth:ResponseType"];
     options.TokenValidationParameters.NameClaimType = builder.Configuration["Auth:NameClaimType"];
     options.TokenValidationParameters.RoleClaimType = builder.Configuration["Auth:RoleClaimType"];
     options.Scope.Add(builder.Configuration["Auth:Scope"]);
     options.ClaimActions.MapJsonKey("role", "role", "role");
     options.ClaimActions.MapJsonKey("sub", "sub", "sub");
     options.SaveTokens = true;
 });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//add authentication here
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
