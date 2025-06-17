using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentraliaStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Intro to C#", "C# Textbook" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Intro to c#", " c# Textbook" });
        }
    }
}
