using log4net;
using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteReactTMS.Server.Repositories.Interface;
using NowasteTms.Model;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;

namespace NowasteReactTMS.Server.Repositories
{
    public class EmailHandler : IEmailHandler
    {
        private readonly string _transportAdviceContentPath = @"\mail\TransportAdviceMailTemplate.html";
        private readonly string _transportAdviceChangedContentPath = @"\mail\TransportAdviceChangedMailTemplate.html";
        private readonly string _imagePath = @"\mail\everfresh logo.jpg";

        private readonly IWebHostEnvironment _hostingEnvironment;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public EmailHandler(IWebHostEnvironment hostingEnvironment, IOrderRepository orderRepo, IAgentRepository agentRepo, ITransportOrderRepository transportOrderRepo, ITransportOrderServiceRepository serviceRepo)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private string PopulateEmailBody(string contentPath, string userName, string title, string url, string description, string transportId)
        {
            string body;
            using (var reader = new StreamReader(_hostingEnvironment.WebRootPath + _transportAdviceContentPath))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{UserName}", userName);
            body = body.Replace("{Title}", title);
            body = body.Replace("{Url}", url);
            body = body.Replace("{Description}", description);
            body = body.Replace("{TransportId}", transportId);

            return body;
        }

        public async Task CreateHtmlFormattedEmail(string recipientEmail, string subject, string body, byte[] attachmentFile)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(recipientEmail));
                mailMessage.From = new MailAddress("transport@nowaste.se");

                //logo
                var filepath = _hostingEnvironment.WebRootPath + _imagePath;
                var reader = File.ReadAllBytes(filepath);
                var img = new MemoryStream(reader);
                var av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

                var headerImage = new LinkedResource(img, MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "everfresh",
                    ContentType = new ContentType("image/jpg")
                };

                av.LinkedResources.Add(headerImage);
                mailMessage.AlternateViews.Add(av);

                //transport advice
                Stream ms = new MemoryStream(attachmentFile);

                /*
                    // Create an PDF locally (for debugging) 
                    CopyStream(ms);
                */

                var transportAdviceAtt = new Attachment(ms, "TransportAdvice.pdf", "application/pdf");
                mailMessage.Attachments.Add(transportAdviceAtt);

                await SendEmail(mailMessage);
            }
        }

        /// <summary>
        /// Create a local saved pdf
        /// </summary>
        /// <param name="file"></param>
        private void CopyStream(Stream file)
        {
            using (var fileStream = new FileStream(@"C:\Temp\test.pdf", FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }
        }

        protected virtual async Task SendEmail(MailMessage mail)
        {
            Logger.Info($"Trying to send email to {mail.To} with subject \"{mail.Subject}\".");
#if !DEBUG
            var smtp = new SmtpClient("smtp.everfresh.int");
#else
            // Nice to have if you install Papercut smtp server app. https://github.com/ChangemakerStudios/Papercut
            var smtp = new SmtpClient("localhost");
#endif

            try
            {
                await smtp.SendMailAsync(mail);
                Logger.Info("Email successfully sent.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        public List<SelectListItem> GetListOfAgentsEmailAddresses(Agent commonAgentForAllOrderLines)
        {
            var emailAddresses = new List<SelectListItem>();
            foreach (var c in commonAgentForAllOrderLines.BusinessUnit.ContactInformations.OrderByDescending(x => x.IsDefault))
            {
                if (null != c.Email)
                {
                    if (emailAddresses.All(item => item.Value != c.Email))
                    {
                        emailAddresses.Add(new SelectListItem { Text = c.Email, Value = c.Email });
                    }
                }

                foreach (var r in c.References)
                {
                    if (null == r.Email) continue;
                    if (emailAddresses.All(item => item.Value != r.Email))
                    {
                        emailAddresses.Add(new SelectListItem { Text = r.Email, Value = r.Email });
                    }
                }
            }

            return emailAddresses;
        }

        public async Task CreateAndSendTransportOrderEmail(byte[] file, TransportOrder transportOrder, string receivingAddress, string name, string url, string token)
        {
            if (!string.IsNullOrEmpty(receivingAddress))
            {
                var body = PopulateEmailBody(
                    _transportAdviceContentPath,
                    name,
                    "Title",
                    url,
                    "Description",
                    transportOrder.TransportOrderID);

                await CreateHtmlFormattedEmail(receivingAddress, "Everfresh - Transport order", body, file);
            }
        }

        public async Task CreateAndSendTransportOrderChangedEmail(byte[] file, TransportOrder transportOrder, string receivingAddress, string name, string url, string token)
        {
            if (!string.IsNullOrEmpty(receivingAddress))
            {
                var body = PopulateEmailBody(
                    _transportAdviceChangedContentPath,
                    name,
                    "Title",
                    url,
                    "Description",
                    transportOrder.TransportOrderID);

                await CreateHtmlFormattedEmail(receivingAddress, "Everfresh - Transport order", body, file);
            }
        }
    }
}
