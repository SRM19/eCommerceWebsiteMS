using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Services;
using Foody.Services.Identity.DbContexts;
using Foody.Services.Identity.Initializer;
using Foody.Services.Identity.Models;
using Foody.Services.Identity.Services;
using Foody.Services.Identity.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Transient);

//Make use of Aspnet identity and add user and role
builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Add Identity Server
//Add configured resource, api scope and client to identity server
//Make identity server to use identity that we created: ApplicationUser
//Add developer signing credential for dev purpose: generate key and add
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(Constants.IdentityResources)
.AddInMemoryApiScopes(Constants.ApiScopes)
.AddInMemoryClients(Constants.Clients)
.AddAspNetIdentity<ApplicationUser>()
.AddDeveloperSigningCredential();

builder.Services.AddScoped<IDbInitialize, DbInitialize>();
//profile service
builder.Services.AddScoped<IProfileService, ProfileService>();

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

//add identity serveer to request pipeline
app.UseIdentityServer();

app.UseAuthorization();

SeedDatabase();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using(var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitialize>();
        dbInitializer.Initialize();
    }
}