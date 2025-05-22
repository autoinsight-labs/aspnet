using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class YardVehicleProfile: Profile
  {
    public YardVehicleProfile()
    {
      CreateMap<YardVehicle, YardVehicleDTO>();
      CreateMap<YardVehicleDTO, YardVehicle>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
