using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nubulus.backend.api.Migrations
{
    /// <inheritdoc />
    public partial class AddFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "accounts_users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "shared",
                table: "accounts_users",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "accounts_users");

            migrationBuilder.DropColumn(
                name: "shared",
                table: "accounts_users");
        }
    }
}
