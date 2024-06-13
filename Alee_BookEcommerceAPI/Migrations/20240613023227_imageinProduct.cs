using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alee_BookEcommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class imageinProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price100",
                table: "Products",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "ImagesLocalPath",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagesUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImagesLocalPath", "ImagesUrl" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImagesLocalPath", "ImagesUrl" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImagesLocalPath", "ImagesUrl" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImagesLocalPath", "ImagesUrl" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ImagesLocalPath", "ImagesUrl" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImagesLocalPath", "ImagesUrl" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagesLocalPath",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImagesUrl",
                table: "Products");

            migrationBuilder.AlterColumn<double>(
                name: "Price100",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
