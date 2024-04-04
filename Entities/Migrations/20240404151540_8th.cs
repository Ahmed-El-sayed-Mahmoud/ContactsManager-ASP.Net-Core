using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class _8th : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("89e5f445-d89f-4e12-94e0-5ad5b235d704"),
                column: "Gender",
                value: "Female");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("89e5f445-d89f-4e12-94e0-5ad5b235d704"),
                column: "Gender",
                value: "Gender");
        }
    }
}
