using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Mynt.Data;
using Mynt.Endpoints;
using Mynt.Services;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Enable XML comments
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below. Do not include Bearer at the beginning of your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtAudience = builder.Configuration["Jwt:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback to environment variable for Docker support
        connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
    }
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string not found"));
});

builder.Services.AddScoped<IUserActivityService, UserActivityService>();
builder.Services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();

// Add background services
builder.Services.AddHostedService<ExchangeRateUpdateService>();

// Add localization services
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        "en",
        "es",
        "fr",
        "de"
        // Add more cultures as needed
    };

    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS
app.UseCors("AllowAll");

// Debug middleware to log Authorization header
// app.Use(async (context, next) =>
// {
//     var authHeader = context.Request.Headers.Authorization.ToString();
//     Console.WriteLine($"Authorization header: '{authHeader}'");
//     Console.WriteLine($"Authorization header length: {authHeader.Length}");
//     await next();
// });

app.UseAuthentication();
app.UseAuthorization();

// Register endpoints
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapUserActivityEndpoints();
app.MapAssetTypeEndpoints();
app.MapAssetEndpoints();
app.MapAssetValueEndpoints();
app.MapCurrencyEndpoints();

// Development-only endpoint to generate JWT key
// if (app.Environment.IsDevelopment())
// {
//     app.MapGet("/api/dev/generate-jwt-key", () =>
//     {
//         using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
//         var keyBytes = new byte[32]; // 256-bit key
//         rng.GetBytes(keyBytes);
//         var base64Key = Convert.ToBase64String(keyBytes);

//         return Results.Ok(new
//         {
//             JwtKey = base64Key,
//             ConfigExample = new
//             {
//                 Jwt = new
//                 {
//                     Key = base64Key,
//                     Issuer = "Mynt",
//                     Audience = "Mynt"
//                 }
//             }
//         });
//     });
// }

// Add localization middleware
app.UseRequestLocalization();

app.Run();

