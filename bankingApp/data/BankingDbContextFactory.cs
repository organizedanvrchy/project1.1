using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace bankingApp.data;

public class BankingDbContextFactory : IDesignTimeDbContextFactory<BankingDbContext>
{
    public BankingDbContext CreateDbContext(string[] args)
    {
        string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        using JsonDocument configuration = JsonDocument.Parse(File.ReadAllText(appSettingsPath));

        var optionsBuilder = new DbContextOptionsBuilder<BankingDbContext>();
        string connectionString = configuration.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("BankingAppDb")
            .GetString()
            ?? throw new InvalidOperationException("Connection string 'BankingAppDb' not found.");

        optionsBuilder.UseSqlServer(connectionString);

        return new BankingDbContext(optionsBuilder.Options);
    }
}