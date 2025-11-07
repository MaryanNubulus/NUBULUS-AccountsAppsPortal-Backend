using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nubulus.backend.api.Migrations
{
    /// <inheritdoc />
    public partial class AfegirNousCampsAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountUsers_Users_UserId",
                table: "AccountUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountUsers_accounts_AccountId",
                table: "AccountUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountUsers",
                table: "AccountUsers");

            migrationBuilder.DropIndex(
                name: "IX_AccountUsers_AccountId",
                table: "AccountUsers");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "AccountUsers",
                newName: "accounts_users");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "users",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "users",
                newName: "key");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "accounts_users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "accounts_users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "accounts_users",
                newName: "account_id");

            migrationBuilder.RenameIndex(
                name: "IX_AccountUsers_UserId",
                table: "accounts_users",
                newName: "IX_accounts_users_user_id");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "key",
                table: "users",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "accounts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "accounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "number_id",
                table: "accounts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "accounts",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "accounts",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users",
                columns: new[] { "account_id", "user_id" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_accounts_account_id",
                table: "accounts_users");

            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_users_user_id",
                table: "accounts_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users");

            migrationBuilder.DropColumn(
                name: "address",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "email",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "number_id",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "status",
                table: "accounts");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "accounts_users",
                newName: "AccountUsers");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "key",
                table: "Users",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AccountUsers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AccountUsers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "AccountUsers",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_users_user_id",
                table: "AccountUsers",
                newName: "IX_AccountUsers_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountUsers",
                table: "AccountUsers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccountUsers_AccountId",
                table: "AccountUsers",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountUsers_Users_UserId",
                table: "AccountUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountUsers_accounts_AccountId",
                table: "AccountUsers",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
