using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class YardVehicleProfile: Profile
  {
    public YardVehicleProfile()
    {
      CreateMap<YardVehicle, YardVehicleDto>();
      CreateMap<YardVehicleDto, YardVehicle>()
       .ForMember(dest => dest.Id, opt => opt.Ignore())
       .ForMember(dest => dest.Vehicle, opt => opt.Ignore())
       .ForMember(dest => dest.VehicleId, opt => opt.Ignore())
       .ForMember(dest => dest.Yard, opt => opt.Ignore())
       .ForMember(dest => dest.YardId, opt => opt.Ignore())
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
      CreateMap<CreateYardVehicleDto, YardVehicle>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
