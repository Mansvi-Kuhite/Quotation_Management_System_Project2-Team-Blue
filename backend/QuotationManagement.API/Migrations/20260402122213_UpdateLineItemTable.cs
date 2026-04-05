using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuotationManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLineItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemDescription",
                table: "LineItems",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "LineItemId",
                table: "LineItems",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "LineItems",
                newName: "ItemDescription");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LineItems",
                newName: "LineItemId");
        }
    }
}
