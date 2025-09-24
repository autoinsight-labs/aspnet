using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class EmployeeInviteProfile : Profile
  {
    public EmployeeInviteProfile()
    {
      CreateMap<EmployeeInvite, EmployeeInviteDto>();
    }
  }
}
