using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthYearNotifyDateToWorkProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MonthNotifyDate",
                table: "WorkProgresses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "YearNotifyDate",
                table: "WorkProgresses",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthNotifyDate",
                table: "WorkProgresses");

            migrationBuilder.DropColumn(
                name: "YearNotifyDate",
                table: "WorkProgresses");
        }
    }
}
