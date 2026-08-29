using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kritikos.Samples.CityCensus.Migrations
{
    /// <inheritdoc />
    public partial class AddsRedactedAuditColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Redacted",
                table: "AuditRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Redacted",
                table: "AuditRecords");
        }
    }
}
