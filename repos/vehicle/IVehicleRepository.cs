using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
  public interface IVehicleRepository
  {
    Task<Vehicle?> FindAsyncById(string id);
    Task<Vehicle?> FindAsyncByQRCode(string qrCodeId);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
  }
}
