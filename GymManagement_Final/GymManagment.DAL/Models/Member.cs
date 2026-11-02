using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Models
{
    public class Member:GymUser
    {
        public string? Photo { get; set; }
        //JoinDate = CreatedAt of BaseEntity
        public HealthRecord HealthRecord { get; set; } = null!;

        public ICollection<Membership> memberships = new HashSet<Membership>();

        public ICollection<Booking> Booking = new HashSet<Booking>();
    }
}
