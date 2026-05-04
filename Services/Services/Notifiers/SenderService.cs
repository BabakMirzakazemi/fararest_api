using Common.Configurations;
using Common.Markers;
using Common.Utilities.Extensions;
using Kavenegar;
using Kavenegar.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Services.Contracts.Notifiers;
using Services.DTOs.Mails;
using Services.DTOs.Notices;
using System.Net;
using System.Net.Mail;

namespace Services.Services.Notifiers;

public class SenderService : ISenderService, IScopedDependency
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ILogger<SenderService> _logger;
    private readonly SiteSettings _siteSettings;

    public SenderService(
        IWebHostEnvironment hostingEnvironment,
        ILogger<SenderService> logger,
        IOptionsSnapshot<SiteSettings> siteSettings)
    {
        _hostingEnvironment = hostingEnvironment;
        _logger = logger;
        _siteSettings = siteSettings.Value;
    }

    public Task<bool> SendOtpSmsAsync(string number, string text, CancellationToken cancellationToken)
    {
        var receptor = number.NormalizeMobile();
        //TODO : uncomment for production
        //KavenegarApi kavenegar = new KavenegarApi("6730347A667647645944354467515449484C4178726C6A53515530316A4436617A30472F7A7474335444413D");
        //SendResult res = kavenegar.VerifyLookup(receptor, "token20", text);
        if (_hostingEnvironment.IsDevelopment())
            return Task.FromResult(true);
        else
            return Task.FromResult(false);
    }

    public Task<bool> SendGeneralSmsAsync(string number, string text, CancellationToken cancellationToken)
    {
        var receptor = number.NormalizeMobile();
        KavenegarApi kavenegar = new KavenegarApi("6730347A667647645944354467515449484C4178726C6A53515530316A4436617A30472F7A7474335444413D");
        SendResult res = kavenegar.VerifyLookup(receptor, "token20", text);
        if (_hostingEnvironment.IsDevelopment())
            return Task.FromResult(true);
        else
            return Task.FromResult(false);
    }

    public Task<bool> SendTestSmsAsync(string number, string text, CancellationToken cancellationToken)
    {
        var receptor = number.NormalizeMobile();
        KavenegarApi kavenegar = new KavenegarApi("6730347A667647645944354467515449484C4178726C6A53515530316A4436617A30472F7A7474335444413D");
        SendResult res = kavenegar.VerifyLookup(receptor, "token20", text);
        if (_hostingEnvironment.IsDevelopment())
            return Task.FromResult(true);
        else
            return Task.FromResult(false);
    }

    public async Task<bool> SendEmailAsync(PostalServerMailRequest mailRequest, CancellationToken cancellationToken)
    {
        var mailSettings = _siteSettings.MailSettings;
        if (mailSettings == null || string.IsNullOrWhiteSpace(mailSettings.Host) || mailSettings.Port <= 0)
        {
            _logger.LogWarning("MailSettings is invalid. Host/Port is not configured.");
            return false;
        }

        try
        {
            using var message = BuildMailMessage(mailRequest, mailSettings);
            using var smtpClient = new SmtpClient(mailSettings.Host, mailSettings.Port)
            {
                EnableSsl = mailSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            if (!string.IsNullOrWhiteSpace(mailSettings.Mail))
            {
                smtpClient.Credentials = new NetworkCredential(mailSettings.Mail, mailSettings.Password);
            }

            await smtpClient.SendMailAsync(message, cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while sending email. Subject: {Subject}", mailRequest.Subject);
            return false;
        }
    }

    public Task<bool> SendEmailWithAttachmentAsync(SendEmailRequest dto, CancellationToken cancellationToken)
    {
        return SendEmailAsync(new PostalServerMailRequest
        {
            To = dto.Receivers,
            From = _siteSettings.MailSettings?.Mail ?? string.Empty,
            ReplyTo = _siteSettings.MailSettings?.Mail ?? string.Empty,
            Subject = dto.Subject,
            PlainBody = dto.Body,
            HtmlBody = dto.Body,
            Attachments = dto.AttachmentFiles
        }, cancellationToken);
    }

    private static MailMessage BuildMailMessage(PostalServerMailRequest mailRequest, MailSettings mailSettings)
    {
        var senderMail = string.IsNullOrWhiteSpace(mailRequest.From) ? mailSettings.Mail : mailRequest.From;
        var displayName = string.IsNullOrWhiteSpace(mailSettings.DisplayName) ? senderMail : mailSettings.DisplayName;
        if (string.IsNullOrWhiteSpace(senderMail))
            throw new InvalidOperationException("Sender mail is not configured.");

        var message = new MailMessage
        {
            From = new MailAddress(senderMail, displayName),
            Subject = mailRequest.Subject ?? string.Empty,
            Body = !string.IsNullOrWhiteSpace(mailRequest.HtmlBody) ? mailRequest.HtmlBody : mailRequest.PlainBody ?? string.Empty,
            IsBodyHtml = !string.IsNullOrWhiteSpace(mailRequest.HtmlBody)
        };

        foreach (var receiver in (mailRequest.To ?? []).Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            message.To.Add(receiver);
        }

        if (message.To.Count == 0)
            throw new InvalidOperationException("Receiver mail list is empty.");

        if (!string.IsNullOrWhiteSpace(mailRequest.ReplyTo))
        {
            message.ReplyToList.Add(new MailAddress(mailRequest.ReplyTo));
        }

        if (mailRequest.Attachments == null || mailRequest.Attachments.Count == 0)
            return message;

        foreach (var formFile in mailRequest.Attachments.Where(x => x is { Length: > 0 }))
        {
            var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            memoryStream.Position = 0;

            var attachment = new Attachment(memoryStream, formFile.FileName, formFile.ContentType);
            message.Attachments.Add(attachment);
        }

        return message;
    }
}
