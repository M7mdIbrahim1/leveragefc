using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class project_additional_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ContractValue",
                table: "Project",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractValueCurrency",
                table: "Project",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractValue",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ContractValueCurrency",
                table: "Project");
        }
    }
}
