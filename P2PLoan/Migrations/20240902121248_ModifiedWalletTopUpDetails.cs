using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P2PLoan.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedWalletTopUpDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopUpAccountName",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "TopUpAccountNumber",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "TopUpBankCode",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "TopUpBankName",
                table: "Wallets");

            migrationBuilder.CreateTable(
                name: "WalletTopUpDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTopUpDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTopUpDetails_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WalletTopUpDetails_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WalletTopUpDetails_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTopUpDetails_CreatedById",
                table: "WalletTopUpDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTopUpDetails_ModifiedById",
                table: "WalletTopUpDetails",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTopUpDetails_WalletId",
                table: "WalletTopUpDetails",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletTopUpDetails");

            migrationBuilder.AddColumn<string>(
                name: "TopUpAccountName",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopUpAccountNumber",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopUpBankCode",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopUpBankName",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
