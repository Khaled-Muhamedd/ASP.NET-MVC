using GymManagment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface ISessionReposaitory : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(
       Expression<Func<Session, bool>>? predicate = null,
       CancellationToken ct = default);
        Task<int> GetCountOfBookedSlotsAsync(int SessionId , CancellationToken ct = default);
        Task<Session?> GetSessionByIdWithTrainerAndCategory(int SessionId , CancellationToken ct = default);
    }
}
