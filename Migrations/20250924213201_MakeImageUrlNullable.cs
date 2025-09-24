using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoInsightAPI.Migrations
{
    /// <inheritdoc />
    public partial class MakeImageUrlNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: MigrationConstants.ImageUrlColumn,
                table: MigrationConstants.YardEmployeesTable,
                type: MigrationConstants.NVarChar2000Type,
                nullable: true,
                oldClrType: typeof(string),
                oldType: MigrationConstants.NVarChar2000Type);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: MigrationConstants.ImageUrlColumn,
                table: MigrationConstants.YardEmployeesTable,
                type: MigrationConstants.NVarChar2000Type,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: MigrationConstants.NVarChar2000Type,
                oldNullable: true);
        }
    }
}
