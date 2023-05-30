using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Razgrom_v_9._184.Migrations
{
    /// <inheritdoc />
    public partial class dsa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "Rolls",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsInStandard",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Rolls_ItemId",
                table: "Rolls",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rolls_Items_ItemId",
                table: "Rolls",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rolls_Items_ItemId",
                table: "Rolls");

            migrationBuilder.DropIndex(
                name: "IX_Rolls_ItemId",
                table: "Rolls");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Rolls");

            migrationBuilder.DropColumn(
                name: "IsInStandard",
                table: "Items");
        }
    }
}
