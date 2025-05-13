using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 13, 11, 17, 47, 602, DateTimeKind.Unspecified).AddTicks(753), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$77KBwDHDXBlyHLCzBW2irO0UBDP1MHq4DFaHJkdnzUs1Y2a39BFeW" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 13, 9, 2, 8, 278, DateTimeKind.Unspecified).AddTicks(7552), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$8LEapvTlESSly52.rYVLM.G5LTHWuRFKMH.25heLVTKf0NX5Pg7lS" });
        }
    }
}
