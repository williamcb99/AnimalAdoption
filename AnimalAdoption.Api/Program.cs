using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AnimalAdoptionDbContext>(options => options.UseInMemoryDatabase("AnimalAdoptionDb"));
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api/scalar");
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

app.UseAuthorization();

app.MapControllers();

app.Run();
