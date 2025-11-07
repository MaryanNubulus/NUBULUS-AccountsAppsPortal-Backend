using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nubulus.backend.api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedAccountId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "accounts",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone",
                table: "accounts");
        }
    }
}
