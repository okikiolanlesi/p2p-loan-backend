using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P2PLoan.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedLoanRequestAndOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccruingInterestRate",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "GracePeriodDays",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "LoanDurationDays",
                table: "LoanRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessingStartTime",
                table: "LoanRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "InterestRate",
                table: "LoanOffers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "AccruingInterestRate",
                table: "LoanOffers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessingStartTime",
                table: "LoanRequests");

            migrationBuilder.AddColumn<int>(
                name: "AccruingInterestRate",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GracePeriodDays",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InterestRate",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoanDurationDays",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "InterestRate",
                table: "LoanOffers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "AccruingInterestRate",
                table: "LoanOffers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
