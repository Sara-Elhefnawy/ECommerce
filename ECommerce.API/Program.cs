using ECommerce.Infrastructure;
using ECommerce.APP;

var builder = WebApplication.CreateBuilder(args);

// can chain cuz method return IServiceCollection
builder.Services.AddPresentation()
                .AddInfrastructure(builder.Configuration)
                .AddApp();

var app = builder.Build();

app.Run();
