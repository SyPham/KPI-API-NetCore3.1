using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class version107 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "KPIs",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "Categories",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "ActionPlans",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ActionPlans",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LateOnUpLoads",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    NotificationID = table.Column<int>(nullable: false),
                    Area = table.Column<string>(nullable: true),
                    DeadLine = table.Column<string>(nullable: true),
                    KPIName = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LateOnUpLoads", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LateOnUpLoads");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ActionPlans");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "KPIs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "ActionPlans",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }
    }
}
