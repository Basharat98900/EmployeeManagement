using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EF_DotNetCore.Migrations
{
    public partial class PhotoPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "DateOfBirth",
                table: "Employees",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Employees");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Employees",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "ID", "Address", "DateOfBirth", "Name", "Salary" },
                values: new object[] { 1, "Chawni", new DateTime(2022, 9, 6, 22, 45, 3, 757, DateTimeKind.Local).AddTicks(9145), "Mike", 3000 });
        }
    }
}
