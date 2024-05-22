using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using multipleEmailProvider.Models;
using multipleEmailProvider.Service;

namespace multipleEmailProvider.Functions
{
    public class EmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailService _emailService;

        public EmailSender(ILogger<EmailSender> logger, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [Function(nameof(EmailSender))]
        public async Task Run(
            [ServiceBusTrigger("email_request", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            try
            {
                var emailRequest = _emailService.UnPackEmailRequest(message);
                if (emailRequest != null && emailRequest.To != null && emailRequest.To.Count > 0)
                {
                    if (_emailService.SendEmail(emailRequest))
                    {
                        await messageActions.CompleteMessageAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: EmailSender.Run :: {ex.Message}");
            }
        }
    }
}