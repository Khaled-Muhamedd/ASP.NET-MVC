using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Classes;

namespace GymManagment.DAL.Repositories.Interfaces
{
    // will make it when i have multiple transction "to avoid partial save"
    public interface IUnitOfWork
    {
        public IMembershipRepository MembershipRepository { get; }
        public ISessionReposaitory SessionRepository { get; }
        public IBookingRepository BookingRepository { get; }
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
        Task<int> SaveChangesAsync(CancellationToken ct=default);
        

    }
}
