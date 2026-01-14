using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkProgressQuater : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkProgressQuaters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Quarter = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkProgressId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkProgressQuaters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkProgressQuaters_WorkProgresses_WorkProgressId",
                        column: x => x.WorkProgressId,
                        principalTable: "WorkProgresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkProgressQuaters_WorkProgressId",
                table: "WorkProgressQuaters",
                column: "WorkProgressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkProgressQuaters");
        }
    }
}
