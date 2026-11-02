using GymManagment.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Configurations
{
    public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.Property(p => p.Weight)
              .HasPrecision(10, 2);

            builder.Property(p => p.Height)
             .HasPrecision(10, 2);

            builder.Property(x => x.Note)
                .HasMaxLength(200);
        }
    }
}
