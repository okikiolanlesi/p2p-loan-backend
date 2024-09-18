using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P2PLoan.Migrations
{
    /// <inheritdoc />
    public partial class LoanAndRepaymentMod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Repayments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "FinancialTransactionId",
                table: "Repayments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "InterestRate",
                table: "Repayments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "LoanId",
                table: "Repayments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Repayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Repayments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "CurrentInterestRate",
                table: "Loans",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Repayments_LoanId",
                table: "Repayments",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repayments_Loans_LoanId",
                table: "Repayments",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repayments_Loans_LoanId",
                table: "Repayments");

            migrationBuilder.DropIndex(
                name: "IX_Repayments_LoanId",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "FinancialTransactionId",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "LoanId",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "CurrentInterestRate",
                table: "Loans");
        }
    }
}
