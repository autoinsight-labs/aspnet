using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class YardProfile: Profile
  {
    public YardProfile()
    {
      CreateMap<Yard, YardDto>();
      CreateMap<YardDto, Yard>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
      CreateMap<CreateYardDto, Yard>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
