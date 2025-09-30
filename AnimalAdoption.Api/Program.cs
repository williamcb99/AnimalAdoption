using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AnimalAdoptionDbContext>(options => options.UseInMemoryDatabase("AnimalAdoptionDb"));
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<JwtTokenService>();

var jwtSettingsSection = builder.Configuration.GetSection("Authentication:JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Authentication:JwtSettings").Get<JwtSettings>();
        var whitelistSection = builder.Configuration.GetSection("Authentication:Whitelist");
        var allowedAudiences = whitelistSection.GetChildren().Select(x => x.Key).ToList();
        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
        {
            throw new InvalidOperationException("JWT settings or secret is not configured properly.");
        }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.Secret)),
            ValidIssuer = jwtSettings.Issuer,
            ValidAudiences = allowedAudiences,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api/scalar", options =>
    {
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = new[] { JwtBearerDefaults.AuthenticationScheme }
        };
    });
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AnimalAdoptionDbContext>();
    context.Database.EnsureCreated();

    if (!context.Animals.Any())
    {
        context.Animals.AddRange(
            new Animal { Id = 1, Name = "Buddy", Type = AnimalType.Dog, Age = 3, Breed = "Golden Retriever", Description = "Friendly and energetic.", IsAdopted = false },
            new Animal { Id = 2, Name = "Mittens", Type = AnimalType.Cat, Age = 2, Breed = "Tabby", Description = "Loves to cuddle.", IsAdopted = false },
            new Animal { Id = 3, Name = "Charlie", Type = AnimalType.Dog, Age = 4, Breed = "Beagle", Description = "Curious and playful.", IsAdopted = true },
            new Animal { Id = 4, Name = "Luna", Type = AnimalType.Cat, Age = 1, Breed = "Siamese", Description = "Very vocal and affectionate.", IsAdopted = false }
        );
        context.SaveChanges();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
