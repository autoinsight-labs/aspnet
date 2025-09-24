using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.Repositories
{
    public class ModelRepository : IModelRepository
    {
      private readonly AutoInsightDb _db;

      public ModelRepository(AutoInsightDb db)
      {
        this._db = db;
      }

      public async Task<Model?> FindAsyncById(string id)
      {
        return await _db.Models.FirstOrDefaultAsync(m => m.Id == id);
      }

      public async Task<Model> CreateAsync(Model model)
      {
        _db.Models.Add(model);
        await _db.SaveChangesAsync();
        return model;
      }
    }
}