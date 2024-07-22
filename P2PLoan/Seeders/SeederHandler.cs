using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace P2PLoan.Seeders;

public class SeederHandler : ISeederHandler
{
    private readonly ISeedRepository seedRepository;
    private readonly IServiceProvider serviceProvider;

    public SeederHandler(ISeedRepository seedRepository, IServiceProvider serviceProvider)
    {
        this.seedRepository = seedRepository;
        this.serviceProvider = serviceProvider;
    }

    public async Task seed()
    {
        // Get the namespace to search in
        string targetNamespace = "P2PLoan.Seeders";

        // Get the interface type
        Type interfaceType = typeof(ISeeder);

        // Get all types in the assembly
        var seederTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t =>
            {
                return t.Namespace == targetNamespace && interfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract;
            }).ToList();

        // Sort the types based on the order number in the class name
        seederTypes.Sort((x, y) => ExtractOrderNumber(x.Name).CompareTo(ExtractOrderNumber(y.Name)));

        var alreadyExecutedSeeds = await seedRepository.GetAll();

        Dictionary<string, Seed> executedSeedsDictionary = alreadyExecutedSeeds.ToDictionary(e => e.Name);

        try
        {
            foreach (var seederType in seederTypes)
            {
                if (executedSeedsDictionary.TryGetValue(seederType.Name, out Seed executedSeed) && executedSeed != null)
                {
                    // Seed already executed, skip
                    continue;
                }

                // Create an instance of the class using the service provider
                var instance = serviceProvider.GetService(seederType) as ISeeder;

                // Call the method
                await instance?.up();

                seedRepository.Add(new Seed { Name = seederType.Name, Description = instance.Description() });

                await seedRepository.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {

            // Log the exception
            Console.WriteLine($"Error occurred while seeding: {ex.Message}");
            // You may want to log detailed exception information
        }

    }
    
    // Helper method to extract the order number from the class name
    static int ExtractOrderNumber(string className)
    {
        var parts = className.Split('_');
        if (parts.Length > 1 && int.TryParse(parts[1], out int orderNumber))
        {
            return orderNumber;
        }
        throw new InvalidOperationException($"Class name {className} does not contain a valid order number.");
    }
}
