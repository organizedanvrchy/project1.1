using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bankingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionDirection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "transaction_direction",
                table: "transactions",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "transaction_direction",
                table: "transactions");
        }
    }
}
