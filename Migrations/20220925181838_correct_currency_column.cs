using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class correct_currency_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstProposalValueCurrncey",
                table: "OpportunityHistory",
                newName: "FirstProposalValueCurrency");

            migrationBuilder.RenameColumn(
                name: "FinalProposalValueCurrncey",
                table: "OpportunityHistory",
                newName: "FinalProposalValueCurrency");

            migrationBuilder.RenameColumn(
                name: "CurrentProposalValueCurrncey",
                table: "OpportunityHistory",
                newName: "CurrentProposalValueCurrency");

            migrationBuilder.RenameColumn(
                name: "FirstProposalValueCurrncey",
                table: "Opportunity",
                newName: "FirstProposalValueCurrency");

            migrationBuilder.RenameColumn(
                name: "FinalContractValueCurrncey",
                table: "Opportunity",
                newName: "FinalContractValueCurrency");

            migrationBuilder.RenameColumn(
                name: "CurrentProposalValueCurrncey",
                table: "Opportunity",
                newName: "CurrentProposalValueCurrency");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstProposalValueCurrency",
                table: "OpportunityHistory",
                newName: "FirstProposalValueCurrncey");

            migrationBuilder.RenameColumn(
                name: "FinalProposalValueCurrency",
                table: "OpportunityHistory",
                newName: "FinalProposalValueCurrncey");

            migrationBuilder.RenameColumn(
                name: "CurrentProposalValueCurrency",
                table: "OpportunityHistory",
                newName: "CurrentProposalValueCurrncey");

            migrationBuilder.RenameColumn(
                name: "FirstProposalValueCurrency",
                table: "Opportunity",
                newName: "FirstProposalValueCurrncey");

            migrationBuilder.RenameColumn(
                name: "FinalContractValueCurrency",
                table: "Opportunity",
                newName: "FinalContractValueCurrncey");

            migrationBuilder.RenameColumn(
                name: "CurrentProposalValueCurrency",
                table: "Opportunity",
                newName: "CurrentProposalValueCurrncey");
        }
    }
}
