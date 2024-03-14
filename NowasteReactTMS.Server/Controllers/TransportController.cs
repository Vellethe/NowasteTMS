using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NowasteReactTMS.Server.Models;
using NowasteTms.Model;
using NuGet.Protocol.Plugins;
using System.Globalization;

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

        /// <summary>
        /// Updates the OrderLines inside the TransportOrder
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        //[HttpPut]
        //public async Task<IActionResult> Edit(Guid pk)
        //{
        //    var transportOrder = await _transportOrderRepo.Get(pk);
        //    var orders = new List<Order>();
        //    foreach (var orderLink in transportOrder.OrderTransportOrders)
        //    {
        //        orders.Add(await _orderRepository.GetOrder(orderLink.OrderPK));
        //    }

        //    var toLine = transportOrder.TransportOrderLines.FirstOrDefault();
        //    var services = new List<TransportOrderService>();
        //    var agentEmailAdresses = new List<string>();
        //    if (toLine != null)
        //    {
        //        var agent = await _agentRepository.GetAgent(toLine.Agent.AgentPK);
        //        services = await _transportOrderServiceRepo.GetAllTransportOrderServices(agent.AgentPK, true);
        //        agentEmailAdresses = _emailHandler.GetListOfAgentsEmailAddresses(agent).Select(item => item.Value).ToList();
        //    }

        //    foreach (var order in orders)
        //    {
        //        var updatedOrderLines = order.Lines.Select(line =>
        //        {
        //            var orderLine = updatedOrderLines.FirstOrDefault(ol => ol.OrderLinePK == line.OrderLinePK);
        //            if (orderLine != null)
        //            {
        //                line.LineNumber = orderLine.LineNumber;
        //                line.PalletTypeId = orderLine.PalletTypeId;
        //                line.PalletTypeName = orderLine.PalletTypeName;
        //                line.PalletQty = orderLine.PalletQty;
        //                line.Item = orderLine.Item;
        //                line.ItemQty = orderLine.ItemQty;
        //                line.ItemName = orderLine.ItemName;
        //                line.TotalNetPrice = orderLine.TotalNetPrice;
        //            }
        //            return line;
        //        });

        //        await _orderRepository.UpdateOrder(updatedOrderLines);
        //    }

        //    return Ok(new { message = "Edit successful" });
        //}


        /// <summary>
        /// Display one TransportOrders details by searching on their PK
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Deletes one TransportOrder
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpDelete("{pk}")]
        public async Task<IActionResult> DeleteTransportOrder(Guid pk)
        {
            await _transportOrderRepo.Delete(pk);
            return Ok("Successfully deleted");
        }

        /// <summary>
        /// Displays all information about a TransportOrder depending on what role inside the webpage you have
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> SearchTransportOrders(SearchOrderDTO dto)
        {
            var searchParameters = new SearchParameters
            {
                Limit = dto.Size,
                Offset = dto.Page * dto.Size,
                Filters = dto.Filter,
                SortOrders = dto.Column
            };

            var user = await _userManager.GetUserAsync(User);

            List<TransportOrder> transportOrders = new List<TransportOrder>();

            if (User.IsInRole(Constants.ROLE_ADMIN) || User.IsInRole(Constants.ROLE_SUPERUSER) || User.IsInRole(Constants.ROLE_STANDARDUSER))
            {
                var response = await _transportOrderRepo.SearchOrders(searchParameters, null, dto.Historical);
                transportOrders.AddRange(response.TransportOrders);
            }
            else if (User.IsInRole(Constants.ROLE_AGENT) && user.BusinessUnitId != null)
            {
                var response = await _transportOrderRepo.SearchOrders(searchParameters, user.BusinessUnitId, dto.Historical);
                transportOrders.AddRange(response.TransportOrders);
            }

            return Ok(transportOrders);
        }

    }
}
