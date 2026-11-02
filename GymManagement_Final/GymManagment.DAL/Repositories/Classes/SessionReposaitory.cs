using GymManagment.DAL.DbContexts;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes
{
    public class SessionReposaitory : GenericRepository<Session>, ISessionReposaitory

    {
        private readonly GymDbContext gymDbContext;

        public SessionReposaitory(GymDbContext gymDbContext):base(gymDbContext)
        {
            this.gymDbContext = gymDbContext;
        }
        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(Expression<Func<Session, bool>>? predicate = null, CancellationToken ct = default)
        {
            IQueryable<Session> query = gymDbContext.sessions
                .AsNoTracking()
                .Include(s => s.trainer)
                .Include(s => s.category);

            if (predicate is not null) query = query.Where(predicate);

            return await query.ToListAsync(ct);
        }

        public async Task<int> GetCountOfBookedSlotsAsync(int SessionId, CancellationToken ct)
        {
            return await gymDbContext.bookings.AsNoTracking().CountAsync(b=>b.SessionId==SessionId);
        }

        public async Task<Session?> GetSessionByIdWithTrainerAndCategory(int SessionId, CancellationToken ct = default)
        {
            return await gymDbContext.sessions.AsNoTracking().Include(x => x.trainer).Include(x => x.category).FirstOrDefaultAsync(x=>x.Id==SessionId);
        }
    }
}
