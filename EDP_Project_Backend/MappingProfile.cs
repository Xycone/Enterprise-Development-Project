using AutoMapper;
using EDP_Project_Backend.Models;

namespace EDP_Project_Backend
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<User, UserBasicDTO>();
            CreateMap<User, UserProfileDTO>();
            CreateMap<Tier, TierDTO>();
            CreateMap<Perk, PerkDTO>();
        }
    }
}
