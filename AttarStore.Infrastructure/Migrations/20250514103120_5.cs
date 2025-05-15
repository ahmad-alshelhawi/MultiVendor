using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 14, 10, 31, 20, 76, DateTimeKind.Unspecified).AddTicks(3122), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$EhWfilJGg8QCA5p0wLsrPeebtldwd3J0/vAhbQIuV/LoFeDi4IpAC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 13, 11, 17, 47, 602, DateTimeKind.Unspecified).AddTicks(753), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$77KBwDHDXBlyHLCzBW2irO0UBDP1MHq4DFaHJkdnzUs1Y2a39BFeW" });
        }
    }
}
