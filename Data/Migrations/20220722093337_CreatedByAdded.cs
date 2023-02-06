using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class CreatedByAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecycledItems_SubCategories_RecycledId",
                table: "RecycledItems");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Recycleds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RecycledItems_SubCategoryId",
                table: "RecycledItems",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecycledItems_SubCategories_SubCategoryId",
                table: "RecycledItems",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecycledItems_SubCategories_SubCategoryId",
                table: "RecycledItems");

            migrationBuilder.DropIndex(
                name: "IX_RecycledItems_SubCategoryId",
                table: "RecycledItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Recycleds");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Purchases");

            migrationBuilder.AddForeignKey(
                name: "FK_RecycledItems_SubCategories_RecycledId",
                table: "RecycledItems",
                column: "RecycledId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
