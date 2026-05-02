using Services.DTOs.Mails;
using Services.DTOs.Notices;

namespace Services.Contracts.Notifiers;

public interface ISenderService
{
    Task<bool> SendOtpSmsAsync(string number, string text, CancellationToken cancellationToken);

    Task<bool> SendGeneralSmsAsync(string number, string text, CancellationToken cancellationToken);

    Task<bool> SendTestSmsAsync(string number, string text, CancellationToken cancellationToken);

    Task<bool> SendEmailAsync(PostalServerMailRequest mailRequest, CancellationToken cancellationToken);

    Task<bool> SendEmailWithAttachmentAsync(SendEmailRequest dto, CancellationToken cancellationToken);
}