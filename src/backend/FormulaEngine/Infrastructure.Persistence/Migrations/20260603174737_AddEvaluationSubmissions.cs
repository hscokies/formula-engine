using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "submissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    result = table.Column<decimal>(type: "numeric", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    formula_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_submissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_submissions_formulas_formula_id",
                        column: x => x.formula_id,
                        principalTable: "formulas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_submissions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_submissions_formula_id",
                table: "submissions",
                column: "formula_id");

            migrationBuilder.CreateIndex(
                name: "ix_submissions_user_id",
                table: "submissions",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "submissions");
        }
    }
}
