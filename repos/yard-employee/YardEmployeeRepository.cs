using Microsoft.EntityFrameworkCore;
using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
    class YardEmployeeRepository : IYardEmployeeRepository
    {
      private readonly AutoInsightDB _db;

      public YardEmployeeRepository(AutoInsightDB db)
      {
        this._db = db;
      }

      public async Task<PagedResponse<YardEmployee>> ListPagedAsync(int page, int pageSize, Yard yard)
      {
        var totalRecords = _db.YardEmployees.Where(ye => ye.YardId == yard.Id).Count();

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
    }
}
