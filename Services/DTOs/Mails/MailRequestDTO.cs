using Microsoft.AspNetCore.Http;

namespace Services.DTOs.Mails;

public class MailRequestDTO
{
    public string ToEmail { get; set; } = null!;
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public List<IFormFile>? Attachments { get; set; }
}
