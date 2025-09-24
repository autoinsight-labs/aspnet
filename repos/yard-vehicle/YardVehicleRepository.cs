using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.Repositories
{
    class YardVehicleRepository : IYardVehicleRepository
    {
      private readonly AutoInsightDb _db;

      public YardVehicleRepository(AutoInsightDb db)
      {
        this._db = db;
      }

      public async Task<YardVehicle> CreateAsync(YardVehicle vehicle)
      {
        _db.YardVehicles.Add(vehicle);
        await _db.SaveChangesAsync();

        return vehicle;
      }

      public async Task<YardVehicle?> FindAsync(string id)
      {
        return await _db.YardVehicles.Include(yv => yv.Vehicle).ThenInclude(v => v.Model).FirstOrDefaultAsync(ye => ye.Id == id);
      }

      public async Task<PagedResponse<YardVehicle>> ListPagedAsync(int page, int pageSize, Yard yard)
      {
        var totalRecords = await _db.YardVehicles.Where(yv => yv.YardId == yard.Id).CountAsync();

        var vehicles = await _db.YardVehicles
          .AsNoTracking()
          .Include(yv => yv.Vehicle)
          .ThenInclude(v => v.Model)
          .Where(yv => yv.YardId == yard.Id)
          .OrderBy(ye => ye.Id)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

        var pagedResponse = new PagedResponse<YardVehicle>(vehicles, page, pageSize, totalRecords);

        return pagedResponse;
      }

      public async Task UpdateAsync()
      {
        await _db.SaveChangesAsync();
      }
    }
}
