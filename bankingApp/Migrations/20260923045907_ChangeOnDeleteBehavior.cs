using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bankingApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOnDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bank_accounts_users_customer_id",
                table: "bank_accounts");

            migrationBuilder.AddForeignKey(
                name: "FK_bank_accounts_users_customer_id",
                table: "bank_accounts",
                column: "customer_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bank_accounts_users_customer_id",
                table: "bank_accounts");

            migrationBuilder.AddForeignKey(
                name: "FK_bank_accounts_users_customer_id",
                table: "bank_accounts",
                column: "customer_id",
                principalTable: "users",
                principalColumn: "user_id");
        }
    }
}
