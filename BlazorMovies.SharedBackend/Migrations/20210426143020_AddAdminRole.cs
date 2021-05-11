using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorMovies.SharedBackend.Migrations
{
    public partial class AddAdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    insert into AspNetRoles (Id, [Name], NormalizedName) 
                    Values('a4b18336-c811-4d3d-ad72-a708e1963eba', 'Admin', 'Admin')
                    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from AspNetRoles");
        }
    }
}
