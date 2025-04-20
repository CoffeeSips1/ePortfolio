using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNHU_Capstone_Project.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPermsv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_PermissionSet_PermissionsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PermissionsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PermissionsId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PermissionSet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionSet_Users_Id",
                table: "PermissionSet",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionSet_Users_Id",
                table: "PermissionSet");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PermissionSet");

            migrationBuilder.AddColumn<Guid>(
                name: "PermissionsId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_PermissionsId",
                table: "Users",
                column: "PermissionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PermissionSet_PermissionsId",
                table: "Users",
                column: "PermissionsId",
                principalTable: "PermissionSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
