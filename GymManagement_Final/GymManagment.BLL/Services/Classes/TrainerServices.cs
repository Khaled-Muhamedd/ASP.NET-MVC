using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.TrainerViewModels;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Interfaces;
using GymManagment.DAL.Models.Enums;

namespace GymManagment.BLL.Services.Classes
{
    public class TrainerServices : ITrainerServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public TrainerServices(IUnitOfWork unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct)) return Result.Validation("The Email Is Already Taken");
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct)) return Result.Validation("The Phone Is Already Taken");
            var trainer =mapper.Map<Trainer>(model);
            #region Manual Mapping
            //var trainer = new Trainer()
            //{
            //    Email = model.Email,
            //    Phone = model.Phone,
            //    Name = model.Name,
            //    DateOfBirth = model.DateOfBirth,
            //    Gender = model.Gender,
            //    Specialty = model.Specialties,
            //    Address = new Address()
            //    {
            //        City = model.City,
            //        Street = model.Street,
            //        BuildingNumber = model.BuildingNumber
            //    }
            //};
            #endregion
            unitOfWork.GetRepository<Trainer>().Add(trainer);
            var result= await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Create Trainer");
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            if (trainers == null) return [];
            return mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
            #region Manual Mapping
            //return trainers.Select(x => new TrainerViewModel()
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        Email = x.Email,
            //        Phone = x.Phone,
            //        Specialties = x.Specialty.ToString()
            //    });
            #endregion
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer=await unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId);
            if (trainer == null) return null;
            return mapper.Map<TrainerViewModel?>(trainer);
            #region Manaual Mapping 
            //else
            //{
            //    return new TrainerViewModel()
            //    {
            //        Name= trainer.Name,
            //        Email= trainer.Email,
            //        Phone= trainer.Phone,
            //        Specialties= trainer.Specialty.ToString(),
            //        DateOfBirth=trainer.DateOfBirth.ToString(),
            //        Address=$"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
            //    };
            //}
            #endregion
        }

        public async Task<UpdateTrainerViewModel?> GetTrainerToUpdateAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer  = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct);
            if (trainer == null ) return null;
            return mapper.Map<UpdateTrainerViewModel>(trainer);
            #region Manual Mapping
            //else
            //{
            //    return new UpdateTrainerViewModel()
            //    {
            //        Name = trainer.Name,
            //        Email = trainer.Email,
            //        Phone = trainer.Phone,
            //        BuildingNumber=trainer.Address.BuildingNumber,
            //        City=trainer.Address.City,
            //        Street=trainer.Address.Street,
            //        Specialties=trainer.Specialty
            //    };
            //}
            #endregion
        }

        public async Task<Result> RemoveTrainerAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer =await unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct);
            if (trainer == null) return Result.Fail("Trainer Is Not Found");

            var hasFutureSessions =await unitOfWork.GetRepository<Session>().AnyAsync(s=>s.TrainerId==TrainerId && s.EndDate>DateTime.Now, ct);
            if (hasFutureSessions) return Result.Validation("Can Not Remove Trainer that Has Future Sessions");

            unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Remove The Trainer");
        }

        public async Task<Result> UpdateTrainerAsync(int TrainerId, UpdateTrainerViewModel model, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct);
            if (trainer == null) return Result.Fail("Trainer Is Not Found");
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(t=>t.Id!=TrainerId && t.Email==model.Email,ct)) return Result.Validation("Another trainer is already using this email.");
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Id != TrainerId && t.Phone == model.Phone,ct)) return Result.Fail("Another trainer is already using this Phone.");
            var trainerSessions = await unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(s => s.TrainerId == TrainerId, ct);

            foreach (var session in trainerSessions)
            {
                var isValid = Enum.TryParse<Specialty>(session.category.CategoryName, true, out var sessionCategorySpecialty);
                if (!isValid || model.Specialty != sessionCategorySpecialty)
                    return Result.Validation("Can Not Change Specialty, It Does Not Match The Category Of Sessions Already Assigned To This Trainer");
            }
            mapper.Map(model ,trainer);
            #region Manual Mapping
            //trainer.Name = model.Name;
            //trainer.Email = model.Email;
            //trainer.Phone = model.Phone;
            //trainer.Address.BuildingNumber = model.BuildingNumber;
            //trainer.Address.Street = model.Street;  
            //trainer.Address.City = model.City;
            //trainer.UpdatedAt= DateTime.Now;
            #endregion
            unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Update The Trainer");

        }

    }
}
