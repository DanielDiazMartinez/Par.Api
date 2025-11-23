using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;  
using Par.Api.Data;    
using Par.Api.Services;
using Par.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Database=MyCustomParDB;Username=myadmin;Password=mypass123;SSL Mode=Disable";  

builder.Services.AddDbContext<ParContext>(options =>
    options.UseNpgsql(connectionString));  

builder.Services.AddScoped<BoxService>();

var app = builder.Build();

// Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ParContext>();
        context.Database.Migrate();
        
      
        Console.WriteLine("Do you want to run the data seeder? (y/n): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        
        if (response == "s" || response == "si" || response == "y" || response == "yes")
        {
            if (!await context.Boxes.AnyAsync())
            {
                Console.WriteLine("Running seeder...");
                await SeedData.InitializeAsync(services);
                Console.WriteLine("Seeder executed successfully.");
            }
            else
            {
                Console.WriteLine("The database already contains data. Seeder will not be executed.");
            }
        }
        else
        {
            Console.WriteLine("Seeder skipped.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error occurred while initializing the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints
app.MapBoxEndpoints();

app.Run();