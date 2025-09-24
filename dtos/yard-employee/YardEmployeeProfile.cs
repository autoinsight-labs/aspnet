using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class YardEmployeeProfile: Profile
  {
    public YardEmployeeProfile()
    {
      CreateMap<YardEmployee, YardEmployeeDto>();
      CreateMap<YardEmployeeDto, YardEmployee>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
      CreateMap<CreateYardEmployeeDto, YardEmployee>()
       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
  }
}
