using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendsSociety.Shaurya.Migrations
{
    /// <inheritdoc />
    public partial class teamAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leader",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamAssignments",
                columns: table => new
                {
                    TeamAssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeaderID = table.Column<int>(type: "int", nullable: false),
                    MemberIDs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroundID = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAssignments", x => x.TeamAssignmentID);
                    table.ForeignKey(
                        name: "FK_TeamAssignments_Grounds_GroundID",
                        column: x => x.GroundID,
                        principalTable: "Grounds",
                        principalColumn: "GroundID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TeamAssignments_Leader_LeaderID",
                        column: x => x.LeaderID,
                        principalTable: "Leader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamAssignments_GroundID",
                table: "TeamAssignments",
                column: "GroundID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAssignments_LeaderID",
                table: "TeamAssignments",
                column: "LeaderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamAssignments");

            migrationBuilder.DropTable(
                name: "Leader");
        }
    }
}
