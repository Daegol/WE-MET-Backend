using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mapping
{
    public class RecycledMapping : MappingEntityTypeConfiguration<Recycled>
    {
        public override void Configure(EntityTypeBuilder<Recycled> builder)
        {
            builder.ToTable("Recycleds");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Date).HasColumnType("DateTime");
            builder.Property(p => p.DateToApproval).HasColumnType("DateTime");
            builder.Property(p => p.CreateUTC).HasColumnType("DateTime").HasDefaultValueSql("GetUtcDate()");

            builder.HasOne(x => x.Employee)
                .WithMany(x => x.Recycleds)
                .HasForeignKey(x => x.EmployeeId);

            base.Configure(builder);
        }
    }
}
