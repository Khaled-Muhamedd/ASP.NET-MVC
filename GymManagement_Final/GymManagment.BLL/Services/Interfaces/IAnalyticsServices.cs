using GymManagment.BLL.ViewModels.AnalyticsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface IAnalyticsServices
    {
        Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default);
    }
}
