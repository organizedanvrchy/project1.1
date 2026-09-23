using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bankingApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOnDeleteBehaviorForAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cheque_requests_accounts",
                table: "cheque_book_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_recipient",
                table: "transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_cheque_requests_accounts",
                table: "cheque_book_requests",
                column: "account_id",
                principalTable: "bank_accounts",
                principalColumn: "account_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_recipient",
                table: "transactions",
                column: "recipient_account_id",
                principalTable: "bank_accounts",
                principalColumn: "account_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cheque_requests_accounts",
                table: "cheque_book_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_recipient",
                table: "transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_cheque_requests_accounts",
                table: "cheque_book_requests",
                column: "account_id",
                principalTable: "bank_accounts",
                principalColumn: "account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_recipient",
                table: "transactions",
                column: "recipient_account_id",
                principalTable: "bank_accounts",
                principalColumn: "account_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
