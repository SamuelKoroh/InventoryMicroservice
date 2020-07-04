using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityService.Migrations
{
    public partial class SeedUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "79f83df0-7f13-416f-9acb-149576f3bd52", "d62e7153-acb3-4cfa-8a43-a557b9eae9bf", "InventoryKeeper", "INVENTORYKEEPER" },
                    { "4a954103-165f-4e41-9de8-03b6872e4d18", "59308c6f-2617-482c-bf70-a0caca93d82d", "Requester", "REQUESTER" },
                    { "0dd662cc-e4bc-4795-9a08-9098f708f0b7", "eea8ccbf-2454-4e2e-80ef-84ae4a1faffb", "Approver", "APPROVER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0dd662cc-e4bc-4795-9a08-9098f708f0b7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a954103-165f-4e41-9de8-03b6872e4d18");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "79f83df0-7f13-416f-9acb-149576f3bd52");
        }
    }
}
