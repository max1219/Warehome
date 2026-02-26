using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehome.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddItemTypeAndCategoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemTypeCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemTypeCategories_ItemTypeCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ItemTypeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemTypes_ItemTypeCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ItemTypeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypeCategories_ParentId",
                table: "ItemTypeCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypeCategories_Path",
                table: "ItemTypeCategories",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_CategoryId",
                table: "ItemTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_Name",
                table: "ItemTypes",
                column: "Name",
                unique: true,
                filter: "CategoryId IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_Name_CategoryId",
                table: "ItemTypes",
                columns: new[] { "Name", "CategoryId" },
                unique: true,
                filter: "CategoryId IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "ItemTypeCategories");
        }
    }
}
