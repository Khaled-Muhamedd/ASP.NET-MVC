using AutoMapper;
using AutoMapper.Execution;
using GymManagment.BLL.ViewModels.MembershipViewModels;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.BLL.ViewModels.PlanViewModels;
using GymManagment.BLL.ViewModels.SessionViewModels;
using GymManagment.BLL.ViewModels.TrainerViewModels;
using GymManagment.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Member = GymManagment.DAL.Models.Member;

namespace GymManagment.BLL
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //CreateMap (source , distination) ---> bydwer 3la properties b nafs el2esm "with the same name" w y3mel mapping
            MapMember();
            MapSession();
            MapPlan();
            MapTrainer();
            MapMemberships();

        }
        private void MapSession()
        {
            CreateMap<CreateSessionViewModel, Session>().ReverseMap();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Session,SessionViewModel>()
                 .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.category.CategoryName))
                 .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.trainer.Name))
                 .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore()).ReverseMap(); // Will Be Calculated After Map;
            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();

        }
        private void MapMember()
        {
            CreateMap<Member, MemberViewModel>()// done mapping
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City)).ReverseMap();

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.Street = src.Street;
                    dest.Address.City = src.City;
                });
            //or ---->.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src));

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
                {
                    Street = src.Street,
                    City = src.City,
                    BuildingNumber = src.BuildingNumber
                }))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));

        }
        private void MapPlan()
        {
            CreateMap<Plan, PlanViewModel>();
            CreateMap<Plan, UpdatePlanViewModel>().ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UpdatePlanViewModel, Plan>()
           .ForMember(dest => dest.Name, opt => opt.Ignore())
           .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));

        }
        private void MapTrainer()
        {
            CreateMap<CreateTrainerViewModel, Trainer>()
              .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
              {
                  BuildingNumber = src.BuildingNumber,
                  Street = src.Street,
                  City = src.City
              }));

            CreateMap<Trainer , TrainerViewModel>()
                .ForMember(dest => dest.Address,
                            opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));

            CreateMap<Trainer, UpdateTrainerViewModel>()
                .ForMember(dist => dist.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dist => dist.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dist => dist.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber));

            CreateMap<UpdateTrainerViewModel, Trainer>()
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.Address.BuildingNumber = src.BuildingNumber;
                dest.Address.City = src.City;
                dest.Address.Street = src.Street;
                dest.UpdatedAt = DateTime.Now;
            });
        }

        private void MapMemberships()
        {
            CreateMap<Membership, MemberShipForMemberViewModel>()
                     .ForMember(dist => dist.MemberName, Option => Option.MapFrom(Src => Src.member.Name))
                     .ForMember(dist => dist.PlanName, Option => Option.MapFrom(Src => Src.plan.Name))
                     .ForMember(dist => dist.StartDate, Option => Option.MapFrom(X => X.CreatedAt));

            CreateMap<Membership, MemberShipViewModel>()
                     .ForMember(dist => dist.MemberName, Option => Option.MapFrom(Src => Src.member.Name))
                     .ForMember(dist => dist.PlanName, Option => Option.MapFrom(Src => Src.plan.Name))
                                          .ForMember(dist => dist.StartDate, Option => Option.MapFrom(X => X.CreatedAt));

            CreateMap<CreateMemberShipViewModel, Membership>();
            CreateMap<Member, MemberSelectListViewModel>();
            CreateMap<Plan, PlanSelectListViewModel>();
        }
    }
}
