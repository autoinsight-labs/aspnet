using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoMapper;

namespace AutoInsightAPI.Profiles
{
  public class YardProfile: Profile
  {
    public YardProfile()
    {
      CreateMap<Yard, YardDTO>();
      CreateMap<YardDTO, Yard>();
    }
  }
}
