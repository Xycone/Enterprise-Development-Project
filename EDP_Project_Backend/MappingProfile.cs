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
            // Maps the tierName from the tier object included tgt with the user
            CreateMap<User, UserProfileDTO>().ForMember(dest => dest.TierName, opt => opt.MapFrom(src => src.Tier.TierName)); ;
            CreateMap<Tier, TierDTO>();
            // Maps the tierName from the tier object included tgt with the perk 
            CreateMap<Perk, PerkDTO>().ForMember(dest => dest.TierName, opt => opt.MapFrom(src => src.Tier.TierName));
            // YES :)
            CreateMap<Voucher, VoucherDTO>()
            .ForMember(dest => dest.PercentageDiscount, opt => opt.MapFrom(src => src.Perk.PercentageDiscount))
            .ForMember(dest => dest.FixedDiscount, opt => opt.MapFrom(src => src.Perk.FixedDiscount))
            .ForMember(dest => dest.MinGroupSize, opt => opt.MapFrom(src => src.Perk.MinGroupSize))
            .ForMember(dest => dest.MinSpend, opt => opt.MapFrom(src => src.Perk.MinSpend))
            .ForMember(dest => dest.VoucherQuantity, opt => opt.MapFrom(src => src.Perk.VoucherQuantity))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<Order, OrderDTO>();
            CreateMap<ActivityListing, ListingDTO>();
            CreateMap<Activity, ActivityDTO>();
        }
    }
}
