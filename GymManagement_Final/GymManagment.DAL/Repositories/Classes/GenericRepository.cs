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
    // Repo ----> is Factory of Quiries (no logic)"gate of context"
    //Services----->bt2ol how to use Repo
    // I will do GenericRepository to common implementation
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity, new()
    {
        private GymDbContext dbContext;
        private readonly DbSet<TEntity> _set;
        public GenericRepository(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
            _set = dbContext.Set<TEntity>();
        }
        public void Add(TEntity Entity)
        {
            _set.Add(Entity);
        }

        //public async Task<int> CompleteAsync()
        //{
        //    return await dbContext.SaveChangesAsync();
        //}

        public void Delete(TEntity entity)
        {
           //var Item = _set.FirstOrDefault(x=>x.Id == id);// or use find =>search in first locally
           // if (Item != null)
           _set.Remove(entity);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            if (predicate is not null) query = query.Where(predicate);
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var Item =await _set.FirstOrDefaultAsync(x=>x.Id == id);
            return Item;
        }

        public void Update(TEntity Entity)
        {
            //Untracked from controller
            // get plan by Id
            //Equalize each snapshot with original coloumn (because I don't know where the update)
            // then this method make Track Object depend on iths PK
            _set.Update(Entity);
        }

        Task<bool> IGenericRepository<TEntity>.AnyAsync(Expression<Func<TEntity, bool>> predicte, CancellationToken ct=default)
        {
            return _set.AsNoTracking().AnyAsync(predicte, ct);
        }

        async Task<TEntity?> IGenericRepository<TEntity>.FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicte, bool tracking, CancellationToken ct)
        {
            IQueryable<TEntity> query =tracking ? _set :_set.AsNoTracking();
           return await query.FirstOrDefaultAsync(predicte);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null, CancellationToken token = default)
        {
            //if(condition==null)
            //    return await _set.AsNoTracking().CountAsync(token);
            //else
            //    return await _set.AsNoTracking().CountAsync(condition,token);
            //-->OR
            return condition==null ? await _set.AsNoTracking().CountAsync(token) : await _set.AsNoTracking().CountAsync(condition,token);
        }
    }
}
