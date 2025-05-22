using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class VehicleProfile: Profile
  {
    public VehicleProfile()
    {
      CreateMap<Vehicle, VehicleDTO>();
      CreateMap<VehicleDTO, Vehicle>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
      CreateMap<Model, ModelDTO>();
      CreateMap<ModelDTO, Model>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
