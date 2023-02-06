﻿// <auto-generated />
using System;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220722093337_CreatedByAdded")]
    partial class CreatedByAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.14")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Models.DbEntities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CompanyName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<string>("FirstName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("LastName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Models.DbEntities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<string>("FirstName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("LastName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Models.DbEntities.MainCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("MainCategory");
                });

            modelBuilder.Entity("Models.DbEntities.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Approved")
                        .HasColumnType("bit");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("DateTime");

                    b.Property<DateTime>("DateToApproval")
                        .HasColumnType("DateTime");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("Models.DbEntities.PurchaseItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<double>("Contamination")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<int>("PurchaseId")
                        .HasColumnType("int");

                    b.Property<int>("SubCategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PurchaseId");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("PurchaseItems");
                });

            modelBuilder.Entity("Models.DbEntities.Recycled", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Approved")
                        .HasColumnType("bit");

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("DateTime");

                    b.Property<DateTime>("DateToApproval")
                        .HasColumnType("DateTime");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Recycleds");
                });

            modelBuilder.Entity("Models.DbEntities.RecycledItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<int>("RecycledId")
                        .HasColumnType("int");

                    b.Property<int>("SubCategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecycledId");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("RecycledItems");
                });

            modelBuilder.Entity("Models.DbEntities.Sale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Approved")
                        .HasColumnType("bit");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("DateTime");

                    b.Property<DateTime>("DateToApproval")
                        .HasColumnType("DateTime");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("Models.DbEntities.SaleItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<int>("SaleId")
                        .HasColumnType("int");

                    b.Property<int>("SubCategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SaleId");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("SaleItems");
                });

            modelBuilder.Entity("Models.DbEntities.SubCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateUTC")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("DateTime")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<int>("MainCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("MainCategoryId");

                    b.ToTable("SubCategories");
                });

            modelBuilder.Entity("Models.DbEntities.Purchase", b =>
                {
                    b.HasOne("Models.DbEntities.Client", "Client")
                        .WithMany("Purchases")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Models.DbEntities.PurchaseItem", b =>
                {
                    b.HasOne("Models.DbEntities.Purchase", "Purchase")
                        .WithMany("PurchaseItems")
                        .HasForeignKey("PurchaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.DbEntities.SubCategory", "SubCategory")
                        .WithMany("PurchaseItems")
                        .HasForeignKey("SubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Purchase");

                    b.Navigation("SubCategory");
                });

            modelBuilder.Entity("Models.DbEntities.Recycled", b =>
                {
                    b.HasOne("Models.DbEntities.Employee", "Employee")
                        .WithMany("Recycleds")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Models.DbEntities.RecycledItem", b =>
                {
                    b.HasOne("Models.DbEntities.Recycled", "Recycled")
                        .WithMany("RecycledItems")
                        .HasForeignKey("RecycledId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.DbEntities.SubCategory", "SubCategory")
                        .WithMany("RecycledItems")
                        .HasForeignKey("SubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recycled");

                    b.Navigation("SubCategory");
                });

            modelBuilder.Entity("Models.DbEntities.Sale", b =>
                {
                    b.HasOne("Models.DbEntities.Client", "Client")
                        .WithMany("Sales")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Models.DbEntities.SaleItem", b =>
                {
                    b.HasOne("Models.DbEntities.Sale", "Sale")
                        .WithMany("SaleItems")
                        .HasForeignKey("SaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.DbEntities.SubCategory", "SubCategory")
                        .WithMany("SaleItems")
                        .HasForeignKey("SubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sale");

                    b.Navigation("SubCategory");
                });

            modelBuilder.Entity("Models.DbEntities.SubCategory", b =>
                {
                    b.HasOne("Models.DbEntities.MainCategory", "MainCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("MainCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MainCategory");
                });

            modelBuilder.Entity("Models.DbEntities.Client", b =>
                {
                    b.Navigation("Purchases");

                    b.Navigation("Sales");
                });

            modelBuilder.Entity("Models.DbEntities.Employee", b =>
                {
                    b.Navigation("Recycleds");
                });

            modelBuilder.Entity("Models.DbEntities.MainCategory", b =>
                {
                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("Models.DbEntities.Purchase", b =>
                {
                    b.Navigation("PurchaseItems");
                });

            modelBuilder.Entity("Models.DbEntities.Recycled", b =>
                {
                    b.Navigation("RecycledItems");
                });

            modelBuilder.Entity("Models.DbEntities.Sale", b =>
                {
                    b.Navigation("SaleItems");
                });

            modelBuilder.Entity("Models.DbEntities.SubCategory", b =>
                {
                    b.Navigation("PurchaseItems");

                    b.Navigation("RecycledItems");

                    b.Navigation("SaleItems");
                });
#pragma warning restore 612, 618
        }
    }
}
