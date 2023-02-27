using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class project_and_milestone_adjustments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectMilestoneId",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "ProjectMilestone",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "Project",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "KickOffDate",
                table: "Project",
                newName: "KickOffDateScheduled");

            migrationBuilder.RenameColumn(
                name: "ClientApprovalDate",
                table: "Project",
                newName: "KickOffDateActual");

            migrationBuilder.AddColumn<bool>(
                name: "End",
                table: "ProjectMilestone",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MilestoneIndex",
                table: "ProjectMilestone",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Start",
                table: "ProjectMilestone",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentProjectMilestoneId",
                table: "Project",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentProjectMilestoneIndex",
                table: "Project",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MilestoneCount",
                table: "Project",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "End",
                table: "ProjectMilestone");

            migrationBuilder.DropColumn(
                name: "MilestoneIndex",
                table: "ProjectMilestone");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "ProjectMilestone");

            migrationBuilder.DropColumn(
                name: "CurrentProjectMilestoneId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CurrentProjectMilestoneIndex",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "MilestoneCount",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "ProjectMilestone",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Project",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "KickOffDateScheduled",
                table: "Project",
                newName: "KickOffDate");

            migrationBuilder.RenameColumn(
                name: "KickOffDateActual",
                table: "Project",
                newName: "ClientApprovalDate");

            migrationBuilder.AddColumn<int>(
                name: "ProjectMilestoneId",
                table: "Project",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
