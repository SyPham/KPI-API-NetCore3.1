using Microsoft.EntityFrameworkCore.Migrations;

namespace Models.Migrations
{
    public partial class version102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Revises");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "Permission",
                table: "Menus");

            migrationBuilder.AddColumn<int>(
                name: "RoleID",
                table: "Menus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MenuRoles",
                columns: table => new
                {
                    MenuID = table.Column<int>(nullable: false),
                    RoleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuRoles", x => new { x.RoleID, x.MenuID });
                    table.ForeignKey(
                        name: "FK_MenuRoles_Menus_MenuID",
                        column: x => x.MenuID,
                        principalTable: "Menus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuRoles_MenuID",
                table: "MenuRoles",
                column: "MenuID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuRoles");

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "Menus");

            migrationBuilder.AddColumn<int>(
                name: "ParentID",
                table: "Menus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Permission",
                table: "Menus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Revises",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KPILevelCodePeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeriodValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Revises", x => x.ID);
                });
        }
    }
}
