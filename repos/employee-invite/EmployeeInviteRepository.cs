using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.Repositories
{
    public class EmployeeInviteRepository : IEmployeeInviteRepository
    {
        private readonly AutoInsightDb _db;

        public EmployeeInviteRepository(AutoInsightDb db)
        {
            this._db = db;
        }

        public async Task<EmployeeInvite> CreateAsync(EmployeeInvite invite)
        {
            _db.EmployeeInvites.Add(invite);
            await _db.SaveChangesAsync();
            return invite;
        }

        public async Task<EmployeeInvite?> FindByTokenAsync(string token)
        {
            return await _db.EmployeeInvites
                .Include(ei => ei.Yard)
                .FirstOrDefaultAsync(ei => ei.Token == token);
        }

        public async Task<EmployeeInvite?> FindByIdAsync(string id)
        {
            return await _db.EmployeeInvites
                .Include(ei => ei.Yard)
                .FirstOrDefaultAsync(ei => ei.Id == id);
        }

        public async Task<PagedResponse<EmployeeInvite>> ListByYardAsync(int page, int pageSize, string yardId)
        {
            var totalRecords = await _db.EmployeeInvites
                .Where(ei => ei.YardId == yardId)
                .CountAsync();

            var invites = await _db.EmployeeInvites
                .AsNoTracking()
                .Where(ei => ei.YardId == yardId)
                .OrderByDescending(ei => ei.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<EmployeeInvite>(invites, page, pageSize, totalRecords);
        }

        public async Task<PagedResponse<EmployeeInvite>> ListByUserAsync(int page, int pageSize, string userId)
        {
            var totalRecords = await _db.EmployeeInvites
                .Where(ei => ei.AcceptedByUserId == userId)
                .CountAsync();

            var invites = await _db.EmployeeInvites
                .AsNoTracking()
                .Include(ei => ei.Yard)
                .Where(ei => ei.AcceptedByUserId == userId)
                .OrderByDescending(ei => ei.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<EmployeeInvite>(invites, page, pageSize, totalRecords);
        }

        public async Task<PagedResponse<EmployeeInvite>> ListByEmailAsync(int page, int pageSize, string email)
        {
            var baseQuery = GetPendingInvitesByEmailQuery(email);
            
            var totalRecords = await baseQuery.CountAsync();

            var invites = await baseQuery
                .AsNoTracking()
                .Include(ei => ei.Yard)
                .OrderByDescending(ei => ei.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<EmployeeInvite>(invites, page, pageSize, totalRecords);
        }

        private IQueryable<EmployeeInvite> GetPendingInvitesByEmailQuery(string email)
        {
            return _db.EmployeeInvites
                .Where(ei => ei.Email == email && ei.Status == InviteStatus.PENDING);
        }

        public async Task<EmployeeInvite?> FindByEmailAndYardAsync(string email, string yardId)
        {
            return await GetPendingInvitesByEmailQuery(email)
                .FirstOrDefaultAsync(ei => ei.YardId == yardId);
        }

        public async Task UpdateAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(EmployeeInvite invite)
        {
            _db.EmployeeInvites.Remove(invite);
            await _db.SaveChangesAsync();
        }
    }
}
