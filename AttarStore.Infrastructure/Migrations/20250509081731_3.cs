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
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Notifications",
                newName: "TargetUserId");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Notifications",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetRole",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 9, 8, 17, 30, 661, DateTimeKind.Unspecified).AddTicks(8345), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$yOhnmkx2F5R3qFu7KAWOku9PL9Elr4I6.8BZaXLYXO3Chm8nqdAWq" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TargetRole",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "TargetUserId",
                table: "Notifications",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Notifications",
                newName: "SentAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 9, 7, 12, 28, 279, DateTimeKind.Unspecified).AddTicks(4098), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$ido2xCnkzLGGFrScgRlHnuMSp8Nl4CFfyWsRhvDrPP30Fl2ggPeqO" });
        }
    }
}
