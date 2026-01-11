using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConstructionStaff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructionStaff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceivingOfficers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivingOfficers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Signers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PIN = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    SecurityQuestion1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SecurityAnswer1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SecurityQuestion2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SecurityAnswer2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SecondaryPassword = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomingDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArrivalNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DocumentNumber = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SecurityLevel = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", nullable: false),
                    Sender = table.Column<string>(type: "TEXT", nullable: false),
                    SignerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: false),
                    RecipientId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceivingOfficerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ConstructionStaffId = table.Column<int>(type: "INTEGER", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomingDocuments_ConstructionStaff_ConstructionStaffId",
                        column: x => x.ConstructionStaffId,
                        principalTable: "ConstructionStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingDocuments_ReceivingOfficers_ReceivingOfficerId",
                        column: x => x.ReceivingOfficerId,
                        principalTable: "ReceivingOfficers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingDocuments_Recipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Recipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingDocuments_Signers_SignerId",
                        column: x => x.SignerId,
                        principalTable: "Signers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentNumber = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DocumentType = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: false),
                    SecurityLevel = table.Column<string>(type: "TEXT", nullable: false),
                    ConstructionStaffId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReceivingOfficerId = table.Column<int>(type: "INTEGER", nullable: true),
                    SignerId = table.Column<int>(type: "INTEGER", nullable: true),
                    RecipientId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingDocuments_ConstructionStaff_ConstructionStaffId",
                        column: x => x.ConstructionStaffId,
                        principalTable: "ConstructionStaff",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutgoingDocuments_ReceivingOfficers_ReceivingOfficerId",
                        column: x => x.ReceivingOfficerId,
                        principalTable: "ReceivingOfficers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutgoingDocuments_Recipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Recipients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutgoingDocuments_Signers_SignerId",
                        column: x => x.SignerId,
                        principalTable: "Signers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocuments_ConstructionStaffId",
                table: "IncomingDocuments",
                column: "ConstructionStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocuments_ReceivingOfficerId",
                table: "IncomingDocuments",
                column: "ReceivingOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocuments_RecipientId",
                table: "IncomingDocuments",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocuments_SignerId",
                table: "IncomingDocuments",
                column: "SignerId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocuments_ConstructionStaffId",
                table: "OutgoingDocuments",
                column: "ConstructionStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocuments_ReceivingOfficerId",
                table: "OutgoingDocuments",
                column: "ReceivingOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocuments_RecipientId",
                table: "OutgoingDocuments",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocuments_SignerId",
                table: "OutgoingDocuments",
                column: "SignerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingDocuments");

            migrationBuilder.DropTable(
                name: "OutgoingDocuments");

            migrationBuilder.DropTable(
                name: "UserCredentials");

            migrationBuilder.DropTable(
                name: "ConstructionStaff");

            migrationBuilder.DropTable(
                name: "ReceivingOfficers");

            migrationBuilder.DropTable(
                name: "Recipients");

            migrationBuilder.DropTable(
                name: "Signers");
        }
    }
}
