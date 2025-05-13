using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class YardEmployeeProfile: Profile
  {
    public YardEmployeeProfile()
    {
      CreateMap<YardEmployee, YardEmployeeDTO>();
      CreateMap<YardEmployeeDTO, YardEmployee>();
    }
  }
}
