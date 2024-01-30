using Depository.Extensions.DependencyInjection;
using EasyCraft.Daemon.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new DepositoryServiceProviderFactory());

builder.AddControllerServices();

var app = builder.Build();

app.UseControllerApp();
app.Run();