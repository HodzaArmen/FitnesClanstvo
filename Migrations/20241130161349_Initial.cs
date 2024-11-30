using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnesClanstvo.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clani",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priimek = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumRojstva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clani", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vadbe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<int>(type: "int", nullable: false),
                    DatumInUra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kapaciteta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vadbe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clanstva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tip = table.Column<int>(type: "int", nullable: true),
                    Zacetek = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Konec = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clanstva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clanstva_Clani_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clani",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prisotnosti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumPrisotnosti = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClanId = table.Column<int>(type: "int", nullable: false),
                    VadbaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prisotnosti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prisotnosti_Clani_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clani",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prisotnosti_Vadbe_VadbaId",
                        column: x => x.VadbaId,
                        principalTable: "Vadbe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumRezervacije = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClanId = table.Column<int>(type: "int", nullable: false),
                    VadbaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Clani_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clani",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Vadbe_VadbaId",
                        column: x => x.VadbaId,
                        principalTable: "Vadbe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Placila",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumPlacila = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Znesek = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClanstvoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Placila", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Placila_Clanstva_ClanstvoId",
                        column: x => x.ClanstvoId,
                        principalTable: "Clanstva",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clanstva_ClanId",
                table: "Clanstva",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_Placila_ClanstvoId",
                table: "Placila",
                column: "ClanstvoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prisotnosti_ClanId",
                table: "Prisotnosti",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_Prisotnosti_VadbaId",
                table: "Prisotnosti",
                column: "VadbaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_ClanId",
                table: "Rezervacije",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_VadbaId",
                table: "Rezervacije",
                column: "VadbaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Placila");

            migrationBuilder.DropTable(
                name: "Prisotnosti");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropTable(
                name: "Clanstva");

            migrationBuilder.DropTable(
                name: "Vadbe");

            migrationBuilder.DropTable(
                name: "Clani");
        }
    }
}
