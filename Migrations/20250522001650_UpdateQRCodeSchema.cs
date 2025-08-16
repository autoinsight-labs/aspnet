using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoInsightAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQRCodeSchema : Migration
    {
        private const string TableName = "QRCodes";
        private const string YardIdColumn = "YardId";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: $"FK_{TableName}_Yards_{YardIdColumn}",
                table: TableName);

            migrationBuilder.DropIndex(
                name: $"IX_{TableName}_{YardIdColumn}",
                table: TableName);

            migrationBuilder.DropColumn(
                name: YardIdColumn,
                table: TableName);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: YardIdColumn,
                table: TableName,
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: $"IX_{TableName}_{YardIdColumn}",
                table: TableName,
                column: YardIdColumn);

            migrationBuilder.AddForeignKey(
                name: $"FK_{TableName}_Yards_{YardIdColumn}",
                table: TableName,
                column: YardIdColumn,
                principalTable: "Yards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
