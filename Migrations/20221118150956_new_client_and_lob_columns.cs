using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class new_client_and_lob_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Client",
                newName: "TaxCardNumberPath");

            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "LineOfBusiness",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Client",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Client",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Client",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegistrationNumberPath",
                table: "Client",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Client",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Client",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "Client",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Client",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                table: "LineOfBusiness");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "CommercialRegistrationNumberPath",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Client");

            migrationBuilder.RenameColumn(
                name: "TaxCardNumberPath",
                table: "Client",
                newName: "Address");
        }
    }
}
