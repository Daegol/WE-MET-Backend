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
    public class RecycledItemMapping : MappingEntityTypeConfiguration<RecycledItem>
    {
        public override void Configure(EntityTypeBuilder<RecycledItem> builder)
        {
            builder.ToTable("RecycledItems");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CreateUTC).HasColumnType("DateTime").HasDefaultValueSql("GetUtcDate()");

            builder.HasOne(x => x.Recycled)
                .WithMany(x => x.RecycledItems)
                .HasForeignKey(x => x.RecycledId);

            builder.HasOne(x => x.SubCategory)
                .WithMany(x => x.RecycledItems)
                .HasForeignKey(x => x.SubCategoryId);

            base.Configure(builder);
        }
    }
}
