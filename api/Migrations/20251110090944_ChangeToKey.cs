using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nubulus.backend.api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_accounts_account_id",
                table: "accounts_users");

            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_users_user_id",
                table: "accounts_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users");

            migrationBuilder.DropIndex(
                name: "IX_accounts_users_user_id",
                table: "accounts_users");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "accounts_users");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "accounts_users");

            migrationBuilder.DropColumn(
                name: "role",
                table: "accounts_users");

            migrationBuilder.RenameColumn(
                name: "shared",
                table: "accounts_users",
                newName: "creator");

            migrationBuilder.AddColumn<string>(
                name: "account_key",
                table: "accounts_users",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "key",
                table: "accounts_users",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_key",
                table: "accounts_users",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_users_key",
                table: "users",
                column: "key");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_accounts_users_key",
                table: "accounts_users",
                column: "key");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users",
                column: "id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_accounts_key",
                table: "accounts",
                column: "key");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_users_account_key",
                table: "accounts_users",
                column: "account_key");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_users_user_key",
                table: "accounts_users",
                column: "user_key");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_accounts_account_key",
                table: "accounts_users",
                column: "account_key",
                principalTable: "accounts",
                principalColumn: "key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_users_user_key",
                table: "accounts_users",
                column: "user_key",
                principalTable: "users",
                principalColumn: "key",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_accounts_account_key",
                table: "accounts_users");

            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_users_user_key",
                table: "accounts_users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_users_key",
                table: "users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_accounts_users_key",
                table: "accounts_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users");

            migrationBuilder.DropIndex(
                name: "IX_accounts_users_account_key",
                table: "accounts_users");

            migrationBuilder.DropIndex(
                name: "IX_accounts_users_user_key",
                table: "accounts_users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_accounts_key",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "account_key",
                table: "accounts_users");

            migrationBuilder.DropColumn(
                name: "key",
                table: "accounts_users");

            migrationBuilder.DropColumn(
                name: "user_key",
                table: "accounts_users");

            migrationBuilder.RenameColumn(
                name: "creator",
                table: "accounts_users",
                newName: "shared");

            migrationBuilder.AddColumn<int>(
                name: "account_id",
                table: "accounts_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "accounts_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "accounts_users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users",
                columns: new[] { "account_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_users_user_id",
                table: "accounts_users",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_accounts_account_id",
                table: "accounts_users",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_users_user_id",
                table: "accounts_users",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
