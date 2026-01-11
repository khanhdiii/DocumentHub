using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assigner_FullName",
                table: "WorkProgresses");

            migrationBuilder.DropColumn(
                name: "PersonInCharge_FullName",
                table: "WorkProgresses");

            migrationBuilder.AddColumn<int>(
                name: "AssignerId",
                table: "WorkProgresses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonInChargeId",
                table: "WorkProgresses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkProgresses_AssignerId",
                table: "WorkProgresses",
                column: "AssignerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkProgresses_PersonInChargeId",
                table: "WorkProgresses",
                column: "PersonInChargeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkProgresses_People_AssignerId",
                table: "WorkProgresses",
                column: "AssignerId",
                principalTable: "People",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkProgresses_People_PersonInChargeId",
                table: "WorkProgresses",
                column: "PersonInChargeId",
                principalTable: "People",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkProgresses_People_AssignerId",
                table: "WorkProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkProgresses_People_PersonInChargeId",
                table: "WorkProgresses");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropIndex(
                name: "IX_WorkProgresses_AssignerId",
                table: "WorkProgresses");

            migrationBuilder.DropIndex(
                name: "IX_WorkProgresses_PersonInChargeId",
                table: "WorkProgresses");

            migrationBuilder.DropColumn(
                name: "AssignerId",
                table: "WorkProgresses");

            migrationBuilder.DropColumn(
                name: "PersonInChargeId",
                table: "WorkProgresses");

            migrationBuilder.AddColumn<string>(
                name: "Assigner_FullName",
                table: "WorkProgresses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PersonInCharge_FullName",
                table: "WorkProgresses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
