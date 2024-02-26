using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteTms.Model;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        // Keep names synchronized with OrderRepository.columnMapping (project nowaste.transport), as well in OrderController.SearchOrders().
        public static string[] ColumnNames = [
                "OrderPK",
                "DeliveryDate",
                "DeliveryDateTo",
                "DeliveryDateWD",
                "OrderId",
                "OrderTransportStatus",
                "OrderTransportStatusString",
                "Origin",
                "CollectionDate",
                "CollectionDateTo",
                "CollectionDateWD",
                "SupplierName",
                "CustomerName",
                "PalletExchange",
                "Created",
                "Updated",
                "UpdatedTo",
                "UpdatedWD",
                "InternalComment",
                "LineCount",
                "OrderLinesTypeId2",
                "OrderLinesTypeId8",
                "ItemID",
                "ItemName",
                "ItemCompany",
                "ItemQty",
                "TransportTemp",
                "CustomerAddress",
                "CustomerCountry",
                "SupplierCountry",
                "Email",
                "TransportBooking",
                "SupplierPK",
                "CustomerPK",
                "Status"
            ];
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepo;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly ITransportOrderRepository _transportOrderRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IPalletInventoryRepository _inventoryRepository;

        public OrderController(UserManager<ApplicationUser> userManager,
            IOrderRepository orderRepo,
            INotificationsRepository notificationsRepository,
            ISupplierRepository supplierRepository,
            ICustomerRepository customerRepository,
            IAgentRepository agentRepository,
            ITransportOrderRepository transportOrderRepository,
            IItemRepository itemRepository,
            IOrderLineRepository orderLineRepository,
            IPalletInventoryRepository inventoryRepository)
        {
            _userManager = userManager;
            _orderRepo = orderRepo;
            _notificationsRepository = notificationsRepository;
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
            _agentRepository = agentRepository;
            _transportOrderRepository = transportOrderRepository;
            _itemRepository = itemRepository;
            _inventoryRepository = inventoryRepository;
            _orderLineRepository = orderLineRepository;
        }
        [HttpGet]
        public async Task <IActionResult> GetOrder(Guid id)
        {
             var orders = await _orderRepo.GetOrder(id);

            return Ok(orders);
        }
    }
        
}
