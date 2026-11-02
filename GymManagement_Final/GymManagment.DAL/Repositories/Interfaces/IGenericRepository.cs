using GymManagment.DAL.Models;
using System.Linq.Expressions;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, bool isTracked = false, CancellationToken ct = default);

        // CancellationToken ct=default   ------>lw et25er 3lya el8y el token (automatic operate)
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        void Add(TEntity Entity); // sync --->local 
        void Update(TEntity Entity); // sync --->local 
        void Delete(TEntity entity); // sync --->local 
        //Task<int> CompleteAsync(); // Async ---> operate within DB I/O operation 
        Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicte , CancellationToken ct=default );
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicte,bool tracking =false , CancellationToken ct =default );
        Task<int> CountAsync(Expression<Func<TEntity , bool>>? condition = null , CancellationToken token = default);
    }

}

