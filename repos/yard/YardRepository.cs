using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.Repositories
{
    public class YardRepository : IYardRepository
    {
      private readonly AutoInsightDB _db;

      public YardRepository(AutoInsightDB db)
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

            throw error;
        }
      }

      public async Task<Yard?> FindAsync(string id)
      {
        return await _db.Yards.Include(y => y.Address)
                              .FirstOrDefaultAsync(y => y.Id == id);
      }
    }
}
