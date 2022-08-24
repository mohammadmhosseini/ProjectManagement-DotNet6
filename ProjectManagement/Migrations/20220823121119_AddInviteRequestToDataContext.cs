using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Migrations
{
    public partial class AddInviteRequestToDataContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InviteRequest_Users_UserId",
                table: "InviteRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InviteRequest",
                table: "InviteRequest");

            migrationBuilder.RenameTable(
                name: "InviteRequest",
                newName: "InviteRequests");

            migrationBuilder.RenameIndex(
                name: "IX_InviteRequest_UserId",
                table: "InviteRequests",
                newName: "IX_InviteRequests_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InviteRequests",
                table: "InviteRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InviteRequests_Users_UserId",
                table: "InviteRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InviteRequests_Users_UserId",
                table: "InviteRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InviteRequests",
                table: "InviteRequests");

            migrationBuilder.RenameTable(
                name: "InviteRequests",
                newName: "InviteRequest");

            migrationBuilder.RenameIndex(
                name: "IX_InviteRequests_UserId",
                table: "InviteRequest",
                newName: "IX_InviteRequest_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InviteRequest",
                table: "InviteRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InviteRequest_Users_UserId",
                table: "InviteRequest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
