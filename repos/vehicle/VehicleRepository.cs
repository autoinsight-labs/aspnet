using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
      private readonly AutoInsightDb _db;

      public VehicleRepository(AutoInsightDb db)
      {
        this._db = db;
      }

      public async Task<Vehicle?> FindAsyncById(string id)
      {
        return await _db.Vehicles.Include(y => y.Model)
                              .FirstOrDefaultAsync(y => y.Id == id);
      }

      public async Task<Vehicle?> FindAsyncByQRCode(string qrCodeId)
      {
        var qrCode = await _db.QrCodes.Include(y => y.Vehicle).ThenInclude(v => v!.Model)
                              .FirstOrDefaultAsync(y => y.Id == qrCodeId);

        return qrCode?.Vehicle;
      }

      public async Task<Vehicle> CreateAsync(Vehicle vehicle)
      {
        _db.Vehicles.Add(vehicle);
        await _db.SaveChangesAsync();
        return vehicle;
      }
    }
}
