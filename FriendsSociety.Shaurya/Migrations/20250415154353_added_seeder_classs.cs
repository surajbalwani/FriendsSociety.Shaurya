using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FriendsSociety.Shaurya.Migrations
{
    /// <inheritdoc />
    public partial class added_seeder_classs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AbilityTypes",
                columns: new[] { "AbilityTypeID", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, "Partial or total inability to hear", false, "Hearing Impairment" },
                    { 2, "Partial or total inability to see", false, "Visual Impairment" },
                    { 3, "Difficulty walking or moving", false, "Mobility Impairment" }
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "ActivityID", "IsDeleted", "Name", "Rules" },
                values: new object[,]
                {
                    { 1, false, "Wheelchair Basketball", "Standard 5v5 rules apply" },
                    { 2, false, "Blind Running", "Tethered guide required" }
                });

            migrationBuilder.InsertData(
                table: "Grounds",
                columns: new[] { "GroundID", "Location", "Name" },
                values: new object[,]
                {
                    { 1, "City Sports Complex", "Main Arena" },
                    { 2, "Community Park", "Open Ground" }
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "OrganizationID", "Contact", "IsDeleted", "Name" },
                values: new object[] { 1, "hope@example.org", false, "Hope Foundation" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleID", "Name", "Permissions" },
                values: new object[,]
                {
                    { 1, "Participant", "ViewActivities" },
                    { 2, "Volunteer", "ManageActivities,HelpParticipants" }
                });

            migrationBuilder.InsertData(
                table: "ActivityCategories",
                columns: new[] { "ActivityCategoryID", "AbilityTypeID", "ActivityID", "ExclusionType" },
                values: new object[] { 1, 2, 2, null });

            migrationBuilder.InsertData(
                table: "GroundAllocations",
                columns: new[] { "GroundAllocationID", "ActivityID", "EndTime", "GroundID", "StartTime" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, new DateTime(2025, 5, 1, 14, 30, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2025, 5, 1, 13, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "AbilityTypeID", "Age", "Contact", "IsDeleted", "Name", "OrganizationID", "RoleID" },
                values: new object[,]
                {
                    { 1, 1, 24, "arjun@example.com", false, "Arjun Mehta", 1, 1 },
                    { 2, 2, 30, "nikita@example.com", false, "Nikita Shah", 1, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AbilityTypes",
                keyColumn: "AbilityTypeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ActivityCategories",
                keyColumn: "ActivityCategoryID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GroundAllocations",
                keyColumn: "GroundAllocationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GroundAllocations",
                keyColumn: "GroundAllocationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AbilityTypes",
                keyColumn: "AbilityTypeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AbilityTypes",
                keyColumn: "AbilityTypeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Activities",
                keyColumn: "ActivityID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Activities",
                keyColumn: "ActivityID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Grounds",
                keyColumn: "GroundID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Grounds",
                keyColumn: "GroundID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "OrganizationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 2);
        }
    }
}
