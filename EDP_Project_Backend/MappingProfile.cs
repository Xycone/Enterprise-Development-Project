using AutoMapper;
using EDP_Project_Backend.Models;

namespace EDP_Project_Backend
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Tier, TierDTO>();
            CreateMap<User, UserDTO>();
            CreateMap<User, UserBasicDTO>();
            CreateMap<User, UserProfileDTO>();
            CreateMap<ActivityListing, ListingDTO>();
            CreateMap<Activity, ActivityDTO>();
        }
    }
}
