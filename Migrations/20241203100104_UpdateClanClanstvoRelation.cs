using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnesClanstvo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClanClanstvoRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clanstvo_ClanId",
                table: "Clanstvo");

            migrationBuilder.CreateIndex(
                name: "IX_Clanstvo_ClanId",
                table: "Clanstvo",
                column: "ClanId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clanstvo_ClanId",
                table: "Clanstvo");

            migrationBuilder.CreateIndex(
                name: "IX_Clanstvo_ClanId",
                table: "Clanstvo",
                column: "ClanId");
        }
    }
}
