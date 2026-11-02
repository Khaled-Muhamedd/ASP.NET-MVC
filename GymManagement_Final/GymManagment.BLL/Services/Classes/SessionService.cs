using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.SessionViewModels;
using GymManagment.DAL.Models;
using GymManagment.DAL.Models.Enums;
using GymManagment.DAL.Repositories.Classes;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel session, CancellationToken ct = default)
        {
            if (session.EndDate <= session.StartDate) return Result.Validation("EndDate Must Be After StartDate");
            if (session.StartDate <= DateTime.Now) return Result.Validation("StartDate Must Be In the Future");
            if (session.Capacity < 1 || session.Capacity > 25) return Result.Validation("Capacity Must Be Between 1 and 25");

            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(session.TrainerId);
            if (trainer == null) return Result.Fail("Trainer Not Found");

            var category =await unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);
            if(category == null) return Result.Fail("Category Not Found");

            var IIsValid=Enum.TryParse<Specialty>(category.CategoryName, true ,out var CategorySpecialty);
            if(!IIsValid || trainer.Specialty!=CategorySpecialty) return Result.Validation("Can Not Create this Session To this Trainer");

            var model= mapper.Map<CreateSessionViewModel, Session>(session);

            unitOfWork.GetRepository<Session>().Add(model); // add locally
            var result =await  unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.OK() : Result.Fail("Failed To Create Session");
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessionRepoo = unitOfWork.SessionRepository;
            var sessions = await sessionRepoo.GetAllSessionsWithTrainerAndCategoryAsync(ct: ct);
            if (sessions == null || !sessions.Any()) return null;
            sessions = sessions.OrderByDescending(X => X.StartDate);
            var mappedSessions = mapper.Map<IEnumerable< SessionViewModel >>(sessions);
            #region Mapping Manual
            //var mappedSessions = sessions.Select(s => new SessionViewModel()
            //{
            //    Id = s.Id,
            //    Description = s.Description,
            //    Capacity = s.Capacity,
            //    EndDate = s.EndDate,
            //    StartDate = s.StartDate,
            //    CategoryName = s.category.CategoryName,
            //    TrainerName = s.trainer.Name,
            //});
            #endregion
            foreach (var session in mappedSessions)
            {
                // performance!!!!!!!
                //n+1 problem 
                session.AvailableSlots = session.Capacity - await sessionRepoo.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return mappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var result = await unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);
            return mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct = default)
        {
            var session =await unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategory(id, ct);
            if (session is null)
                return Result<SessionViewModel>.NotFound("Session Not Found");

            else
            {
                var mappedSession = mapper.Map<Session, SessionViewModel>(session);//
                mappedSession.AvailableSlots = mappedSession.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id,ct);
                return Result<SessionViewModel>.OK(mappedSession);
            }
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var result = await unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now) return Result<UpdateSessionViewModel>.Validation("Can Not Update Session that Has Already Started");

            var bookingCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookingCount > 0) return Result<UpdateSessionViewModel>.Validation("Can Not Update On Session that Already Booked");


            var mappedSession = mapper.Map<Session, UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.OK(mappedSession);

        }
        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null) return Result.Fail("Session Not Found ");

            //Then the same Validation As Create Session
            if (model.EndDate <= model.StartDate) return Result.Validation("EndDate Must Be After StartDate");
            if (session.StartDate <= DateTime.Now) return Result.Fail("Can Not Edit Session That Has Already Started");

            var bookingCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookingCount > 0) return Result.Validation("Can Not Update On Session that Already Booked");

            if (model.StartDate <= DateTime.Now) return Result.Validation("StartDate Must Be In the Future");

            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer == null) return Result.Fail("Trainer Not Found");

            var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);

            var IIsValid = Enum.TryParse<Specialty>(category?.CategoryName, true, out var CategorySpecialty);
            if (!IIsValid || trainer.Specialty != CategorySpecialty) return Result.Validation("Can Not Assign this Trainer To this Session Category");

            mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            unitOfWork.SessionRepository.Update(session);
            var result =await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To UpdateSession");
        }

        public async Task<Result> RemoveSessionAsync(int id, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(id,ct);
            if (session == null) return Result.Fail("Session Not Found");

            var bookingCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookingCount > 0) return Result.Validation("Can Not Remove this Session that Already Booked");

            if (session.EndDate >= DateTime.Now) return Result.Fail("Can Not Remove Session That Has Still Running");

            unitOfWork.SessionRepository.Delete(session);
            var result =await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Remove The Session");
        }
    }
}
