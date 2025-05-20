using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
  public interface IYardEmployeeRepository
  {
    Task<PagedResponse<YardEmployee>> ListPagedAsync(int page, int pageSize, Yard yard);
    Task<YardEmployee> CreateAsync(YardEmployee employee);
    Task<YardEmployee?> FindAsync(string id);
    Task DeleteAsync(YardEmployee yardEmployee);
    Task UpdateAsync();
  }
}
