using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Productprice",
                table: "Products",
                newName: "ProductPrice");

            migrationBuilder.RenameColumn(
                name: "Productname",
                table: "Products",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "Productdescription",
                table: "Products",
                newName: "ProductDescription");

            migrationBuilder.RenameColumn(
                name: "ProductImageurl",
                table: "Products",
                newName: "ProductImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "Products",
                newName: "Productprice");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "Products",
                newName: "Productname");

            migrationBuilder.RenameColumn(
                name: "ProductImageUrl",
                table: "Products",
                newName: "ProductImageurl");

            migrationBuilder.RenameColumn(
                name: "ProductDescription",
                table: "Products",
                newName: "Productdescription");
        }
    }
}
