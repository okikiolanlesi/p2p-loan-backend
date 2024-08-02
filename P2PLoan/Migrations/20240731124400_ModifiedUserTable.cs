using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P2PLoan.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasPin",
                table: "Users",
                newName: "PinCreated");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "WalletProviders",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "Modules",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WalletProviders_Slug",
                table: "WalletProviders",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_Identifier",
                table: "Modules",
                column: "Identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletProviders_Slug",
                table: "WalletProviders");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Modules_Identifier",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Modules");

            migrationBuilder.RenameColumn(
                name: "PinCreated",
                table: "Users",
                newName: "HasPin");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "WalletProviders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
