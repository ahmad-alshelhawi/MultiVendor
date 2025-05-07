using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CategoryRequests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddColumn<string>(
                name: "ResponseMessage",
                table: "CategoryRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 7, 18, 18, 24, 704, DateTimeKind.Unspecified).AddTicks(6425), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$ZgNeyzhGsSCOw6mgoNkKRe.WD2EQaauyrPjMP2FPUbSCrdyzSOoI2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseMessage",
                table: "CategoryRequests");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "CategoryRequests",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 7, 13, 0, 37, 896, DateTimeKind.Unspecified).AddTicks(4513), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$y7iquski0b3maZscPkztbO56s8g1in9nWrkdQfZPTH2R0OPqYc6/2" });
        }
    }
}
