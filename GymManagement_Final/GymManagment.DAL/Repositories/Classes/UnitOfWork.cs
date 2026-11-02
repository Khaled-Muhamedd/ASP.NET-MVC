using GymManagment.DAL.DbContexts;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext gymDbContext;
        private readonly Dictionary<string, object> _repositories = [];
        public IMembershipRepository MembershipRepository { get; }
        public ISessionReposaitory SessionRepository { get; }
        public IBookingRepository BookingRepository { get; }
        public UnitOfWork(GymDbContext gymDbContext,
            ISessionReposaitory sessionReposaitory,
            IBookingRepository bookingRepository,
             IMembershipRepository membershipRepository)
        {
            this.gymDbContext = gymDbContext;
            MembershipRepository = membershipRepository;
            SessionRepository = sessionReposaitory;
            BookingRepository = bookingRepository;
        }

        



        // problem--> kol ma a3mel object mn el unitofwork----->lazem a3mel object mn el "SessionRepo" we de mo4kela 3lasjan momken akon fe makan tany 8er el session w ana m4 m7tgaha

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            //TEntity == ????//plan ? , trainer? . member?
            var typeName = typeof(TEntity).Name;
            if(_repositories.TryGetValue(typeName, out object? value))
                return (IGenericRepository<TEntity>)value;
            else
            {
                var repo = new GenericRepository<TEntity>(gymDbContext);
                _repositories[typeName]= repo;
                return repo;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            =>await gymDbContext.SaveChangesAsync(ct);
        
    }
}
