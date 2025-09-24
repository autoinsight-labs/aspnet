namespace AutoInsightAPI.Migrations
{
    internal static class MigrationConstants
    {
        // Table Names
        public const string EmployeeInvitesTable = "EmployeeInvites";
        public const string QRCodesTable = "QRCodes";
        public const string QrCodesTable = "QrCodes";
        public const string YardEmployeesTable = "YardEmployees";
        public const string YardVehiclesTable = "YardVehicles";
        
        // Column Names
        public const string YardEmployeeIdColumn = "YardEmployeeId";
        public const string ImageUrlColumn = "ImageUrl";
        public const string EnteredAtColumn = "EnteredAt";
        public const string VehicleIdColumn = "VehicleId";
        public const string AcceptedAtColumn = "AcceptedAt";
        public const string AcceptedByUserIdColumn = "AcceptedByUserId";
        public const string CreatedAtColumn = "CreatedAt";
        public const string EmailColumn = "Email";
        public const string NameColumn = "Name";
        public const string RoleColumn = "Role";
        public const string StatusColumn = "Status";
        public const string TokenColumn = "Token";
        public const string IdColumn = "Id";
        
        // Data Types
        public const string TimestampType = "TIMESTAMP(7)";
        public const string NVarChar2000Type = "NVARCHAR2(2000)";
        public const string NumberType = "NUMBER(10)";
        
        // Table Names for References
        public const string VehiclesTable = "Vehicles";
        
        // Index Names
        public const string IX_EmployeeInvites_YardEmployeeId = "IX_EmployeeInvites_YardEmployeeId";
        public const string IX_QRCodes_VehicleId = "IX_QRCodes_VehicleId";
        public const string IX_QrCodes_VehicleId = "IX_QrCodes_VehicleId";
        
        // Foreign Key Names
        public const string FK_EmployeeInvites_YardEmployees_YardEmployeeId = "FK_EmployeeInvites_YardEmployees_YardEmployeeId";
        public const string FK_QRCodes_Vehicles_VehicleId = "FK_QRCodes_Vehicles_VehicleId";
        public const string FK_QrCodes_Vehicles_VehicleId = "FK_QrCodes_Vehicles_VehicleId";
        
        // Primary Key Names
        public const string PK_QRCodes = "PK_QRCodes";
        public const string PK_QrCodes = "PK_QrCodes";
    }
}
