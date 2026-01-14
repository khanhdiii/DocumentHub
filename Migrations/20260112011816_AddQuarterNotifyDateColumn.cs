using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class AddQuarterNotifyDateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuarterNotifyDate",
                table: "WorkProgresses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuarterNotifyDate",
                table: "WorkProgresses");
        }
    }
}
