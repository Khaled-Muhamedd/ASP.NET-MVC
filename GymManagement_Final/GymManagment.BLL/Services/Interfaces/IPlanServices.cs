using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.PlanViewModels;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface IPlanServices
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);

        Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default);

        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);

        Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default);

        Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
    }
}
