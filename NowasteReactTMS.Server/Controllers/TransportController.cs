using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NowasteReactTMS.Server.Models;
using NowasteReactTMS.Server.Repositories.Interface;
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
        private readonly IEmailHandler _emailHandler;
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
            IEmailHandler emailHandler,
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
            _emailHandler = emailHandler;
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
        //            var orderLine = order.Lines.FirstOrDefault(ol => ol.OrderLinePK == line.OrderLinePK);
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
        //        }).ToList();

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
        /// Displays all information about a TransportOrder depending on what role inside the website you have
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        //public async Task<IActionResult> SearchTransportOrders(SearchOrderDTO dto)
        //{
        //    var searchParameters = new SearchParameters
        //    {
        //        Limit = dto.Size,
        //        Offset = dto.Page * dto.Size,
        //        Filters = dto.Filter,
        //        SortOrders = dto.Column
        //    };

        //    var user = await _userManager.GetUserAsync(User);

        //    List<TransportOrder> transportOrders = new List<TransportOrder>();

        //    if (User.IsInRole(Constants.ROLE_ADMIN) || User.IsInRole(Constants.ROLE_SUPERUSER) || User.IsInRole(Constants.ROLE_STANDARDUSER))
        //    {
        //        var response = await _transportOrderRepo.SearchOrders(searchParameters, null, dto.Historical);
        //        transportOrders.AddRange(response.TransportOrders);
        //    }
        //    else if (User.IsInRole(Constants.ROLE_AGENT) && user.BusinessUnitId != null)
        //    {
        //        var response = await _transportOrderRepo.SearchOrders(searchParameters, user.BusinessUnitId, dto.Historical);
        //        transportOrders.AddRange(response.TransportOrders);
        //    }

        //    return Ok(transportOrders);
        //}

        public async Task<IActionResult> Consolidate(TransportViewDTO dto)
        {
            var transportOrderPks = dto.CheckedTransportOrderPKs.Split(",");
            var transportOrders = transportOrderPks.Select(async x => await _transportOrderRepo.Get(new Guid(x))).Select(x => x.Result);
            var orders = transportOrders.SelectMany(x => x.OrderTransportOrders.Select(y => y.OrderPK)).Select(async x => await _orderRepository.GetOrder(x)).Select(x => x.Result);
            var allServices = await _transportOrderServiceRepo.GetAllTransportOrderServices(transportOrders.First().TransportOrderLines.First().AgentPK, true);
            var selectedServicePKs = transportOrders.SelectMany(x => x.TransportOrderLines)
                .SelectMany(x => x.TransportOrderLineTransportOrderServices).Select(x => x.TransportOrderServicePK);

            var commonAgentForAllOrderLines = await _agentRepository.GetAgent(transportOrders.First().TransportOrderLines.First().AgentPK.Value);

            var cdto = new ConsolidateDTO
            {
                TransportOrders = transportOrders.Select(x => ViewModelFactory.Create(x, x.OrderTransportOrders.Select(async y => await _orderRepository.GetOrder(y.OrderPK)).Select(y => y.Result).ToList(), allServices)).ToList(),
                Orders = orders.Select(ViewModelFactory.Create).ToList(),
                Services = allServices,
                TotalFullTruckPrice = transportOrders.Sum(x => x.Price),
                CurrencyPK = transportOrders.First().CurrencyPK,
                AgentEmailAddresses = _emailHandler.GetListOfAgentsEmailAddresses(commonAgentForAllOrderLines),
                AgentPK = commonAgentForAllOrderLines.AgentPK,
                ConsolidateServicesViewModel = new ConsolidateServiceDTO
                {
                    UnselectedServices = allServices.Where(x => x.isActive && !selectedServicePKs.Contains(x.TransportOrderServicePK)).Select(x => new TransportOrderServiceDTO
                    {
                        Id = x.CurrencyPK,
                        Name = x.Name,
                        Currency = x.Currency,
                        Price = x.Price
                    }).ToList(),
                    SelectedServices = allServices.Where(x => selectedServicePKs.Contains(x.TransportOrderServicePK)).Select(x => new TransportOrderServiceDTO
                    {
                        Id = x.TransportOrderServicePK,
                        Name = x.Name,
                        Currency = x.Currency,
                        Price = x.Price
                    }).ToList()
                }
            };

            cdto.TransportOrders = cdto.TransportOrders.OrderBy(p => p.TransportOrderId).ToList();

            return Ok(cdto);
        }

        //public async Task<IActionResult> CreateConsolidation(ConsolidateViewModel vm, EditViewModel em, int status)
        //{
        //    await _createConsolidation(vm);

        //    return RedirectToAction("AllTransportOrders");
        //}

        //public async Task<IActionResult> CreateAndBookConsolidation(ConsolidateViewModel vm)
        //{
        //    var consolidatedTransportOrder = await _createConsolidation(vm);

        //    // Send email.
        //    // Despite all the efforts, the field is not auto populated.. Lets fetch it like this.
        //    vm.SelectedAgentEmailAddresses = Request.Form["AgentEmailAddresses.selectedAgentEmailAddresses"].ToList();
        //    //var mailOperationSuccessfulResult = false;

        //    //TransportOrderViewModel to = vm.TransportOrders.First();
        //    var agent = await _agentRepository.GetAgent(vm.AgentPK);

        //    foreach (var address in vm.SelectedAgentEmailAddresses)
        //    {
        //        await SendTransportOrderCreatedMail(consolidatedTransportOrder.TransportOrderPK, address, agent.BusinessUnit.Name);
        //    }

        //    return RedirectToAction("AllTransportOrders");
        //}
    }
}
