using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Restaurant.Data.Migrations
{
    public partial class AddMenuItemsToOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderHeaderId",
                table: "MenuItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_OrderHeaderId",
                table: "MenuItems",
                column: "OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_OrderHeaders_OrderHeaderId",
                table: "MenuItems",
                column: "OrderHeaderId",
                principalTable: "OrderHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_OrderHeaders_OrderHeaderId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_OrderHeaderId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "OrderHeaderId",
                table: "MenuItems");
        }
    }
}
