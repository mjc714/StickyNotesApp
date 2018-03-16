using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace StickyNotesApp.Data.Migrations
{
    public partial class FixSpellingErrors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExprireDate",
                table: "Todo",
                newName: "ExpireDate");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Todo",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpireDate",
                table: "Todo",
                newName: "ExprireDate");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Todo",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
