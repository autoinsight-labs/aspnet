using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.Repositories
{
    class YardEmployeeRepository : IYardEmployeeRepository
    {
      private readonly AutoInsightDb _db;

      public YardEmployeeRepository(AutoInsightDb db)
      {
        this._db = db;
      }

      public async Task<YardEmployee> CreateAsync(YardEmployee employee)
      {
          _db.YardEmployees.Add(employee);
          await _db.SaveChangesAsync();

          return employee;
      }

      public async Task DeleteAsync(YardEmployee yardEmployee)
      {
        _db.YardEmployees.Remove(yardEmployee);
        await _db.SaveChangesAsync();
      }

      public async Task<YardEmployee?> FindAsync(string id)
      {
        return await _db.YardEmployees.FirstOrDefaultAsync(ye => ye.Id == id);
      }

      public async Task<PagedResponse<YardEmployee>> ListPagedAsync(int page, int pageSize, Yard yard)
      {
        var totalRecords = await _db.YardEmployees.Where(ye => ye.YardId == yard.Id).CountAsync();

        var employees = await _db.YardEmployees
          .AsNoTracking()
          .Where(ye => ye.YardId == yard.Id)
          .OrderBy(ye => ye.Id)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

        var pagedResponse = new PagedResponse<YardEmployee>(employees, page, pageSize, totalRecords);

        return pagedResponse;
      }

      public async Task UpdateAsync()
      {
        await _db.SaveChangesAsync();
      }
    }
}
