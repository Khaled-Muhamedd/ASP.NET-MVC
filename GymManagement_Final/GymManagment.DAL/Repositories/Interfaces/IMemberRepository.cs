using GymManagment.DAL.Models;

namespace GymManagment.DAL.Repositories.Interfaces
{
    public interface IMemberRepository:IGenericRepository<Member>
    {
        //m4 h3mlo 8er lma a7tag (new feature within this Module only) 8er kda h4t8l b el generic bs 
        //new Features
        //Task<Member> GetMemberWithHealthRecordById(int id);//include
    }
}
