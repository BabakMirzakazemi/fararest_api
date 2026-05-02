using Common.Markers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTOs.Users
{
    public class UserListDTO : IHaveCustomMapping
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<User, UserListDTO>()
                 .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email));
        }
    }
}
