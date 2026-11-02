using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Models
{
    public class Session:BaseEntity
    {
        public string Description { get; set; } = default!;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Trainer trainer { get; set; } = null!;
        public int TrainerId { get; set; }
        public Category category { get; set; } = null!;
        public int CategoryId { get; set; }

        public ICollection<Booking> Booking = new HashSet<Booking>();

    }
}
