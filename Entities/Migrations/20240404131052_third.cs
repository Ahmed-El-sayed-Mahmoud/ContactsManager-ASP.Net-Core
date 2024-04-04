using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryID",
                keyValue: new Guid("12e15727-d369-49a9-8b13-bc22e9362179"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryID",
                keyValue: new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryID",
                keyValue: new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryID",
                keyValue: new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryID",
                keyValue: new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryID", "CountryName" },
                values: new object[,]
                {
                    { new Guid("12e15727-d369-49a9-8b13-bc22e9362179"), "China" },
                    { new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"), "Philippines" },
                    { new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"), "China" },
                    { new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"), "Thailand" },
                    { new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"), "Palestinian Territory" }
                });
        }
    }
}
