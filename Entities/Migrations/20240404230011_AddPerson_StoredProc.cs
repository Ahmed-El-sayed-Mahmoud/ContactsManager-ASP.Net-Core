using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class AddPerson_StoredProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string proc = @"
        CREATE PROCEDURE [dbo].[AddPerson]
        (
            @PersonID uniqueidentifier,
            @PersonName nvarchar(40),
            @DateOfBirth datetime2(7),
            @Email nvarchar(50),
            @Gender nvarchar(max),
            @CountryID uniqueidentifier,
            @Country nvarchar(40),
            @Address nvarchar(200),
            @ReceiveNewsLetters bit,
        )
        AS
        BEGIN
            INSERT INTO [dbo].[Persons]
            (
                PersonID,
                PersonName,
                DateOfBirth,
                Email,
                Gender,
                CountryID,
                Country,
                Address,
                ReceiveNewsLetters,
            )
            VALUES
            (
                @PersonID,
                @PersonName,
                @DateOfBirth,
                @Email,
                @Gender,
                @CountryID,
                @Country,
                @Address,
                @ReceiveNewsLetters,
            )
        END";

            migrationBuilder.Sql(proc);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string proc = @"Drop PROCEDURE [dbo].[AddPerson]";
            migrationBuilder.Sql(proc);
        }
    }
}
