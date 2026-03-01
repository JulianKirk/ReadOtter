using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadOtter.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToSettingName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_SettingName",
                table: "AppSettings",
                column: "SettingName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppSettings_SettingName",
                table: "AppSettings");
        }
    }
}
