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
    public class PurchaseMapping : MappingEntityTypeConfiguration<Purchase>
    {
        public override void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Date).HasColumnType("DateTime");
            builder.Property(p => p.DateToApproval).HasColumnType("DateTime");
            builder.Property(p => p.CreateUTC).HasColumnType("DateTime").HasDefaultValueSql("GetUtcDate()");

            builder.HasOne(x => x.Client)
                .WithMany(x => x.Purchases)
                .HasForeignKey(x => x.ClientId);

            base.Configure(builder);
        }
    }
}
