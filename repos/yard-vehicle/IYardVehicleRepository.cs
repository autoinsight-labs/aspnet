using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
  public interface IYardVehicleRepository
  {
    Task<YardVehicle> CreateAsync(YardVehicle vehicle);
    Task<YardVehicle?> FindAsync(string id);
    Task<PagedResponse<YardVehicle>> ListPagedAsync(int page, int pageSize, Yard yard);
    Task UpdateAsync();
  }
}
