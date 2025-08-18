using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoInsightAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQRCodeSchema : Migration
    {
        private const string QRCODE_TABLE = "QRCodes";
        private const string YARD_ID_NAME = "YardId";
        
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QRCodes_Yards_YardId",
                table: QRCODE_TABLE);

            migrationBuilder.DropIndex(
                name: "IX_QRCodes_YardId",
                table: QRCODE_TABLE);

            migrationBuilder.DropColumn(
                name: YARD_ID_NAME,
                table: QRCODE_TABLE);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: YARD_ID_NAME,
                table: QRCODE_TABLE,
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_YardId",
                table: QRCODE_TABLE,
                column: YARD_ID_NAME);

            migrationBuilder.AddForeignKey(
                name: "FK_QRCodes_Yards_YardId",
                table: QRCODE_TABLE,
                column: YARD_ID_NAME,
                principalTable: "Yards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
