using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kritikos.Samples.CityCensus.Migrations
{
    /// <inheritdoc />
    public partial class AddsAuditRecordIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Modification",
                table: "AuditRecords",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_CreatedAt",
                table: "AuditRecords",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_Table_Key",
                table: "AuditRecords",
                columns: new[] { "Table", "Key" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditRecords_CreatedAt",
                table: "AuditRecords");

            migrationBuilder.DropIndex(
                name: "IX_AuditRecords_Table_Key",
                table: "AuditRecords");

            migrationBuilder.AlterColumn<int>(
                name: "Modification",
                table: "AuditRecords",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
