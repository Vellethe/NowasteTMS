using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Repositories.Interface
{
    public interface IEmailHandler
    {
        List<SelectListItem> GetListOfAgentsEmailAddresses(Agent commonAgentForAllOrderLines);
        Task CreateAndSendTransportOrderEmail(byte[] file, TransportOrder transportOrder, string receivingAddress, string name, string url, string token);
        Task CreateAndSendTransportOrderChangedEmail(byte[] file, TransportOrder transportOrder, string receivingAddress, string name, string url, string token);
    }
}
