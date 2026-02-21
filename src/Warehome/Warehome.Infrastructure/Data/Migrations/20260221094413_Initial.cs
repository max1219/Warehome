using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Warehome.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StorageCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageCategories_StorageCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "StorageCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Storages_StorageCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "StorageCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorageCategories_ParentId",
                table: "StorageCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageCategories_Path",
                table: "StorageCategories",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Storages_CategoryId",
                table: "Storages",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_Name",
                table: "Storages",
                column: "Name",
                unique: true,
                filter: "CategoryId IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_Name_CategoryId",
                table: "Storages",
                columns: new[] { "Name", "CategoryId" },
                unique: true,
                filter: "CategoryId IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Storages");

            migrationBuilder.DropTable(
                name: "StorageCategories");
        }
    }
}
