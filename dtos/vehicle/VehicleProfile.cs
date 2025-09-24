using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class VehicleProfile: Profile
  {
    public VehicleProfile()
    {
      CreateMap<Vehicle, VehicleDto>();
      CreateMap<VehicleDto, Vehicle>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
      CreateMap<CreateVehicleDto, Vehicle>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
      CreateMap<Model, ModelDto>();
      CreateMap<ModelDto, Model>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
      CreateMap<CreateModelDto, Model>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
