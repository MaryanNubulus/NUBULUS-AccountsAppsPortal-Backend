using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace nubulus.backend.api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToAuditRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "date_registers");

            migrationBuilder.CreateTable(
                name: "audit_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    table_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    record_key = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    record_type = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    user = table.Column<string>(type: "character varying(264)", maxLength: 264, nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_b64 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_records", x => x.id);
                    table.UniqueConstraint("AK_audit_records_key", x => x.key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_records");

            migrationBuilder.CreateTable(
                name: "date_registers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_b64 = table.Column<string>(type: "text", nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    key = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    record_key = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    record_type = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    table_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    user = table.Column<string>(type: "character varying(264)", maxLength: 264, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_date_registers", x => x.id);
                    table.UniqueConstraint("AK_date_registers_key", x => x.key);
                });
        }
    }
}
