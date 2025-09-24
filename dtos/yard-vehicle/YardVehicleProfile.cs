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
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
      CreateMap<CreateYardVehicleDto, YardVehicle>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
