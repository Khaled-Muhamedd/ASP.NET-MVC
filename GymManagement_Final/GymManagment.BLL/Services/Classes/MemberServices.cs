using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Attachement;
using GymManagment.BLL.Services.Attachment;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAttachmentService attachmentService;

        public MemberServices(IUnitOfWork unitOfWork, IMapper mapper , IAttachmentService attachmentService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.attachmentService = attachmentService;
        }


        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members= await unitOfWork.GetRepository<Member>().GetAllAsync(ct :ct);
            if (!members.Any())
            {
                return [];
            }
            #region Manual Mapping
            //var membersViewModel = members.Select(m => new MemberViewModel()
            //{
            //    Email = m.Email,
            //    Phone = m.Phone,
            //    Name = m.Name,
            //    Id = m.Id,
            //    Gender=m.Gender.ToString(),
            //    Photo=m.Photo
            //});
            #endregion
            var membersViewModel = mapper.Map< IEnumerable<Member> , IEnumerable<MemberViewModel>>(members);
            
            return membersViewModel;
        }

        async Task<Result> IMemberServices.CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {

            //check Email
            var EmailExist = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            //check Phone
            var PhoneExist = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);
            //If Email or Phone Exist Return False
            if (EmailExist || PhoneExist) return Result.Validation("Email or Phone is Already Taken");

            //Upload Photo
            var photo = await attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPictures", ct);
            if (string.IsNullOrEmpty(photo.value))
                return Result.Validation("Profile photo upload failed (check file type and size).");


            //Else Return True and Add Member
            #region
            //var member = new Member()
            //{

            //    Email = model.Email,
            //    Phone = model.Phone,
            //    Name = model.Name,
            //    Gender=model.Gender,
            //    DateOfBirth=model.DateOfBirth,
            //    Address =new Address()
            //    {
            //        City=model.City,
            //        Street=model.Street,
            //        BuildingNumber=model.BuildingNumber

            //    },
            //    HealthRecord=new HealthRecord()
            //    {
            //        Weight=model.HealthRecordViewModel.Weight,
            //        BloodType=model.HealthRecordViewModel.BloodType,
            //        Height=model.HealthRecordViewModel.Height,
            //        Note=model.HealthRecordViewModel.Note
            //    }

            //};
            #endregion
            var member = mapper.Map<Member>(model);
            member.Photo = photo.value;
            //var result = await memberRepository.Add(member);//wrong 
            unitOfWork.GetRepository<Member>().Add(member); //Add local
            var result = await unitOfWork.SaveChangesAsync(ct);

            //return result > 0 ? Result.OK() : Result.Fail("Failed To Create Member");
            if (result > 0)
            {
               return Result.OK();
            }
            else
                 //Delete Uploaded Photo
                 if (!string.IsNullOrEmpty(member.Photo))
                attachmentService.Delete(member.Photo, "MembersPictures");
            return Result.Fail("Failed To Create Member");

        }

        async Task<Result> IMemberServices.UpdateMemberDetailsAsync(int id,MemberToUpdateViewModel model, CancellationToken ct)
        {
            var _member=await unitOfWork.GetRepository<Member>().GetByIdAsync(id,ct);
            if(_member == null) return Result.Fail("Member Is Not Found");
            //check Email
            var EmailExist = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email && x.Id != id );
            //check Phone
            var PhoneExist = await unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone && x.Id != id);
            //If Email or Phone Exist Return False
            if (EmailExist || PhoneExist) return Result.Validation("Email or Phone is Already Taken");
            //Else Return True and Add Member
            #region manual mapping
            //_member.Email=model.Email;
            //_member.Phone=model.Phone;
            //_member.Address.BuildingNumber= model.BuildingNumber;
            //_member.Address.City = model.City;
            //_member.Address.Street = model.Street;
            #endregion 
            //             <(source , destination)>
            mapper.Map(model,_member);
            _member.UpdatedAt = DateTime.Now;// not mapped in auto mapping because it should be guarantee that it is the last time recirder after all operation


            unitOfWork.GetRepository<Member>().Update(_member);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Update Member");
        }

        async Task<HealthRecordViewModel?> IMemberServices.GetHealthRecordDetailsByTdAsync(int MemberId, CancellationToken cancellationToken)
        {

            //Get HealthRecord By Member Id
            //Ckeck->IsHealthRecord Equal null? (if it  null--> return Index With Message)
            // else HealthRecord is not null (retutn view with data )
            var healthrecordmember =await unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x=>x.MemberId== MemberId,false, cancellationToken);

            if (healthrecordmember == null) return null;
            var model = mapper.Map<HealthRecord,HealthRecordViewModel>(healthrecordmember);
            #region manual mapping
            //var model = new HealthRecordViewModel()
            //{
            //    Height = healthrecordmember.Height,
            //    Weight=healthrecordmember.Weight,
            //    Note=healthrecordmember.Note,
            //    BloodType = healthrecordmember.BloodType

            //};
            #endregion
            return model;
        }

        async Task<MemberViewModel?> IMemberServices.GetMemberDetailsByTdAsync(int MemberId, CancellationToken cancellationToken)
        {
            //Get Member Id
            //Ckeck->IsMember Equal null? (if it  null--> return Index With Message)
            // else member is not null (retutn view with data )
            var member= await unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, cancellationToken);
            
            if(member ==null) return null;
            #region Manual Mapping
            //var model = new MemberViewModel()
            //{
            //    Name = member.Name,
            //    Email=member.Email,
            //    Phone=member.Phone,
            //    Gender=member.Gender.ToString(),
            //    DateOfBirth=member.DateOfBirth.ToShortDateString(),
            //    Address=$"{ member.Address.BuildingNumber } - {member.Address.Street} - {member.Address.City}"
            //};
            #endregion

            var model = mapper.Map<Member,MemberViewModel>(member);// to avoid manual mapping by install auto mapper package and use it
            var activeMembership = await unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(x => x.MemberId == MemberId && x.EndDate>DateTime.Now);
           
            if (activeMembership is not null) 
            {
                var ActivePlan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, cancellationToken);
                model.PlanName = ActivePlan?.Name;
                model.MembershipStartDate=activeMembership.CreatedAt.ToShortDateString();
                model.MembershipEndDate=activeMembership.EndDate.ToShortDateString();
                
            }

            return model;
        }

        async Task<Result<MemberToUpdateViewModel?>> IMemberServices.GetMemberToUpdate(int MemberId, CancellationToken ct)
        {
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null) return Result<MemberToUpdateViewModel?>.NotFound("Member Is Not Found");
            #region mapping manual
            //var model = new MemberToUpdateViewModel()
            //{
            //    Name= member.Name,
            //    Email=member.Email,
            //    Phone=member.Phone,
            //    Photo=member.Photo,
            //    BuildingNumber=member.Address.BuildingNumber,
            //    Street=member.Address.Street,
            //    City=member.Address.City
            //};
            #endregion
            else
            {
                var result = mapper.Map<Member, MemberToUpdateViewModel>(member);

                return Result<MemberToUpdateViewModel?>.OK(result);
            }
            
        }

        async Task<Result> IMemberServices.RemoveMemberAsync(int MemberId, CancellationToken cancellationToken)
        {
            var member=await unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, cancellationToken);
            if(member==null) return Result.Fail("Member Is Not Found");
            
            var hasFutureBookings=await unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId==member.Id && b.Session.StartDate>DateTime.Now , cancellationToken);
            if (hasFutureBookings) return Result.Validation("Member Has Booking Session");
           
            unitOfWork.GetRepository<Member>().Delete(member);
            var result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result>0 ? Result.OK() :Result.Fail("Failed To Remove Member") ;
        }
    }
}
