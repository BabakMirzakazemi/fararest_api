namespace Services.DTOs.Mails;

public class EmailReceiverWithIdDTO : PagingRequest
{
    public int EmailId { get; set; }
}
