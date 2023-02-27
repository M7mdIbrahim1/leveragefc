using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class client_lob_many_to_many : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_LineOfBusiness_LineOfBusinessId",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_LineOfBusinessId",
                table: "Client");

            migrationBuilder.CreateTable(
                name: "ClientLineOfBusiness",
                columns: table => new
                {
                    ClientsId = table.Column<int>(type: "integer", nullable: false),
                    LineOfBusinessesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLineOfBusiness", x => new { x.ClientsId, x.LineOfBusinessesId });
                    table.ForeignKey(
                        name: "FK_ClientLineOfBusiness_Client_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientLineOfBusiness_LineOfBusiness_LineOfBusinessesId",
                        column: x => x.LineOfBusinessesId,
                        principalTable: "LineOfBusiness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientLineOfBusiness_LineOfBusinessesId",
                table: "ClientLineOfBusiness",
                column: "LineOfBusinessesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientLineOfBusiness");

            migrationBuilder.CreateIndex(
                name: "IX_Client_LineOfBusinessId",
                table: "Client",
                column: "LineOfBusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_LineOfBusiness_LineOfBusinessId",
                table: "Client",
                column: "LineOfBusinessId",
                principalTable: "LineOfBusiness",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
