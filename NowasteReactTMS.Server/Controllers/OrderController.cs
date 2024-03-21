using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NowasteReactTMS.Server.Models;
using NowasteReactTMS.Server.Models.OrderDTOs;
using NowasteReactTMS.Server.Repositories.Interface;
using NowasteTms.Model;

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
        private readonly ITransportTypeRepository _transportTypeRepository;

        public OrderController(UserManager<ApplicationUser> userManager,
            IOrderRepository orderRepo,
            INotificationsRepository notificationsRepository,
            ISupplierRepository supplierRepository,
            ICustomerRepository customerRepository,
            IAgentRepository agentRepository,
            ITransportOrderRepository transportOrderRepository,
            IItemRepository itemRepository,
            IOrderLineRepository orderLineRepository,
            IPalletInventoryRepository inventoryRepository,
            ITransportTypeRepository transportTypeRepository)
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
            _transportTypeRepository = transportTypeRepository;
        }
        /// <summary>
        /// Returns order when searching for its PK
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{pk}")]
        public async Task<IActionResult> GetOrder(Guid pk)
        {
            var orders = await _orderRepo.GetOrder(pk);

            return Ok(orders);
        }
        /// <summary>
        /// Displays all active orders
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SearchOrders(SearchOrderDTO dto)
        {
            var searchParameters = new SearchParameters
            {
                Limit = dto.Size,
                Offset = dto.Page * dto.Size,
                Filters = dto.Filter,
                SortOrders = dto.Column
            };

            searchParameters.Filters.Add("historical", dto.Historical.ToString().ToLower());

            var orders = await _orderRepo.SearchOrders(searchParameters);
            var items = await _itemRepository.GetItems(); // TransportTemp/StorageTemp, itemID, Name och Company

            var responseDTO = new
            {
                Orders = orders.Orders,
                Items = items
            };

            return Ok(responseDTO);
        }

        /// <summary>
        /// Creates a new Order with the required parameters
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder(OrderDTO dto)
        {
            //var user = await _userManager.GetUserAsync(User);
            Dictionary<Guid, Item> items = new Dictionary<Guid, Item>();
            foreach (var orderLine in dto.Lines)
            {
                var item = await _itemRepository.GetItem(orderLine.ItemPK);
                items.Add(orderLine.ItemPK, item);
            }
            var pk = Guid.NewGuid();
            var lineNumber = 1;
            await _orderRepo.AddOrder(new Order
            {
                OrderPK = pk,
                OrderId = dto.OrderId,
                HandlerID = dto.HandlerID,
                Type = OrderType.PurchaseOrder,
                Origin = OrderOrigin.Portal,
                Status = OrderStatus.Imported,// spo.Status,
                CollectionDate = dto.CollectionDate,
                DeliveryDate = dto.DeliveryDate,
                SupplierPK = dto.SupplierPK,
                CustomerPK = dto.CustomerPK,
                Comment = dto.Comment,
                InternalComment = dto.InternalComment,
                UpdatedByUserId = "hej", //user.Id
                Email = "hej@email.com", //user.Email
                TransportBooking = dto.TransportBooking,
                PalletExchange = dto.PalletExchange,
                Lines = dto.Lines.Select(x => new OrderLine
                {
                    OrderLinePK = Guid.NewGuid(),
                    OrderPK = pk,
                    LineNumber = lineNumber++,
                    ItemPK = items[x.ItemPK].ItemPK,
                    ItemQty = x.ItemQty,
                    PalletTypeId = x.PalletTypeId,
                    PalletQty = x.PalletQty,
                    ItemName = items[x.ItemPK].Name
                })
            });
            return Ok(dto);
        }
        /// <summary>
        /// Updates an existing Order and makes sure the required parameters are valid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedOrder"></param>
        /// <returns></returns>
        [HttpPut("{pk}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] Order updatedOrder)
        {
            try
            {
                var existingOrder = await _orderRepo.Get(id);

                if (existingOrder == null)
                {
                    return NotFound();
                }

                existingOrder.OrderId = updatedOrder.OrderId;
                existingOrder.Type = updatedOrder.Type;
                existingOrder.Status = updatedOrder.Status;
                existingOrder.Origin = updatedOrder.Origin;
                existingOrder.CollectionDate = updatedOrder.CollectionDate;
                existingOrder.DeliveryDate = updatedOrder.DeliveryDate;
                existingOrder.PalletExchange = updatedOrder.PalletExchange;
                existingOrder.SupplierPK = updatedOrder.SupplierPK;
                existingOrder.CustomerPK = updatedOrder.CustomerPK;
                existingOrder.HandlerID = updatedOrder.HandlerID;
                existingOrder.Created = updatedOrder.Created;
                existingOrder.Updated = updatedOrder.Updated;
                existingOrder.Comment = updatedOrder.Comment;
                existingOrder.InterchangeReference = updatedOrder.InterchangeReference;
                existingOrder.InternalComment = updatedOrder.InternalComment;
                existingOrder.DivisionId = updatedOrder.DivisionId;
                existingOrder.UpdatedByUserId = updatedOrder.UpdatedByUserId;
                existingOrder.Email = updatedOrder.Email;
                existingOrder.TransportBooking = updatedOrder.TransportBooking;
                existingOrder.Lines = updatedOrder.Lines.ToList();
                existingOrder.TransportOrders = updatedOrder.TransportOrders.ToList(); 
                existingOrder.TransportOrder = updatedOrder.TransportOrder; 
                existingOrder.Customer = updatedOrder.Customer; 
                existingOrder.Supplier = updatedOrder.Supplier;

                // Update the order in the database and get the number of affected rows
                int affectedRows = await _orderRepo.UpdateOrder(existingOrder);

                if (affectedRows > 0)
                {
                    return Ok(existingOrder); 
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update order");
                }
            }
            catch
            {
                // Handle any exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        /// <summary>
        /// Deletes a existing order using its PK
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpDelete("{pk}")]
        public async Task<IActionResult> DeleteOrder(Guid pk)
        {
            await _orderRepo.GetOrder(pk);

            return Ok("Successufully deleted");
        }

        [HttpGet("Items")]
        public async Task<ActionResult<List<Item>>> GetAllItems()
        {
            
                var items = await _itemRepository.GetItems();
                return Ok(items);       
            
        }


    }

}
