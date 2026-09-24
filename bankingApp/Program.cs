using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using bankingApp.data;
using bankingApp.repositories.users;
using bankingApp.repositories.admin;
using bankingApp.repositories.customer;
using bankingApp.repositories.account;
using bankingApp.repositories.transactions;
using bankingApp.repositories.chequebook;
using bankingApp.services.auth;
using bankingApp.services.customer;
using bankingApp.services.admin;
using bankingApp.ui;
using bankingApp.seeding;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string connectionString = configuration.GetConnectionString("BankingAppDb")
    ?? throw new InvalidOperationException("Connection string 'BankingAppDb' not found in appsettings.json.");

var services = new ServiceCollection();

services.AddDbContext<BankingDbContext>(options => options.UseSqlServer(connectionString));

services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<IAdminRepository, AdminRepository>();
services.AddScoped<IBankAccountRepository, BankAccountRepository>();
services.AddScoped<ITransactionRepository, TransactionRepository>();
services.AddScoped<IChequeBookRequestRepository, ChequeBookRequestRepository>();

services.AddScoped<IAuthService, AuthService>();
services.AddScoped<ICustomerService, CustomerService>();
services.AddScoped<IAdminService, AdminService>();

var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var seeder = new DatabaseSeeder(
        scope.ServiceProvider.GetRequiredService<ICustomerRepository>(),
        scope.ServiceProvider.GetRequiredService<IAdminRepository>(),
        scope.ServiceProvider.GetRequiredService<IBankAccountRepository>()
    );
    seeder.Seed();
}

var mainMenu = new MainMenu(provider);
mainMenu.Run();