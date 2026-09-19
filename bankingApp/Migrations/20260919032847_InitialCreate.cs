using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bankingApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    user_type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "bank_accounts",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    balance = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    account_type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    original_loan_amount = table.Column<decimal>(type: "decimal(19,4)", nullable: true),
                    withdrawal_limit_per_month = table.Column<int>(type: "int", nullable: true),
                    withdrawals_this_period = table.Column<int>(type: "int", nullable: true),
                    period_start = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bank_accounts", x => x.account_id);
                    table.ForeignKey(
                        name: "FK_bank_accounts_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "cheque_book_requests",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    request_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    request_status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pending"),
                    request_approved_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cheque_book_requests", x => x.request_id);
                    table.ForeignKey(
                        name: "FK_cheque_requests_accounts",
                        column: x => x.account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    transaction_amount = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    transaction_type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    transaction_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    recipient_account_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_transactions_accounts",
                        column: x => x.account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transactions_recipient",
                        column: x => x.recipient_account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bank_accounts_customer_id",
                table: "bank_accounts",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_cheque_book_requests_account_id",
                table: "cheque_book_requests",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_account_id",
                table: "transactions",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_recipient_account_id",
                table: "transactions",
                column: "recipient_account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cheque_book_requests");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "bank_accounts");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
