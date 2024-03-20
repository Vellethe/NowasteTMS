using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NowastePalletPortal.Extensions.Helpers;
using NowasteReactTMS.Server;
using NowasteReactTMS.Server.Controllers;
using NowasteReactTMS.Server.Repositories;
using NowasteReactTMS.Server.Repositories.Interface;
using WMan.Data.ConnectionFactory;

using IConnectionFactory = WMan.Data.ConnectionFactory.IConnectionFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DB-CONTEXT
builder.Services.AddDbContext<NowastePalletPortalContext>(options =>
    builder.Configuration.GetConnectionString("NowastePalletPortalContext"));

builder.Services.AddDbContext<NowastePalletPortalContext>(options =>
    builder.Configuration.GetConnectionString("DefaultConnection"));

var connectionString = builder.Configuration.GetConnectionString("NowasteTMS");
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<NowastePalletPortalContext>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders()
                .AddSignInManager<NowasteSignInManager<ApplicationUser>>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") // Ange din React-apps URL här
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// All repositories
builder.Services.AddSingleton<IConnectionFactory>(new SqlConnectionFactory(connectionString));
builder.Services.AddSingleton<IPalletReceiptRepository, PalletReceiptRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IOrderLineRepository, OrderLineRepository>();
builder.Services.AddSingleton<ITransportOrderRepository, TransportOrderRepository>();
builder.Services.AddSingleton<ITransportOrderServiceRepository, TransportOrderServiceRepository>();
builder.Services.AddSingleton<ITransportOrderLineRepository, TransportOrderLineRepository>();
builder.Services.AddSingleton<ITransportZonePriceRepository, TransportZonePriceRepository>();
builder.Services.AddSingleton<ITransportZoneRepository, TransportZoneRepository>();
builder.Services.AddSingleton<ITransportZonePriceLineRepository, TransportZonePriceLineRepository>();
builder.Services.AddSingleton<ITransportTypeRepository, TransportTypeRepository>();
builder.Services.AddSingleton<IAgentRepository, AgentRepository>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<ISupplierRepository, SupplierRepository>();
builder.Services.AddSingleton<IContactInformationRepository, ContactInformationRepository>();
builder.Services.AddSingleton<IReferenceRepository, ReferenceRepository>();
builder.Services.AddSingleton<INotificationsRepository, NotificationsRepository>();
builder.Services.AddSingleton<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddSingleton<IPalletInventoryRepository, PalletInventoryRepository>();
builder.Services.AddSingleton<IBusinessUnitRepository, BusinessUnitRepository>();



var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseCors("MyAllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
