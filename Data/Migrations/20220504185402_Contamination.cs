using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Contamination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Contamination",
                table: "SaleItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contamination",
                table: "SaleItems");
        }
    }
}
