using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class version101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "NotificationDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "NotificationDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "NotificationDetails");

            migrationBuilder.DropColumn(
                name: "URL",
                table: "NotificationDetails");
        }
    }
}
