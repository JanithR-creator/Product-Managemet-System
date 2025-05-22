using AdapterService.Services.AdapterService;
using AdapterService.Services.AdapterService.AdapterServiceImpl;
using AdapterService.Services.FactoryService;
using AdapterService.Services.FactoryService.FactoryServiceImpl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddScoped<IProductAdapter, ABCAdapter>();
//builder.Services.AddHttpClient<IProductAdapter, ABCAdapter>();
builder.Services.AddHttpClient<ABCAdapter>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });

//builder.Services.AddHttpClient<IProductAdapter, ABCAdapter>().ConfigurePrimaryHttpMessageHandler(() =>
//{
//    return new HttpClientHandler
//    {
//        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//    };
//});

builder.Services.Scan(scan => scan
    .FromAssemblyOf<IProductAdapter>()
    .AddClasses(c => c.AssignableTo<IProductAdapter>())
    .As<IProductAdapter>()
    .WithScopedLifetime());

builder.Services.AddScoped<IProductAdapterFactoryService, ProductAdapterFactoryService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
