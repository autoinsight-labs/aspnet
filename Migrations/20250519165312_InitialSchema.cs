using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoInsightAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        private const string ID_COLUMN = "NVARCHAR2(450)";
        private const string STRING_COLUMN = "NVARCHAR2(2000)";
        private const string DATETIMME_COLUMN = "TIMESTAMP(7)";
        private const string YARD_TABLE = "Yards";
        private const string VEHICLE_TABLE = "Vehicles";
        private const string YARD_EMPLOYEE_TABLE = "YardEmployees";
        private const string QRCODE_TABLE = "QRCodes";
        private const string YARD_VEHICLE_TABLE = "YardVehicles";
        private const string EMPLOYEE_INVITE_TABLE = "EmployeeInvites";
        private const string YARD_ID_COLUMN = "YardId";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    Country = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    State = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    City = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    ZipCode = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    Neighborhood = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    Complement = table.Column<string>(type: STRING_COLUMN, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    OccursAt = table.Column<DateTime>(type: DATETIMME_COLUMN, nullable: false),
                    CancelledAt = table.Column<DateTime>(type: DATETIMME_COLUMN, nullable: true),
                    VehicleId = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    YardId = table.Column<string>(type: STRING_COLUMN, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    Name = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    Year = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: YARD_TABLE,
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    AddressId = table.Column<string>(type: ID_COLUMN, nullable: false),
                    OwnerId = table.Column<string>(type: STRING_COLUMN, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Yards_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: VEHICLE_TABLE,
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    Plate = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    ModelId = table.Column<string>(type: ID_COLUMN, nullable: false),
                    UserId = table.Column<string>(type: STRING_COLUMN, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: YARD_EMPLOYEE_TABLE,
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    Name = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    ImageUrl = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    Role = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UserId = table.Column<string>(type: STRING_COLUMN, nullable: false),
                    YardId = table.Column<string>(type: ID_COLUMN, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YardEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YardEmployees_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: YARD_TABLE,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: QRCODE_TABLE,
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    VehicleId = table.Column<string>(type: ID_COLUMN, nullable: true),
                    YardId = table.Column<string>(type: ID_COLUMN, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRCodes_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: VEHICLE_TABLE,
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QRCodes_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: YARD_TABLE,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: YARD_VEHICLE_TABLE,
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EnteredAt = table.Column<DateTime>(type: DATETIMME_COLUMN, nullable: false),
                    LeftAt = table.Column<DateTime>(type: DATETIMME_COLUMN, nullable: true),
                    VehicleId = table.Column<string>(type: ID_COLUMN, nullable: false),
                    YardId = table.Column<string>(type: ID_COLUMN, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YardVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YardVehicles_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: VEHICLE_TABLE,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YardVehicles_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: YARD_TABLE,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: EMPLOYEE_INVITE_TABLE,
                columns: table => new
                {
                    Id = table.Column<string>(type: ID_COLUMN, nullable: false),
                    YardEmployeeId = table.Column<string>(type: ID_COLUMN, nullable: false),
                    YardId = table.Column<string>(type: ID_COLUMN, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeInvites_YardEmployees_YardEmployeeId",
                        column: x => x.YardEmployeeId,
                        principalTable: YARD_EMPLOYEE_TABLE,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeInvites_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: YARD_TABLE,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInvites_YardEmployeeId",
                table: EMPLOYEE_INVITE_TABLE,
                column: "YardEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInvites_YardId",
                table: EMPLOYEE_INVITE_TABLE,
                column: YARD_ID_COLUMN);

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_VehicleId",
                table: QRCODE_TABLE,
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_YardId",
                table: QRCODE_TABLE,
                column: YARD_ID_COLUMN);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ModelId",
                table: VEHICLE_TABLE,
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_YardEmployees_YardId",
                table: YARD_EMPLOYEE_TABLE,
                column: YARD_ID_COLUMN);

            migrationBuilder.CreateIndex(
                name: "IX_Yards_AddressId",
                table: YARD_TABLE,
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_YardVehicles_VehicleId",
                table: YARD_VEHICLE_TABLE,
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_YardVehicles_YardId",
                table: YARD_VEHICLE_TABLE,
                column: YARD_ID_COLUMN);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: EMPLOYEE_INVITE_TABLE);

            migrationBuilder.DropTable(
                name: QRCODE_TABLE);

            migrationBuilder.DropTable(
                name: YARD_VEHICLE_TABLE);

            migrationBuilder.DropTable(
                name: YARD_EMPLOYEE_TABLE);

            migrationBuilder.DropTable(
                name: VEHICLE_TABLE);

            migrationBuilder.DropTable(
                name: YARD_TABLE);

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
