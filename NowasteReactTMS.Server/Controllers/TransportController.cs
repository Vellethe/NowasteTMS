using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using NuGet.Protocol.Plugins;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransportOrderRepository _transportOrderRepo;
        private readonly INotificationsRepository _notificationsRepo;
        private readonly ITransportOrderServiceRepository _transportOrderServiceRepo;
        private readonly ICurrencyRepository _currencyRepo;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IMemoryCache _memoryCache;
        public static string[] ColumnNames = [
            "OrderOrigin",
            "TransportOrderID",
            "TransportOrderPK",
            "Orderstatus",
            "OrderStatusString",
            "AgentName",
            "BusinessUnitPK",
            "TransportOrderLineCount",
            "PalletQty",
            "EurPalletQty",
            "Services",
            "Price",
            "TOCurrencyShortName",
            "FromCountry",
            "FromCity",
            "ToCountry",
            "ToCity",
            "VehicleRegistrationPlate",
            "DeliveryDate",
            "Email",
            "ToCustomerName",
            "FromSupplierName",
            "OrderIds",
            "InternalComment",
            "ConsolidatedParentPK",
            "IsConsolidation",
            "CollectionDate",
            "CollectionDateTo",
            "CollectionDateWeekDay",
            "EtaTo",
            "EtaWeekDay",
            "UpdateWeekDay",
            "UpdateTo",
            "Updated"
            ];

        public TransportController(UserManager<ApplicationUser> userManager,
            ITransportOrderRepository transportOrderRepo,
            INotificationsRepository notificationsRepo,
            ITransportOrderServiceRepository transportOrderServiceRepo,
            ICurrencyRepository currencyRepo,
            INotificationsRepository notificationsRepository,
            IAgentRepository agentRepository,
            IOrderRepository orderRepository,
            IOrderLineRepository orderLineRepository,
            IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _transportOrderRepo = transportOrderRepo;
            _notificationsRepo = notificationsRepo;
            _transportOrderServiceRepo = transportOrderServiceRepo;
            _currencyRepo = currencyRepo;
            _notificationsRepository = notificationsRepository;
            _agentRepository = agentRepository;
            _orderRepository = orderRepository;
            _orderLineRepository = orderLineRepository;
            _memoryCache = memoryCache;
        }

        [HttpGet("{pk}")]
        public async Task<IActionResult> GetTransportOrder(Guid pk)
        {
            var transportOrder = await _transportOrderRepo.Get(pk);
            if (transportOrder == null)
            {
                return NotFound();
            }

            return Ok(transportOrder);
        }

        [HttpDelete("{pk}")]
        public async Task<IActionResult> DeleteTransportOrder(Guid pk)
        {
            await _transportOrderRepo.Delete(pk);
            return Ok("Successfully deleted");
        }
    }
}
