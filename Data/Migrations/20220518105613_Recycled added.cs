using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Recycledadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recycleds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "DateTime", nullable: false),
                    DateToApproval = table.Column<DateTime>(type: "DateTime", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CreateUTC = table.Column<DateTime>(type: "DateTime", nullable: false, defaultValueSql: "GetUtcDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recycleds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recycleds_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecycledItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    RecycledId = table.Column<int>(type: "int", nullable: false),
                    CreateUTC = table.Column<DateTime>(type: "DateTime", nullable: false, defaultValueSql: "GetUtcDate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecycledItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecycledItems_Recycleds_RecycledId",
                        column: x => x.RecycledId,
                        principalTable: "Recycleds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecycledItems_SubCategories_RecycledId",
                        column: x => x.RecycledId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecycledItems_RecycledId",
                table: "RecycledItems",
                column: "RecycledId");

            migrationBuilder.CreateIndex(
                name: "IX_Recycleds_EmployeeId",
                table: "Recycleds",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecycledItems");

            migrationBuilder.DropTable(
                name: "Recycleds");
        }
    }
}
