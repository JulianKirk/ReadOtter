using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadOtter.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddLastOpenedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastOpenedAt",
                table: "Books",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastOpenedAt",
                table: "Books");
        }
    }
}
