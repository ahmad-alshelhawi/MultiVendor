using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPermissionManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 8, 39, 58, 566, DateTimeKind.Unspecified).AddTicks(8103), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$Qq276dRXe4dO54plmf.E0.vDDWICutE4sirlWgNxHo9h7i6FGut4a" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 14, "Manage permissions", "Permission.Create" },
                    { 15, "View permissions", "Permission.Read" },
                    { 16, "Edit permissions", "Permission.Update" },
                    { 17, "Remove permissions", "Permission.Delete" }
                });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "PermissionId", "RoleName" },
                values: new object[] { 5, "VendorUser" });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "PermissionId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "PermissionId", "RoleName" },
                values: new object[] { 11, "Client" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "PermissionId", "RoleName" },
                values: new object[,]
                {
                    { 29, 14, "Admin" },
                    { 30, 15, "Admin" },
                    { 31, 16, "Admin" },
                    { 32, 17, "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 8, 27, 3, 450, DateTimeKind.Unspecified).AddTicks(428), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$JFz3YSX.NuIWUp8EkBXhAOsWZkor1eOMt17iV9PGP4xpbPBTp.T2y" });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "PermissionId", "RoleName" },
                values: new object[] { 6, "Client" });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "PermissionId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "PermissionId", "RoleName" },
                values: new object[] { 5, "VendorUser" });
        }
    }
}
