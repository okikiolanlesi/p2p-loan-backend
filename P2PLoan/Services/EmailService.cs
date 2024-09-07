using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using P2PLoan.Interfaces;
using Azure;
using Azure.Communication.Email;
using System.Collections.Generic;

namespace P2PLoan.Services;

public class EmailService : IEmailService
{
    private readonly string _templatesFolderPath;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public EmailService(string templatesFolderPath, ILogger logger, IConfiguration configuration)
    {
        _templatesFolderPath = templatesFolderPath;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendHtmlEmailAsync(string toEmail, string subject, string templateFileName, object model)
    {
        var templateFileNameWithSuffix = templateFileName + ".html";

        var templatePath = Path.Combine(_templatesFolderPath, templateFileNameWithSuffix);

        System.Console.WriteLine(templatePath);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templateFileNameWithSuffix}");
        }

        var templateContent = await File.ReadAllTextAsync(templatePath);
        var mergedContent = MergeTemplateWithModel(templateContent, model);

        // var message = new MimeMessage();
        // message.From.Add(new MailboxAddress("PeerLend", _configuration["EmailSettings:smtpUser"]));
        // message.To.Add(new MailboxAddress("", toEmail));
        // message.Subject = subject;


        // var bodyBuilder = new BodyBuilder
        // {
        //     HtmlBody = mergedContent
        // };

        // message.Body = bodyBuilder.ToMessageBody();

        // Create the email content
        var emailContent = new EmailContent(subject)
        {
            PlainText = mergedContent,
            Html = mergedContent
        };

        // Create the To list
        var toRecipients = new List<EmailAddress>
        {
            new EmailAddress(toEmail),
        };

        // Create the CC list
        //         var ccRecipients = new List<EmailAddress>
        // {
        //   new EmailAddress("<ccemailalias@emaildomain.com>"),
        // };

        // Create the BCC list
        //         var bccRecipients = new List<EmailAddress>
        // {
        //   new EmailAddress("<bccemailalias@emaildomain.com>"),
        // };

        // EmailRecipients emailRecipients = new EmailRecipients(toRecipients, ccRecipients, bccRecipients);

        EmailRecipients emailRecipients = new EmailRecipients(toRecipients);

        // Create the EmailMessage
        var emailMessage = new EmailMessage(
            senderAddress: "DoNotReply@" + _configuration["EmailSettings:azureCommunicationDomainAddress"], // The email address of the domain registered with the Communication Services resource
            emailRecipients,
            emailContent);

        // Add optional ReplyTo address which is where any replies to the email will go to.
        emailMessage.ReplyTo.Add(new EmailAddress(_configuration["EmailSettings:adminEmail"]));

        var emailClient = new EmailClient(_configuration["EmailSettings:COMMUNICATION_SERVICES_CONNECTION_STRING"]);


        //     await client.ConnectAsync(_configuration["EmailSettings:smtpServer"], int.Parse(_configuration["EmailSettings:smtpPort"] ?? "465"), true);
        // await client.AuthenticateAsync(_configuration["EmailSettings:smtpUser"], _configuration["EmailSettings:smtpPassword"]);
        // await client.SendAsync(message);
        // await client.DisconnectAsync(true);

        EmailSendOperation emailSendOperation = emailClient.Send(WaitUntil.Completed, emailMessage);

        /// Get the OperationId so that it can be used for tracking the message for troubleshooting
        string operationId = emailSendOperation.Id;


        _logger.LogDebug($"{templateFileName} email sent to {toEmail} with operationId = {operationId}");
    }

    private string MergeTemplateWithModel(string templateContent, object model)
    {
        foreach (var property in model.GetType().GetProperties())
        {
            var placeholder = $"{{{{{property.Name}}}}}";
            var value = property.GetValue(model)?.ToString();
            templateContent = templateContent.Replace(placeholder, value);
        }

        return templateContent;
    }
}