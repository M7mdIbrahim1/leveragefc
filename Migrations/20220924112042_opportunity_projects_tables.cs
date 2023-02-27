using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    public partial class opportunity_projects_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Milestone",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LineOfBusiness",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRetainer",
                table: "LineOfBusiness",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Company",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Client",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectMilestoneId = table.Column<int>(type: "integer", nullable: false),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InvoiceNumber = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: true),
                    ClientName = table.Column<string>(type: "text", nullable: true),
                    InvoiceDateFW = table.Column<int>(type: "integer", nullable: true),
                    InvoiceDateFM = table.Column<int>(type: "integer", nullable: true),
                    OriginReference = table.Column<int>(type: "integer", nullable: true),
                    UntaxedAmount = table.Column<double>(type: "double precision", nullable: true),
                    TaxAmount = table.Column<double>(type: "double precision", nullable: true),
                    TotalAmount = table.Column<double>(type: "double precision", nullable: false),
                    AmountDue = table.Column<double>(type: "double precision", nullable: true),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    InvoiceStatus = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Opportunity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    ClientStatus = table.Column<int>(type: "integer", nullable: false),
                    LineOfBusinessId = table.Column<int>(type: "integer", nullable: false),
                    ProjectName = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FirstContactDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FirstProposalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FirstProposalValue = table.Column<double>(type: "double precision", nullable: true),
                    FirstProposalValueCurrncey = table.Column<int>(type: "integer", nullable: true),
                    CurrentProposalValue = table.Column<double>(type: "double precision", nullable: true),
                    CurrentProposalValueCurrncey = table.Column<int>(type: "integer", nullable: true),
                    ContractSignatureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FinalContractValue = table.Column<double>(type: "double precision", nullable: true),
                    FinalContractValueCurrncey = table.Column<int>(type: "integer", nullable: true),
                    RetainerValidatity = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Opportunity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opportunity_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Opportunity_LineOfBusiness_LineOfBusinessId",
                        column: x => x.LineOfBusinessId,
                        principalTable: "LineOfBusiness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectName = table.Column<string>(type: "text", nullable: false),
                    LineOfBusinessId = table.Column<int>(type: "integer", nullable: false),
                    OpportunityId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ContractSignatureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    KickOffDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ClientApprovalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompletionDateScheduled = table.Column<DateOnly>(type: "date", nullable: true),
                    CompletionDateActual = table.Column<DateOnly>(type: "date", nullable: true),
                    RetainerValidatity = table.Column<int>(type: "integer", nullable: true),
                    ProjectMilestoneId = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_LineOfBusiness_LineOfBusinessId",
                        column: x => x.LineOfBusinessId,
                        principalTable: "LineOfBusiness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_Opportunity_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OpportunityId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    ClientStatus = table.Column<int>(type: "integer", nullable: false),
                    LineOfBusinessId = table.Column<int>(type: "integer", nullable: false),
                    ProjectName = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    FirstContactDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FirstProposalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FirstProposalValue = table.Column<double>(type: "double precision", nullable: true),
                    FirstProposalValueCurrncey = table.Column<int>(type: "integer", nullable: true),
                    CurrentProposalValue = table.Column<double>(type: "double precision", nullable: true),
                    CurrentProposalValueCurrncey = table.Column<int>(type: "integer", nullable: true),
                    ContractSignatureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FinalContractValue = table.Column<double>(type: "double precision", nullable: true),
                    FinalProposalValueCurrncey = table.Column<int>(type: "integer", nullable: true),
                    RetainerValidatity = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    HistoryRowCreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_OpportunityHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityHistory_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunityHistory_LineOfBusiness_LineOfBusinessId",
                        column: x => x.LineOfBusinessId,
                        principalTable: "LineOfBusiness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunityHistory_Opportunity_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunityHistory_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectMilestone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    NeedPayment = table.Column<bool>(type: "boolean", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    MilestoneId = table.Column<int>(type: "integer", nullable: false),
                    PaymentValue = table.Column<double>(type: "double precision", nullable: true),
                    PaymentValueCurrency = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DateScheduled = table.Column<DateOnly>(type: "date", nullable: true),
                    DateActual = table.Column<DateOnly>(type: "date", nullable: true),
                    InvoiceId = table.Column<int>(type: "integer", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_ProjectMilestone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMilestone_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectMilestone_Milestone_MilestoneId",
                        column: x => x.MilestoneId,
                        principalTable: "Milestone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMilestone_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ClientId",
                table: "Invoice",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ProjectMilestoneId",
                table: "Invoice",
                column: "ProjectMilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_ClientId",
                table: "Opportunity",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_LineOfBusinessId",
                table: "Opportunity",
                column: "LineOfBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_ProjectId",
                table: "Opportunity",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityHistory_ClientId",
                table: "OpportunityHistory",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityHistory_LineOfBusinessId",
                table: "OpportunityHistory",
                column: "LineOfBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityHistory_OpportunityId",
                table: "OpportunityHistory",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityHistory_ProjectId",
                table: "OpportunityHistory",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ClientId",
                table: "Project",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_LineOfBusinessId",
                table: "Project",
                column: "LineOfBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_OpportunityId",
                table: "Project",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_InvoiceId",
                table: "ProjectMilestone",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_MilestoneId",
                table: "ProjectMilestone",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_ProjectId",
                table: "ProjectMilestone",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_ProjectMilestone_ProjectMilestoneId",
                table: "Invoice",
                column: "ProjectMilestoneId",
                principalTable: "ProjectMilestone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunity_Project_ProjectId",
                table: "Opportunity",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_ProjectMilestone_ProjectMilestoneId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunity_Project_ProjectId",
                table: "Opportunity");

            migrationBuilder.DropTable(
                name: "OpportunityHistory");

            migrationBuilder.DropTable(
                name: "ProjectMilestone");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Opportunity");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Milestone");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LineOfBusiness");

            migrationBuilder.DropColumn(
                name: "IsRetainer",
                table: "LineOfBusiness");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");
        }
    }
}
