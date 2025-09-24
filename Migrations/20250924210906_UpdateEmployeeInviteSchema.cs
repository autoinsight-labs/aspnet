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
                name: MigrationConstants.FK_EmployeeInvites_YardEmployees_YardEmployeeId,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropForeignKey(
                name: MigrationConstants.FK_QRCodes_Vehicles_VehicleId,
                table: MigrationConstants.QRCodesTable);

            migrationBuilder.DropPrimaryKey(
                name: MigrationConstants.PK_QRCodes,
                table: MigrationConstants.QRCodesTable);

            migrationBuilder.DropIndex(
                name: MigrationConstants.IX_EmployeeInvites_YardEmployeeId,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.YardEmployeeIdColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.RenameTable(
                name: MigrationConstants.QRCodesTable,
                newName: MigrationConstants.QrCodesTable);

            migrationBuilder.RenameIndex(
                name: MigrationConstants.IX_QRCodes_VehicleId,
                table: MigrationConstants.QrCodesTable,
                newName: MigrationConstants.IX_QrCodes_VehicleId);

            migrationBuilder.AlterColumn<DateTime>(
                name: MigrationConstants.EnteredAtColumn,
                table: MigrationConstants.YardVehiclesTable,
                type: MigrationConstants.TimestampType,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: MigrationConstants.TimestampType);

            migrationBuilder.AddColumn<DateTime>(
                name: MigrationConstants.AcceptedAtColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: MigrationConstants.TimestampType,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: MigrationConstants.AcceptedByUserIdColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: MigrationConstants.NVarChar2000Type,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: MigrationConstants.CreatedAtColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: MigrationConstants.TimestampType,
                nullable: false,
                defaultValue: DateTime.MinValue);

            migrationBuilder.AddColumn<string>(
                name: MigrationConstants.EmailColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: MigrationConstants.NVarChar2000Type,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: MigrationConstants.NameColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: MigrationConstants.NVarChar2000Type,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: MigrationConstants.RoleColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: MigrationConstants.NumberType,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: MigrationConstants.StatusColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: MigrationConstants.NumberType,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: MigrationConstants.TokenColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: MigrationConstants.NVarChar2000Type,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: MigrationConstants.PK_QrCodes,
                table: MigrationConstants.QrCodesTable,
                column: MigrationConstants.IdColumn);

            migrationBuilder.AddForeignKey(
                name: MigrationConstants.FK_QrCodes_Vehicles_VehicleId,
                table: MigrationConstants.QrCodesTable,
                column: MigrationConstants.VehicleIdColumn,
                principalTable: MigrationConstants.VehiclesTable,
                principalColumn: MigrationConstants.IdColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: MigrationConstants.FK_QrCodes_Vehicles_VehicleId,
                table: MigrationConstants.QrCodesTable);

            migrationBuilder.DropPrimaryKey(
                name: MigrationConstants.PK_QrCodes,
                table: MigrationConstants.QrCodesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.AcceptedAtColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.AcceptedByUserIdColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.CreatedAtColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.EmailColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.NameColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.RoleColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.StatusColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.DropColumn(
                name: MigrationConstants.TokenColumn,
                table: MigrationConstants.EmployeeInvitesTable);

            migrationBuilder.RenameTable(
                name: MigrationConstants.QrCodesTable,
                newName: MigrationConstants.QRCodesTable);

            migrationBuilder.RenameIndex(
                name: MigrationConstants.IX_QrCodes_VehicleId,
                table: MigrationConstants.QRCodesTable,
                newName: MigrationConstants.IX_QRCodes_VehicleId);

            migrationBuilder.AlterColumn<DateTime>(
                name: MigrationConstants.EnteredAtColumn,
                table: MigrationConstants.YardVehiclesTable,
                type: MigrationConstants.TimestampType,
                nullable: false,
                defaultValue: DateTime.MinValue,
                oldClrType: typeof(DateTime),
                oldType: MigrationConstants.TimestampType,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: MigrationConstants.YardEmployeeIdColumn,
                table: MigrationConstants.EmployeeInvitesTable,
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: MigrationConstants.PK_QRCodes,
                table: MigrationConstants.QRCodesTable,
                column: MigrationConstants.IdColumn);

            migrationBuilder.CreateIndex(
                name: MigrationConstants.IX_EmployeeInvites_YardEmployeeId,
                table: MigrationConstants.EmployeeInvitesTable,
                column: MigrationConstants.YardEmployeeIdColumn);

            migrationBuilder.AddForeignKey(
                name: MigrationConstants.FK_EmployeeInvites_YardEmployees_YardEmployeeId,
                table: MigrationConstants.EmployeeInvitesTable,
                column: MigrationConstants.YardEmployeeIdColumn,
                principalTable: MigrationConstants.YardEmployeesTable,
                principalColumn: MigrationConstants.IdColumn,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: MigrationConstants.FK_QRCodes_Vehicles_VehicleId,
                table: MigrationConstants.QRCodesTable,
                column: MigrationConstants.VehicleIdColumn,
                principalTable: MigrationConstants.VehiclesTable,
                principalColumn: MigrationConstants.IdColumn);
        }
    }
}
