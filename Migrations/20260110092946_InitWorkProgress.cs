using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class InitWorkProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Assigner_FullName = table.Column<string>(type: "TEXT", nullable: false),
                    PersonInCharge_FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<string>(type: "TEXT", nullable: false),
                    Progress = table.Column<int>(type: "INTEGER", nullable: false),
                    ActualCompletionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsMonth = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsYear = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsYearly = table.Column<bool>(type: "INTEGER", nullable: false),
                    YearlyCount = table.Column<int>(type: "INTEGER", nullable: false),
                    NotificationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Is3Months = table.Column<bool>(type: "INTEGER", nullable: false),
                    Is6Months = table.Column<bool>(type: "INTEGER", nullable: false),
                    Is9Months = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSudden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSeminar = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeminarDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SuddenDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkProgresses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkProgresses");
        }
    }
}
