using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkProgressMonthsYears : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedMonths",
                table: "WorkProgresses");

            migrationBuilder.DropColumn(
                name: "SelectedYears",
                table: "WorkProgresses");

            migrationBuilder.CreateTable(
                name: "WorkProgressMonths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkProgressId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkProgressMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkProgressMonths_WorkProgresses_WorkProgressId",
                        column: x => x.WorkProgressId,
                        principalTable: "WorkProgresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkProgressYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkProgressId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkProgressYears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkProgressYears_WorkProgresses_WorkProgressId",
                        column: x => x.WorkProgressId,
                        principalTable: "WorkProgresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkProgressMonths_WorkProgressId",
                table: "WorkProgressMonths",
                column: "WorkProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkProgressYears_WorkProgressId",
                table: "WorkProgressYears",
                column: "WorkProgressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkProgressMonths");

            migrationBuilder.DropTable(
                name: "WorkProgressYears");

            migrationBuilder.AddColumn<string>(
                name: "SelectedMonths",
                table: "WorkProgresses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedYears",
                table: "WorkProgresses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
