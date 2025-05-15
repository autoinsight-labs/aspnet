using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
  public interface IYardRepository
  {
    Task<Yard?> FindAsync(string id);
    Task<Yard> CreateAsync(Yard yard);
    Task<PagedResponse<Yard>> ListPagedAsync(int pageNumber, int pageSize);
    Task UpdateAsync();
    Task DeleteAsync(Yard yard);
  }
}
