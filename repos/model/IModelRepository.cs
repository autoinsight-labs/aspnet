using AutoInsightAPI.Models;

namespace AutoInsightAPI.Repositories
{
  public interface IModelRepository
  {
    Task<Model?> FindAsyncById(string id);
    Task<Model> CreateAsync(Model model);
  }
}
