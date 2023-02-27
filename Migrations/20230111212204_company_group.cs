using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    public partial class company_group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Group",
                table: "LineOfBusiness",
                newName: "CompanyGroupId");

            migrationBuilder.CreateTable(
                name: "CompanyGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorUserId = table.Column<string>(type: "text", nullable: false),
                    LastUpdateUserId = table.Column<string>(type: "text", nullable: false),
                    ChangeSequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyGroup_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineOfBusiness_CompanyGroupId",
                table: "LineOfBusiness",
                column: "CompanyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyGroup_CompanyId",
                table: "CompanyGroup",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineOfBusiness_CompanyGroup_CompanyGroupId",
                table: "LineOfBusiness",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineOfBusiness_CompanyGroup_CompanyGroupId",
                table: "LineOfBusiness");

            migrationBuilder.DropTable(
                name: "CompanyGroup");

            migrationBuilder.DropIndex(
                name: "IX_LineOfBusiness_CompanyGroupId",
                table: "LineOfBusiness");

            migrationBuilder.RenameColumn(
                name: "CompanyGroupId",
                table: "LineOfBusiness",
                newName: "Group");
        }
    }
}
