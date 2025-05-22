using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
  public interface IYardVehicleRepository
  {
    Task<YardVehicle> CreateAsync(YardVehicle vehicle);
  }
}
