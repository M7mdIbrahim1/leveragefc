using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class client_lob_many_to_many_remove_lobid_from_client : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineOfBusinessId",
                table: "Client");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineOfBusinessId",
                table: "Client",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
