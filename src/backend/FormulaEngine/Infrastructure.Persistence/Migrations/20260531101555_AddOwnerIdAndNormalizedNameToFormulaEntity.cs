using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerIdAndNormalizedNameToFormulaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_formulas_name",
                table: "formulas");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "formulas",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "name_normalized",
                table: "formulas",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                table: "formulas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_formulas_name_normalized",
                table: "formulas",
                column: "name_normalized");

            migrationBuilder.CreateIndex(
                name: "ix_formulas_owner_id",
                table: "formulas",
                column: "owner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_formulas_users_owner_id",
                table: "formulas",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_formulas_users_owner_id",
                table: "formulas");

            migrationBuilder.DropIndex(
                name: "ix_formulas_name_normalized",
                table: "formulas");

            migrationBuilder.DropIndex(
                name: "ix_formulas_owner_id",
                table: "formulas");

            migrationBuilder.DropColumn(
                name: "name_normalized",
                table: "formulas");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "formulas");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "formulas",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_formulas_name",
                table: "formulas",
                column: "name",
                unique: true);
        }
    }
}
