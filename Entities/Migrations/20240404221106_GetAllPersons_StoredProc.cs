using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class GetAllPersons_StoredProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"CREATE PROCEDURE [dbo].[GetAllPersons] As
                                      begin
                                      select * from [dbo].[Persons]
                                      end";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"DROP PROCEDURE [dbo].[GetAllPersons]";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
