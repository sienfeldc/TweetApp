using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace com.tweetapp.Model.Migrations
{
    public partial class removedRedundantUserFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIdFK",
                table: "Tweets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserIdFK",
                table: "Tweets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
