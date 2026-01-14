using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class AddIs11MonthToWorkProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is11Months",
                table: "WorkProgresses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is11Months",
                table: "WorkProgresses");
        }
    }
}
