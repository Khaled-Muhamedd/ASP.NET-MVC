using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Models
{
    public class HealthRecord :BaseEntity
    {
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string? Note { get; set; }
        public string BloodType { get; set; } = null!;// or enum
        public Member member { get; set; } = null!;
        public int MemberId { get; set; }//FK

        //LastUpdated=UpdatedAt of BaseEntity
        //HealthRecordConfiguration
    }
}
