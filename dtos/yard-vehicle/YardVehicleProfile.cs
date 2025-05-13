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
      CreateMap<YardEmployeeDTO, YardVehicle>();
    }
  }
}
