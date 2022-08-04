using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Migrations
{
    public partial class UserProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Projects",
                newName: "Id");

            migrationBuilder.CreateTable(
                name: "ApplicationUserProjectModel",
                columns: table => new
                {
                    ProjectsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserProjectModel", x => new { x.ProjectsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserProjectModel_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserProjectModel_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserProjectModel_UsersId",
                table: "ApplicationUserProjectModel",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserProjectModel");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Projects",
                newName: "ID");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Projects",
                type: "TEXT",
                nullable: true);
        }
    }
}
