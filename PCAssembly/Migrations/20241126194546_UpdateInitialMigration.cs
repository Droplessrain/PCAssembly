using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCAssembly.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssemblyComponentId",
                table: "AssemblyComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssemblyComponentId",
                table: "AssemblyComponents");
        }
    }
}
