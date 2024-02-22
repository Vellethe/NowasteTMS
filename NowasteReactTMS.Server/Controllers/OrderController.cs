using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Controllers
{
    [Area("Transpot")]
    public class OrderController : Controller
    {
        // Keep names synchronized with OrderRepository.columnMapping (project nowaste.transport), as well in OrderController.SearchOrders().
        public static string[] ColumnNames = new string[] {
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
            };
        private readonly IPalletReceiptRepository palletReceiptRepository;
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
        public async Task<IActionResult> AllOrders()
        {
            var allOrders = await GetAllOrdersAsync();

            return Ok(allOrders);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SingleOrderViewModel spo)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var lineNumber = 1;

                // Mapping SingleOrderViewModel to OrderViewModel
                var order = new OrderViewModel
                {
                    OrderPK = spo.OrderPK,
                    OrderId = spo.OrderId,
                    HandlerID = spo.HandlerID,
                    Type = OrderType.PurchaseOrder,
                    Origin = OrderOrigin.Portal,
                    Status = OrderStatus.Imported, // spo.Status,
                    CollectionDate = spo.CollectionDate,
                    DeliveryDate = spo.DeliveryDate,
                    SupplierPK = spo.SupplierPK,
                    CustomerPK = spo.CustomerPK,
                    Comment = spo.Comment,
                    InternalComment = spo.InternalComment,
                    UpdatedByUserId = user.Id,
                    Email = user.Email,
                    TransportBooking = spo.TransportBooking,
                    Lines = spo.Lines.Select(x => new OrderLineViewModel
                    {
                        OrderLinePK = Guid.NewGuid(),
                        OrderPK = spo.OrderPK,
                        LineNumber = lineNumber++,
                        ItemPK = x.Item?.ItemPK,
                        ItemQty = x.ItemQty,
                        PalletTypeId = x.PalletTypeId,
                        PalletQty = x.PalletQty,
                        ItemName = x.Item?.Name
                    }).ToList()
                };

                // Calling the backend service method to add the order
                var result = await _orderService.AddOrderAsync(order);

                // Checking the result and returning appropriate response
                if (result.Success)
                {
                    return Ok(new { message = "Order created successfully" });
                }
                else
                {
                    return BadRequest(new { message = $"Failed to create order: {result.ErrorMessage}" });
                }
            }
            catch (Exception ex)
            {
                // Return error message
                return BadRequest(new { message = $"Failed to create order: {ex.Message}" });
            }
        }
    }
}
