using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
  public interface IEmployeeInviteRepository
  {
    Task<EmployeeInvite> CreateAsync(EmployeeInvite invite);
    Task<EmployeeInvite?> FindByTokenAsync(string token);
    Task<EmployeeInvite?> FindByIdAsync(string id);
    Task<PagedResponse<EmployeeInvite>> ListByYardAsync(int page, int pageSize, string yardId);
    Task<PagedResponse<EmployeeInvite>> ListByUserAsync(int page, int pageSize, string userId);
    Task<PagedResponse<EmployeeInvite>> ListByEmailAsync(int page, int pageSize, string email);
    Task<EmployeeInvite?> FindByEmailAndYardAsync(string email, string yardId);
    Task UpdateAsync();
    Task DeleteAsync(EmployeeInvite invite);
  }
}
