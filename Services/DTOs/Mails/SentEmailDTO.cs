using Common.Markers;
using Entities.Emails;
using Entities.Enums.Emails;

namespace Services.DTOs.Mails;

public class SentEmailDTO : IHaveCustomMapping
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Subject { get; set; } = null!;
    public int CountReceivers { get; set; }
    public int CountSucceeded { get; set; }
    public int CountPending { get; set; }
    public int CountFailed { get; set; }
    public int CountCancelled { get; set; }
    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<EmailSharedInformation, SentEmailDTO>()
               .ForMember(d => d.CountReceivers, opt => opt.MapFrom(s => s.Emails.Count))
               .ForMember(d => d.CountPending, opt => opt.MapFrom(s => s.Emails.Where(s => s.Status == EmailStatus.Pending).Count()))
               .ForMember(d => d.CountSucceeded, opt => opt.MapFrom(s => s.Emails.Where(s => s.Status == EmailStatus.Succeeded).Count()))
               .ForMember(d => d.CountFailed, opt => opt.MapFrom(s => s.Emails.Where(s => s.Status == EmailStatus.Failed).Count()))
               .ForMember(d => d.CountCancelled, opt => opt.MapFrom(s => s.Emails.Where(s => s.Status == EmailStatus.Cancelled).Count()));
    }
}
