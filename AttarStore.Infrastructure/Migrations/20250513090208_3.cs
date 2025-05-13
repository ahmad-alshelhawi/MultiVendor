using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 13, 9, 2, 8, 278, DateTimeKind.Unspecified).AddTicks(7552), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$8LEapvTlESSly52.rYVLM.G5LTHWuRFKMH.25heLVTKf0NX5Pg7lS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 13, 6, 54, 55, 535, DateTimeKind.Unspecified).AddTicks(5667), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$P0kErq9sprygyc1V7.pIYO.HNCq3OcHmesWnZM7gG6.qN58/Ih8Wq" });
        }
    }
}
