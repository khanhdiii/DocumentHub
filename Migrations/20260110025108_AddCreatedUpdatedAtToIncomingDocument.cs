using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedUpdatedAtToIncomingDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_ConstructionStaff_ConstructionStaffId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_ReceivingOfficers_ReceivingOfficerId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_Recipients_RecipientId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_Signers_SignerId",
                table: "IncomingDocuments");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OutgoingDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OutgoingDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SignerId",
                table: "IncomingDocuments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "RecipientId",
                table: "IncomingDocuments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ReceivingOfficerId",
                table: "IncomingDocuments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ConstructionStaffId",
                table: "IncomingDocuments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IncomingDocuments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IncomingDocuments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_ConstructionStaff_ConstructionStaffId",
                table: "IncomingDocuments",
                column: "ConstructionStaffId",
                principalTable: "ConstructionStaff",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_ReceivingOfficers_ReceivingOfficerId",
                table: "IncomingDocuments",
                column: "ReceivingOfficerId",
                principalTable: "ReceivingOfficers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_Recipients_RecipientId",
                table: "IncomingDocuments",
                column: "RecipientId",
                principalTable: "Recipients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_Signers_SignerId",
                table: "IncomingDocuments",
                column: "SignerId",
                principalTable: "Signers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_ConstructionStaff_ConstructionStaffId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_ReceivingOfficers_ReceivingOfficerId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_Recipients_RecipientId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_Signers_SignerId",
                table: "IncomingDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OutgoingDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OutgoingDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IncomingDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IncomingDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "SignerId",
                table: "IncomingDocuments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RecipientId",
                table: "IncomingDocuments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReceivingOfficerId",
                table: "IncomingDocuments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ConstructionStaffId",
                table: "IncomingDocuments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_ConstructionStaff_ConstructionStaffId",
                table: "IncomingDocuments",
                column: "ConstructionStaffId",
                principalTable: "ConstructionStaff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_ReceivingOfficers_ReceivingOfficerId",
                table: "IncomingDocuments",
                column: "ReceivingOfficerId",
                principalTable: "ReceivingOfficers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_Recipients_RecipientId",
                table: "IncomingDocuments",
                column: "RecipientId",
                principalTable: "Recipients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_Signers_SignerId",
                table: "IncomingDocuments",
                column: "SignerId",
                principalTable: "Signers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
