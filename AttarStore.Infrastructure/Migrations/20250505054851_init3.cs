using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 5, 48, 50, 677, DateTimeKind.Unspecified).AddTicks(2761), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$2aPSsKXHlTUaQgGbBKtg/e7ysa5FKCrK271opJsTYd1p7JBSRq82S" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 5, 45, 25, 234, DateTimeKind.Unspecified).AddTicks(3740), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$ABCDEFGHijklmnopqrstuvwxyz1234567890" });
        }
    }
}
