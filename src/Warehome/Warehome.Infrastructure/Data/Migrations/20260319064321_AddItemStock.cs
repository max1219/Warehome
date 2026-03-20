using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehome.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddItemStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    StorageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemStocks_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemStocks_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemStocks_ItemTypeId",
                table: "ItemStocks",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemStocks_StorageId",
                table: "ItemStocks",
                column: "StorageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemStocks");
        }
    }
}
