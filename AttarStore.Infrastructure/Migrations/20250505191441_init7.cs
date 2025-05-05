using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "ProductImages");

            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "ProductVariants",
                newName: "Sku");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ProductVariants",
                newName: "Stock");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariants_ProductId_SKU",
                table: "ProductVariants",
                newName: "IX_ProductVariants_ProductId_Sku");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 19, 14, 40, 275, DateTimeKind.Unspecified).AddTicks(2196), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$U/F8KsXzsboCB3JomyKf2O.WmFNFAfEgZQ/ygbO9U3ku3qDF8j8eW" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "ProductVariants",
                newName: "SKU");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "ProductVariants",
                newName: "Quantity");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariants_ProductId_Sku",
                table: "ProductVariants",
                newName: "IX_ProductVariants_ProductId_SKU");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "ProductImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 5, 5, 18, 4, 29, 194, DateTimeKind.Unspecified).AddTicks(7219), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$zahpTT6157xYxbqTvOzvLekv1KvRH1uG0SA6XCvCPQ7PfTTaB4M6K" });
        }
    }
}
