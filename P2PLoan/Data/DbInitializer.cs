using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P2PLoan.Interfaces;

namespace P2PLoan.Data;

public class DbInitializer
{
    public async static Task InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<P2PLoanDbContext>();
            await context.Database.MigrateAsync();

            var seederHandler = services.GetRequiredService<ISeederHandler>();
            await SeedData(seederHandler);
        }
        catch (Exception ex)
        {
            // Log the error (you could use a logging framework like Serilog or NLog)
            Console.WriteLine($"An error occurred while migrating or seeding the database: {ex.Message}");
        }
    }

    private async static Task SeedData(ISeederHandler seederHandler)
    {
        Console.WriteLine("made it here");
        await seederHandler.seed();
    }
}
