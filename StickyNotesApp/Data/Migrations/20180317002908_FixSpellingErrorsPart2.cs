using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace StickyNotesApp.Data.Migrations
{
    public partial class FixSpellingErrorsPart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnderID",
                table: "Todo",
                newName: "OwnerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerID",
                table: "Todo",
                newName: "OwnderID");
        }
    }
}
