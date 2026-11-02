using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Models
{
    public class Membership:BaseEntity
    {
        public Member member { get; set; } = null!;
        public int MemberId { get; set; }
        public Plan plan { get; set; } = null!;
        public int PlanId { get; set; }
        public DateTime EndDate { get; set; }

        [NotMapped]
        public string Status => EndDate < DateTime.Now ? "Expired" : "Active";

        [NotMapped]
        public bool IsActive => EndDate > DateTime.Now;
    }
}
//MembershipConfiguration