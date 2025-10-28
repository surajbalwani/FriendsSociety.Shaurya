using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendsSociety.Shaurya.Migrations
{
    /// <inheritdoc />
    public partial class AddGamesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameCodeNumber = table.Column<int>(type: "int", nullable: false),
                    DisabilityTypeCode = table.Column<int>(type: "int", nullable: false),
                    AgeCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgeRangeStart = table.Column<int>(type: "int", nullable: false),
                    AgeRangeEnd = table.Column<int>(type: "int", nullable: false),
                    AbilityTypeID = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameID);
                    table.ForeignKey(
                        name: "FK_Games_AbilityTypes_AbilityTypeID",
                        column: x => x.AbilityTypeID,
                        principalTable: "AbilityTypes",
                        principalColumn: "AbilityTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_AbilityTypeID",
                table: "Games",
                column: "AbilityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameCode",
                table: "Games",
                column: "GameCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
