using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EF_DotNetCore.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 40, nullable: false),
                    Address = table.Column<string>(maxLength: 40, nullable: false),
                    Salary = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LogIn",
                columns: table => new
                {
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "ID", "Address", "DateOfBirth", "Name", "Salary" },
                values: new object[] { 1, "Chawni", new DateTime(2022, 9, 6, 22, 45, 3, 757, DateTimeKind.Local).AddTicks(9145), "Mike", 3000 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "LogIn");
        }
    }
}
