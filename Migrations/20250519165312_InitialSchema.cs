using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoInsightAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        private const string IdColumnType = "NVARCHAR2(450)";
        private const string StringColumnType = "NVARCHAR2(2000)";
        private const string DateTimeColumnType = "TIMESTAMP(7)";
        private const string YardsTableName = "Yards";
        private const string VehiclesTableName = "Vehicles";
        private const string YardEmployeesTableName = "YardEmployees";
        private const string QRCodesTableName = "QRCodes";
        private const string YardVehiclesTableName = "YardVehicles";
        private const string EmployeeInvitesTableName = "EmployeeInvites";
        private const string YardIdColumnName = "YardId";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    Country = table.Column<string>(type: StringColumnType, nullable: false),
                    State = table.Column<string>(type: StringColumnType, nullable: false),
                    City = table.Column<string>(type: StringColumnType, nullable: false),
                    ZipCode = table.Column<string>(type: StringColumnType, nullable: false),
                    Neighborhood = table.Column<string>(type: StringColumnType, nullable: false),
                    Complement = table.Column<string>(type: StringColumnType, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    OccursAt = table.Column<DateTime>(type: DateTimeColumnType, nullable: false),
                    CancelledAt = table.Column<DateTime>(type: DateTimeColumnType, nullable: true),
                    VehicleId = table.Column<string>(type: StringColumnType, nullable: false),
                    YardId = table.Column<string>(type: StringColumnType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    Name = table.Column<string>(type: StringColumnType, nullable: false),
                    Year = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: YardsTableName,
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    AddressId = table.Column<string>(type: IdColumnType, nullable: false),
                    OwnerId = table.Column<string>(type: StringColumnType, nullable: false)
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
                name: VehiclesTableName,
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    Plate = table.Column<string>(type: StringColumnType, nullable: false),
                    ModelId = table.Column<string>(type: IdColumnType, nullable: false),
                    UserId = table.Column<string>(type: StringColumnType, nullable: false)
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
                name: YardEmployeesTableName,
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    Name = table.Column<string>(type: StringColumnType, nullable: false),
                    ImageUrl = table.Column<string>(type: StringColumnType, nullable: false),
                    Role = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UserId = table.Column<string>(type: StringColumnType, nullable: false),
                    YardId = table.Column<string>(type: IdColumnType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YardEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YardEmployees_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: YardsTableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: QRCodesTableName,
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    VehicleId = table.Column<string>(type: IdColumnType, nullable: true),
                    YardId = table.Column<string>(type: IdColumnType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRCodes_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: VehiclesTableName,
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QRCodes_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: YardsTableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: YardVehiclesTableName,
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EnteredAt = table.Column<DateTime>(type: DateTimeColumnType, nullable: false),
                    LeftAt = table.Column<DateTime>(type: DateTimeColumnType, nullable: true),
                    VehicleId = table.Column<string>(type: IdColumnType, nullable: false),
                    YardId = table.Column<string>(type: IdColumnType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YardVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YardVehicles_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: VehiclesTableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YardVehicles_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: YardsTableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: EmployeeInvitesTableName,
                columns: table => new
                {
                    Id = table.Column<string>(type: IdColumnType, nullable: false),
                    YardEmployeeId = table.Column<string>(type: IdColumnType, nullable: false),
                    YardId = table.Column<string>(type: IdColumnType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeInvites_YardEmployees_YardEmployeeId",
                        column: x => x.YardEmployeeId,
                        principalTable: YardEmployeesTableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeInvites_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: YardsTableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInvites_YardEmployeeId",
                table: EmployeeInvitesTableName,
                column: "YardEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInvites_YardId",
                table: EmployeeInvitesTableName,
                column: YardIdColumnName);

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_VehicleId",
                table: QRCodesTableName,
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_YardId",
                table: QRCodesTableName,
                column: YardIdColumnName);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ModelId",
                table: VehiclesTableName,
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_YardEmployees_YardId",
                table: YardEmployeesTableName,
                column: YardIdColumnName);

            migrationBuilder.CreateIndex(
                name: "IX_Yards_AddressId",
                table: YardsTableName,
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_YardVehicles_VehicleId",
                table: YardVehiclesTableName,
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_YardVehicles_YardId",
                table: YardVehiclesTableName,
                column: YardIdColumnName);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: EmployeeInvitesTableName);

            migrationBuilder.DropTable(
                name: QRCodesTableName);

            migrationBuilder.DropTable(
                name: YardVehiclesTableName);

            migrationBuilder.DropTable(
                name: YardEmployeesTableName);

            migrationBuilder.DropTable(
                name: VehiclesTableName);

            migrationBuilder.DropTable(
                name: YardsTableName);

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
