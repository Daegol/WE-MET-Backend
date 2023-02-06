using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.DbEntities;

namespace Data.Mapping
{
    public class PurchaseItemMapping : MappingEntityTypeConfiguration<PurchaseItem>
    {
        public override void Configure(EntityTypeBuilder<PurchaseItem> builder)
        {
            builder.ToTable("PurchaseItems");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CreateUTC).HasColumnType("DateTime").HasDefaultValueSql("GetUtcDate()");

            builder.HasOne(x => x.Purchase)
                .WithMany(x => x.PurchaseItems)
                .HasForeignKey(x => x.PurchaseId);

            builder.HasOne(x => x.SubCategory)
                .WithMany(x => x.PurchaseItems)
                .HasForeignKey(x => x.SubCategoryId);

            base.Configure(builder);
        }
    }
}
