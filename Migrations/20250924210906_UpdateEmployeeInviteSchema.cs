using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoInsightAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeInviteSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeInvites_YardEmployees_YardEmployeeId",
                table: "EmployeeInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_QRCodes_Vehicles_VehicleId",
                table: "QRCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QRCodes",
                table: "QRCodes");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeInvites_YardEmployeeId",
                table: "EmployeeInvites");

            migrationBuilder.DropColumn(
                name: "YardEmployeeId",
                table: "EmployeeInvites");

            migrationBuilder.RenameTable(
                name: "QRCodes",
                newName: "QrCodes");

            migrationBuilder.RenameIndex(
                name: "IX_QRCodes_VehicleId",
                table: "QrCodes",
                newName: "IX_QrCodes_VehicleId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EnteredAt",
                table: "YardVehicles",
                type: "TIMESTAMP(7)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "EmployeeInvites",
                type: "TIMESTAMP(7)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedByUserId",
                table: "EmployeeInvites",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EmployeeInvites",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EmployeeInvites",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "EmployeeInvites",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "EmployeeInvites",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EmployeeInvites",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "EmployeeInvites",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QrCodes",
                table: "QrCodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QrCodes_Vehicles_VehicleId",
                table: "QrCodes",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QrCodes_Vehicles_VehicleId",
                table: "QrCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QrCodes",
                table: "QrCodes");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "EmployeeInvites");

            migrationBuilder.DropColumn(
                name: "AcceptedByUserId",
                table: "EmployeeInvites");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EmployeeInvites");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "EmployeeInvites");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "EmployeeInvites");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "EmployeeInvites");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmployeeInvites");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "EmployeeInvites");

            migrationBuilder.RenameTable(
                name: "QrCodes",
                newName: "QRCodes");

            migrationBuilder.RenameIndex(
                name: "IX_QrCodes_VehicleId",
                table: "QRCodes",
                newName: "IX_QRCodes_VehicleId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EnteredAt",
                table: "YardVehicles",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YardEmployeeId",
                table: "EmployeeInvites",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QRCodes",
                table: "QRCodes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInvites_YardEmployeeId",
                table: "EmployeeInvites",
                column: "YardEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeInvites_YardEmployees_YardEmployeeId",
                table: "EmployeeInvites",
                column: "YardEmployeeId",
                principalTable: "YardEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QRCodes_Vehicles_VehicleId",
                table: "QRCodes",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }
    }
}
