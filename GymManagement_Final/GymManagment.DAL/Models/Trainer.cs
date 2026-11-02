using GymManagment.DAL.Models.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Models
{
    public class Trainer:GymUser
    {
        public Specialty Specialty { get; set; }

        public ICollection<Session> sessions = new HashSet<Session>();
        //HireDate =CreatedAt of BaseEntity

    }
}
