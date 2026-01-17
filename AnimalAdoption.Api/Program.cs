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
builder.Services.AddDbContext<UserDbContext>(options => options.UseInMemoryDatabase("UserDb"));
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<JwtTokenService>();

var jwtSettingsSection = builder.Configuration.GetSection("Authentication:JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection("Authentication:JwtSettings")
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings or secret is not configured properly.");
            
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.Secret)),
            ValidIssuer = jwtSettings.Issuer,
            ValidateIssuer = true,
            ValidateAudience = false,
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
            new Animal { Id = 1, Name = "Buddy", Type = AnimalType.Dog, Birthdate = new DateOnly(2021, 12, 25), ArrivalDate = new DateOnly(2025, 01, 12), Breed = "Golden Retriever", Description = "Friendly and energetic.", IsAdopted = false },
            new Animal { Id = 2, Name = "Mittens", Type = AnimalType.Cat, Birthdate = new DateOnly(2025, 01, 05), ArrivalDate = new DateOnly(2025, 05, 11), Breed = "Tabby", Description = "Loves to cuddle.", IsAdopted = false },
            new Animal { Id = 3, Name = "Charlie", Type = AnimalType.Dog, Birthdate = new DateOnly(2023, 06, 28), ArrivalDate = new DateOnly(2025, 08, 10), Breed = "Beagle", Description = "Curious and playful.", IsAdopted = true },
            new Animal { Id = 4, Name = "Luna", Type = AnimalType.Cat, Birthdate = new DateOnly(2018, 09, 14), ArrivalDate = new DateOnly(2024, 10, 30), Breed = "Siamese", Description = "Very vocal and affectionate.", IsAdopted = false }
        );
        context.SaveChanges();
    }
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    context.Database.EnsureCreated();   

    if (!context.Users.Any())
    {
        var seedUsers = app.Configuration.GetSection("SeedUsers").Get<List<SeedUser>>() ?? new List<SeedUser>();
        foreach (var seedUser in seedUsers)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = seedUser.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedUser.Password)
            };
            context.Users.Add(user);
        }
        context.SaveChanges();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
