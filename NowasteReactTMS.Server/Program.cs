using Microsoft.Extensions.Configuration;
using WMan.Data.ConnectionFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("NowasteTMS");
// All repositories
builder.Services.AddSingleton<IConnectionFactory>(new SqlConnectionFactory(connectionString));
builder.Services.AddSingleton<IPalletReceiptRepository, PalletReceiptRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IOrderLineRepository, OrderLineRepository>();
builder.Services.AddSingleton<ITransportOrderRepository, TransportOrderRepository>();
builder.Services.AddSingleton<ITransportOrderServiceRepository, TransportOrderServiceRepository>();
builder.Services.AddSingleton<ITransportZonePriceRepository, TransportZonePriceRepository>();
builder.Services.AddSingleton<ITransportZoneRepository, TransportZoneRepository>();
builder.Services.AddSingleton<IAgentRepository, AgentRepository>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<ISupplierRepository, SupplierRepository>();
builder.Services.AddSingleton<IContactInformationRepository, ContactInformationRepository>();
builder.Services.AddSingleton<IReferenceRepository, ReferenceRepository>();
builder.Services.AddSingleton<INotificationsRepository, NotificationsRepository>();
builder.Services.AddSingleton<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddSingleton<IPalletInventoryRepository, PalletInventoryRepository>();



var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
