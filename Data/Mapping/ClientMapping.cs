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
    public class ClientMapping : MappingEntityTypeConfiguration<Client>
    {
        public override void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.FirstName).HasMaxLength(255);
            builder.Property(p => p.LastName).HasMaxLength(255);
            builder.Property(p => p.CompanyName).HasMaxLength(255);
            builder.Property(p => p.CreateUTC).HasColumnType("DateTime").HasDefaultValueSql("GetUtcDate()");

            base.Configure(builder);
        }
    }
}
