using Microsoft.AspNetCore.Http;

namespace Services.DTOs.Notices;

public class SendEmailRequest
{
    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public List<string> Receivers { get; set; } = null!;

    public List<IFormFile> AttachmentFiles { get; set; } = [];
}
