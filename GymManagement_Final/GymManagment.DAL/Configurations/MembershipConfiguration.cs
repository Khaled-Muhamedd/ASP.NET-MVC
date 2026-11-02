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
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedAt)
                .HasColumnName("StartDate")
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(m=>m.plan)
                .WithMany(p=>p.memberships)
                .HasForeignKey(m=>m.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.member)
               .WithMany(p => p.memberships)
               .HasForeignKey(m => m.MemberId)
               .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
