using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.Repositories
{
    public class YardRepository : IYardRepository
    {
      private readonly AutoInsightDb _db;

      public YardRepository(AutoInsightDb db)
      {
        this._db = db;
      }

      public async Task<Yard> CreateAsync(Yard yard)
      {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            Address address = yard.Address;
            _db.Addresses.Add(address);

            await _db.SaveChangesAsync();

            _db.Yards.Add(yard);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return yard;
        }
        catch (Exception error)
        {
            await transaction.RollbackAsync();

            Console.WriteLine(error);

            throw;
        }
      }

      public async Task DeleteAsync(Yard yard)
      {
        _db.Yards.Remove(yard);
        await _db.SaveChangesAsync();
      }

      public async Task<Yard?> FindAsync(string id)
      {
        return await _db.Yards.Include(y => y.Address)
                              .FirstOrDefaultAsync(y => y.Id == id);
      }

      public async Task<PagedResponse<Yard>> ListPagedAsync(int pageNumber, int pageSize)
      {
        var totalRecords = await _db.Yards.AsNoTracking().CountAsync();

        var yards = await _db.Yards.AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(y => y.Address)
            .ToListAsync();

        var pagedResponse = new PagedResponse<Yard>(yards, pageNumber, pageSize, totalRecords);

        return pagedResponse;
      }

      public async Task UpdateAsync()
      {
        await _db.SaveChangesAsync();
      }
    }
}
