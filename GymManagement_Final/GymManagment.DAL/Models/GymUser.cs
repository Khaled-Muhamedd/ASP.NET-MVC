using GymManagment.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Models
{
    public abstract class GymUser:BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        [RegularExpression(@"^(010|011|012|015)\d{8}$",
       ErrorMessage = "Egyptian phone format only.")]
        public string Phone { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; }
        public Address Address { get; set; }
        public Gender Gender { get; set; }
    }
    [Owned]
    public class Address
    {
        public string Street { get; set; }=default!;
        public string City { get; set; }=default!;
        public int BuildingNumber { get; set; }
    }

}
