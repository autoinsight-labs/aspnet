using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
    class YardVehicleRepository : IYardVehicleRepository
    {
      private readonly AutoInsightDB _db;

      public YardVehicleRepository(AutoInsightDB db)
      {
        this._db = db;
      }

      public async Task<YardVehicle> CreateAsync(YardVehicle vehicle)
      {
        _db.YardVehicles.Add(vehicle);
        await _db.SaveChangesAsync();

        return vehicle;
      }
    }
}
