using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 8, 27, 3, 450, DateTimeKind.Unspecified).AddTicks(428), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$JFz3YSX.NuIWUp8EkBXhAOsWZkor1eOMt17iV9PGP4xpbPBTp.T2y" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 7, 58, 9, 993, DateTimeKind.Unspecified).AddTicks(1038), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$zmbIkBatTA1GHJnl1vgHguU66qQ3vNjNG1cKOtpatj/cXgUsRhv1G" });
        }
    }
}
