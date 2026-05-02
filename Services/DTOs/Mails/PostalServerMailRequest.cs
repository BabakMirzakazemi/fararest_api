using Microsoft.AspNetCore.Http;

namespace Services.DTOs.Mails;

public class PostalServerMailRequest
{
    [JsonProperty("to")]
    public List<string> To { get; set; } = [];
    [JsonProperty("from")]
    public string From { get; set; } = string.Empty;
    [JsonProperty("subject")]
    public string Subject { get; set; } = string.Empty;
    [JsonProperty("reply_to")]
    public string ReplyTo { get; set; } = string.Empty;
    [JsonProperty("plain_body")]
    public string PlainBody { get; set; } = string.Empty;
    [JsonProperty("html_body")]
    public string HtmlBody { get; set; } = string.Empty;

    public List<IFormFile> Attachments { get; set; } = [];
}
