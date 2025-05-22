using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoInsightAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQRCodeSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QRCodes_Yards_YardId",
                table: "QRCodes");

            migrationBuilder.DropIndex(
                name: "IX_QRCodes_YardId",
                table: "QRCodes");

            migrationBuilder.DropColumn(
                name: "YardId",
                table: "QRCodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YardId",
                table: "QRCodes",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_YardId",
                table: "QRCodes",
                column: "YardId");

            migrationBuilder.AddForeignKey(
                name: "FK_QRCodes_Yards_YardId",
                table: "QRCodes",
                column: "YardId",
                principalTable: "Yards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
