using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Models
{
    public class Category:BaseEntity
    {
        public string CategoryName { get; set; } = null!;

        public ICollection<Session> sessions = new HashSet<Session>();

    }
}
