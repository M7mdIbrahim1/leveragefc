using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    public partial class new_client_and_company_files_details : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommercialRegistrationNumberPath",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "TaxCardNumberPath",
                table: "Client");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegistrationNumber",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxCardNumber",
                table: "Company",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientFile",
                columns: table => new
                {
                    ClientsId = table.Column<int>(type: "integer", nullable: false),
                    FilesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientFile", x => new { x.ClientsId, x.FilesId });
                    table.ForeignKey(
                        name: "FK_ClientFile_Client_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientFile_File_FilesId",
                        column: x => x.FilesId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyFile",
                columns: table => new
                {
                    CompaniesId = table.Column<int>(type: "integer", nullable: false),
                    FilesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyFile", x => new { x.CompaniesId, x.FilesId });
                    table.ForeignKey(
                        name: "FK_CompanyFile_Company_CompaniesId",
                        column: x => x.CompaniesId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyFile_File_FilesId",
                        column: x => x.FilesId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientFile_FilesId",
                table: "ClientFile",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyFile_FilesId",
                table: "CompanyFile",
                column: "FilesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientFile");

            migrationBuilder.DropTable(
                name: "CompanyFile");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CommercialRegistrationNumber",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "TaxCardNumber",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegistrationNumberPath",
                table: "Client",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxCardNumberPath",
                table: "Client",
                type: "text",
                nullable: true);
        }
    }
}
