using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[] { new Guid("7ceb50b3-2d36-4a76-a8c7-ae2d5a924d4c"), "1549794e-2db9-4e5b-bc8d-798627fc4d37", "Admin", "ADMIN" });

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("7ceb50b3-2d36-4a76-a8c7-ae2d5a924d4c"));
        }
    }
}
