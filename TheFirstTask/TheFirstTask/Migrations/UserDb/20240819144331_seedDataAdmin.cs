using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheFirstTask.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class seedDataAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9a99d1dd-ecc5-4007-b9da-b4417c86e378", null, "admin", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "bda40279-03eb-4d5e-b2b3-543223c7ee64", 0, "481b62f2-1ee1-4e63-ac43-666d160ce727", "admin@gmail.com", false, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEPvzK+a1QxPA8UIYb/0vrHbXaRlQGWnLdFAbfRQiA3dPCVmwiAgHIq2KM9yKOk9hsg==", null, false, "", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "9a99d1dd-ecc5-4007-b9da-b4417c86e378", "bda40279-03eb-4d5e-b2b3-543223c7ee64" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "9a99d1dd-ecc5-4007-b9da-b4417c86e378", "bda40279-03eb-4d5e-b2b3-543223c7ee64" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a99d1dd-ecc5-4007-b9da-b4417c86e378");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bda40279-03eb-4d5e-b2b3-543223c7ee64");
        }
    }
}
