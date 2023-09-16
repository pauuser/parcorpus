using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parcorpus.DataAccess.Context.Migrations
{
    /// <inheritdoc />
    public partial class SearchHistoryFixFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchHistory_Users_UserNavigationUserId",
                table: "SearchHistory");

            migrationBuilder.DropIndex(
                name: "IX_SearchHistory_UserNavigationUserId",
                table: "SearchHistory");

            migrationBuilder.DropColumn(
                name: "UserNavigationUserId",
                table: "SearchHistory");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistory_UserId",
                table: "SearchHistory",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchHistory_Users_UserId",
                table: "SearchHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchHistory_Users_UserId",
                table: "SearchHistory");

            migrationBuilder.DropIndex(
                name: "IX_SearchHistory_UserId",
                table: "SearchHistory");

            migrationBuilder.AddColumn<Guid>(
                name: "UserNavigationUserId",
                table: "SearchHistory",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistory_UserNavigationUserId",
                table: "SearchHistory",
                column: "UserNavigationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchHistory_Users_UserNavigationUserId",
                table: "SearchHistory",
                column: "UserNavigationUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
