using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nubulus.backend.api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PASO 1: Eliminar Foreign Keys que dependen de los Alternate Keys
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_accounts_account_key",
                table: "accounts_users");

            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_users_user_key",
                table: "accounts_users");

            // PASO 2: Eliminar Alternate Keys (ahora que no hay FK que dependan)
            migrationBuilder.DropUniqueConstraint(
                name: "AK_users_key",
                table: "users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_accounts_key",
                table: "accounts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_audit_records_key",
                table: "audit_records");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_accounts_users_key",
                table: "accounts_users");

            // PASO 3: Eliminar Primary Keys actuales
            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts",
                table: "accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_records",
                table: "audit_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users");

            // PASO 4: Crear nuevas Primary Keys usando 'key'
            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "key");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts",
                table: "accounts",
                column: "key");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_records",
                table: "audit_records",
                column: "key");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users",
                column: "key");

            // PASO 5: Crear índices en las antiguas PK (ahora solo columnas normales)
            migrationBuilder.CreateIndex(
                name: "IX_users_id",
                table: "users",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_id",
                table: "accounts",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_records_id",
                table: "audit_records",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_users_id",
                table: "accounts_users",
                column: "id");

            // PASO 6: Recrear Foreign Keys apuntando a las nuevas PK
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
            // PASO 1: Eliminar Foreign Keys actuales
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_accounts_account_key",
                table: "accounts_users");

            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_users_user_key",
                table: "accounts_users");

            // PASO 2: Eliminar Primary Keys actuales
            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts",
                table: "accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_records",
                table: "audit_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users");

            // PASO 3: Eliminar índices
            migrationBuilder.DropIndex(
                name: "IX_users_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_accounts_id",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_audit_records_id",
                table: "audit_records");

            migrationBuilder.DropIndex(
                name: "IX_accounts_users_id",
                table: "accounts_users");

            // PASO 4: Recrear Alternate Keys
            migrationBuilder.AddUniqueConstraint(
                name: "AK_users_key",
                table: "users",
                column: "key");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_accounts_key",
                table: "accounts",
                column: "key");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_audit_records_key",
                table: "audit_records",
                column: "key");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_accounts_users_key",
                table: "accounts_users",
                column: "key");

            // PASO 5: Recrear Primary Keys originales con 'id'
            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts",
                table: "accounts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_records",
                table: "audit_records",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts_users",
                table: "accounts_users",
                column: "id");

            // PASO 6: Recrear Foreign Keys apuntando a Alternate Keys
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
    }
}
