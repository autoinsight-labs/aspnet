using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
  public interface IYardEmployeeRepository
  {
    Task<PagedResponse<YardEmployee>> ListPagedAsync(int page, int pageSize, Yard yard);
  }
}
