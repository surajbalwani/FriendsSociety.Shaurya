using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendsSociety.Shaurya.Migrations
{
    /// <inheritdoc />
    public partial class newMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "AspNetRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleID",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
