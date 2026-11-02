using GymManagment.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagment.DAL.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(x => x.Name)
                .HasColumnType("varchar")// defalt was nvarchar
                .HasMaxLength(50);//3lshan el entity framework ykon 3arf en el max length 50 m4 h3ml (varchar(50)) 3lshan kda DB bs hya elly 3arfa w ana 3awza a3rf eletneen
           
            builder.Property(d => d.Description)
                .HasMaxLength(100);
          
            builder.Property(p => p.Price)
                .HasPrecision(10, 2);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("PlanDurationCheck", "DurationDays Between 1 and 365");
            });
        }
    }
}
