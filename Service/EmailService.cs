using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using multipleEmailProvider.Functions;
using multipleEmailProvider.Models;
using Newtonsoft.Json;

namespace multipleEmailProvider.Service;

public class EmailService(EmailClient emailClient, ILogger<EmailService> logger)
{
    private readonly EmailClient _emailClient = emailClient;
    private readonly ILogger<EmailService> _logger = logger;

    public EmailRequest UnPackEmailRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var emailRequest = JsonConvert.DeserializeObject<EmailRequest>(message.Body.ToString());
            if (emailRequest != null)
            {
                return emailRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR :  EmailSender.UnPackEmailRequest :: {ex.Message} ");
        }
        return null!;
    }

    public bool SendEmail(EmailRequest emailRequest)
    {
        try
        {
            foreach (var recipient in emailRequest.To)
            {
                var result = _emailClient.Send(WaitUntil.Completed,
                    senderAddress: Environment.GetEnvironmentVariable("SenderAdress"),
                    recipientAddress: recipient,
                    subject: emailRequest.Subject,
                    htmlContent: emailRequest.HtmlBody,
                    plainTextContent: emailRequest.PlainText);

                if (!result.HasCompleted)
                {
                    // Om en sändningsförfrågan misslyckas, logga och returnera false
                    _logger.LogError($"Failed to send email to {recipient}");
                    return false;
                }
            }

            // Om alla e-postmeddelanden skickas framgångsrikt, returnera true
            return true;
        }
        catch (Exception ex)
        {
            // Vid eventuella undantag, logga och returnera false
            _logger.LogError($"ERROR: EmailSender.SendEmailAsync :: {ex.Message}");
            return false;
        }
    }

}