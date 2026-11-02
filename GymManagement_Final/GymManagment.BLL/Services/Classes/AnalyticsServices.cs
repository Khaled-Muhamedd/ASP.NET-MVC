using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.AnalyticsViewModels;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes
{
    public class AnalyticsServices : IAnalyticsServices
    {
        private readonly IUnitOfWork unitOfWork;

        public AnalyticsServices(IUnitOfWork unitOfWork) 
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default)
        {
            var now=DateTime.Now;
            // sessions startrtrDate lsa mga4
            var upcomingSessions= await unitOfWork.GetRepository<Session>().CountAsync(x=>x.StartDate> now);
            var ongoingSessions =await unitOfWork.GetRepository<Session>().CountAsync(x => x.StartDate <= now && x.EndDate >= now);
            var completedSessions =await unitOfWork.GetRepository<Session>().CountAsync(x => x.EndDate < now);
            var totalMembers = await unitOfWork.GetRepository<Member>().CountAsync();///----
            var totalTrainers = await unitOfWork.GetRepository<Trainer>().CountAsync();///----
            var acativeMembers = await unitOfWork.GetRepository<Membership>().CountAsync(m=>m.EndDate >now , ct);

            return new AnalyticsViewModel()
            {
                TotalMembers = totalMembers,
                CompletedSessions = completedSessions,
                TotalTrainers = totalTrainers,
                 OngoingSessions = ongoingSessions,
                 UpcomingSessions = upcomingSessions,
                 ActiveMembers =acativeMembers
            };

        }
    }
}
