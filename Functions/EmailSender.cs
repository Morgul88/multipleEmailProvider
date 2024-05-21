using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using multipleEmailProvider.Models;
using multipleEmailProvider.Service;
using Newtonsoft.Json;

namespace multipleEmailProvider.Functions;

public class EmailSender(ILogger<EmailSender> logger, EmailService emailService)
{
    private readonly ILogger<EmailSender> _logger = logger;
    
    private readonly EmailService _emailService = emailService;

    [Function(nameof(EmailSender))]
    public async Task Run(
        [ServiceBusTrigger("email_request", Connection = "ServiceBusConnection")]ServiceBusReceivedMessage message,ServiceBusMessageActions messageActions)
    {
        try
        {
            var emailRequest  = _emailService.UnPackEmailRequest(message);
            if(emailRequest != null && !string.IsNullOrEmpty(emailRequest.To)) 
            {
                if(_emailService.SendEmail(emailRequest))
                {
                    await messageActions.CompleteMessageAsync(message);
                }
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR :  EmailSender.Run :: {ex.Message} ");
        }
    }

   
}
