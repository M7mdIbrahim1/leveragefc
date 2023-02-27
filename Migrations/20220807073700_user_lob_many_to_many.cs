using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class user_lob_many_to_many : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LineOfBusiness_LineOfBusinessId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LineOfBusinessId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LineOfBusinessId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ApplicationUserLineOfBusiness",
                columns: table => new
                {
                    LineOfBusinessesId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserLineOfBusiness", x => new { x.LineOfBusinessesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserLineOfBusiness_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserLineOfBusiness_LineOfBusiness_LineOfBusiness~",
                        column: x => x.LineOfBusinessesId,
                        principalTable: "LineOfBusiness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLineOfBusiness_UsersId",
                table: "ApplicationUserLineOfBusiness",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserLineOfBusiness");

            migrationBuilder.AddColumn<int>(
                name: "LineOfBusinessId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LineOfBusinessId",
                table: "AspNetUsers",
                column: "LineOfBusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_LineOfBusiness_LineOfBusinessId",
                table: "AspNetUsers",
                column: "LineOfBusinessId",
                principalTable: "LineOfBusiness",
                principalColumn: "Id");
        }
    }
}
