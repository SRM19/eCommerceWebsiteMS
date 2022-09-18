using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//https://localhost:44375/ -> Identity Server URL
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:44375/";
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false
        };

    });
builder.Services.AddOcelot();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

await app.UseOcelot();

app.Run();
