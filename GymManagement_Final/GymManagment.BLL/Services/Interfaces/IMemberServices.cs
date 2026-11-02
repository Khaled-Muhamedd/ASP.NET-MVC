using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface IMemberServices
    {
        //put all services
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken cancellationToken = default);
        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);

        Task<MemberViewModel?> GetMemberDetailsByTdAsync(int MemberId, CancellationToken cancellationToken = default);
        Task<HealthRecordViewModel?> GetHealthRecordDetailsByTdAsync(int MemberId, CancellationToken cancellationToken = default);
        Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdate(int MemberId, CancellationToken ct = default);
        Task<Result> UpdateMemberDetailsAsync(int id,MemberToUpdateViewModel model, CancellationToken ct = default);
        Task<Result> RemoveMemberAsync(int MemberId, CancellationToken cancellationToken = default);
    }
}
