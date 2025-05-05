using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 18, 4, 29, 194, DateTimeKind.Unspecified).AddTicks(7219), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$zahpTT6157xYxbqTvOzvLekv1KvRH1uG0SA6XCvCPQ7PfTTaB4M6K" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 22, "Admin Add new user assigined to a Vendor", "VendorUser.Create" },
                    { 23, "Admin reads users belong to a spicefic Vendor", "VendorUser.Read" },
                    { 24, "Admin update User Belong to a Vendor", "VendorUser.Update" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "PermissionId", "RoleName" },
                values: new object[,]
                {
                    { 37, 22, "Admin" },
                    { 38, 23, "Admin" },
                    { 39, 24, "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 13, 43, 32, 511, DateTimeKind.Unspecified).AddTicks(3487), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$ZJhFEC1HRel6L9xNjPKAi.rJ1i0.LjCJ6/Kp864zHDCw.jr8mJAkK" });
        }
    }
}
