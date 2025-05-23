using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
      private readonly AutoInsightDB _db;

      public VehicleRepository(AutoInsightDB db)
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
        var qrCode = await _db.QRCodes.Include(y => y.Vehicle).ThenInclude(v => v!.Model)
                              .FirstOrDefaultAsync(y => y.Id == qrCodeId);

        return qrCode?.Vehicle;
      }
    }
}
