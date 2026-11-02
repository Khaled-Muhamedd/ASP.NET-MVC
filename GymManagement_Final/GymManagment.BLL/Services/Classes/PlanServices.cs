using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.PlanViewModels;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Classes;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes
{
    public class PlanServices : IPlanServices
    { 
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PlanServices(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var Plans = await unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            var plans= mapper.Map<IEnumerable<PlanViewModel>>(Plans);
           #region Mapping Manual
            //var plans= Plans.Select(p => new PlanViewModel()
            // {
            //    Id = p.Id,
            //    Name = p.Name,
            //    Description = p.Description,
            //    DurationDays = p.DurationDays,
            //    Price = p.Price,
            //    IsActive = p.IsActive,
            // });
            #endregion
            return plans;
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var Plan=await unitOfWork.GetRepository<Plan>().GetByIdAsync(planId,ct);
            if (Plan == null) return null;
            else
            {
                var plan = mapper.Map<PlanViewModel>(Plan);
                #region Manual Mapping
                //return new PlanViewModel()
                //{
                //    Id = Plan.Id,
                //    Name = Plan.Name,
                //    Description = Plan.Description,
                //    DurationDays = Plan.DurationDays,
                //    Price = Plan.Price,
                //    IsActive = Plan.IsActive,
                //};
                #endregion
                return plan;
            }

        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan =await unitOfWork.GetRepository<Plan>().GetByIdAsync(planId,ct);
            if (plan is null||!plan.IsActive) return null;
            if (await HasActiveMembershipAsync(planId,ct)) return null;
           
            return mapper.Map<UpdatePlanViewModel>(plan);
            #region Manual Mapping
                //var Plan = new UpdatePlanViewModel()
                //{
                //    Name = plan.Name,
                //    Description = plan.Description,
                //    DurationDays = plan.DurationDays,
                //    Price = plan.Price
                //};
                #endregion'
            

        }

        public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null) return Result.Fail("Plan Is Not Found");
            if(plan.IsActive && await HasActiveMembershipAsync(planId, ct)) return Result.Validation("Can Not Remove Plan that Is Active Or Has Active Membership");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Remove the PLan");
        }

        public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var repo = unitOfWork.GetRepository<Plan>();
            var plan = await repo.GetByIdAsync(id, ct);
            if (plan is null) return Result.Fail("Plan not found.");
            if (await HasActiveMembershipAsync(id, ct))
                return Result.Fail("Cannot edit a plan that has active memberships.");

            mapper.Map(model, plan);
            //plan.Name=model.PlanName;
            //plan.Price=model.Price;
            //plan.Description=model.Description;
            //plan.DurationDays=model.DurationDays;
            plan.UpdatedAt = DateTime.Now;
            repo.Update(plan);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.OK();

        }
        private async Task<bool> HasActiveMembershipAsync(int PlanId, CancellationToken ct)
        {
            return await unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == PlanId && m.EndDate > DateTime.Now, ct);
        }

    }
}
