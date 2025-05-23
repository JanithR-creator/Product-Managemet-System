using AdapterService.Services.AdapterService;
using AdapterService.Services.AdapterService.AdapterServiceImpl;
using AdapterService.Services.FactoryService;
using AdapterService.Services.FactoryService.FactoryServiceImpl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Registers Adapter as the implementation of IProductAdapter, and injects an HttpClient into it automatically.
builder.Services.AddHttpClient<IProductAdapter, SchoolItemAdapter>();

//Registers factory service, so when .Factory("abc") is called, it can return the correct adapter (like ABCAdapter) using the dictionary set up in your factory.
builder.Services.AddScoped<IProductFactoryService, ProductFactoryService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
