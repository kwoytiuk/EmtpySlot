using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmptySlot.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAndPasswordToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make Provider.UserId nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Providers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // Add Email column to Profiles
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Profiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            // Add PasswordHash column to Profiles
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Profiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            // Create unique index on Email
            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Email",
                table: "Profiles",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_Profiles_Email",
                table: "Profiles");

            // Remove Email column
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Profiles");

            // Remove PasswordHash column
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Profiles");

            // Make Provider.UserId non-nullable again
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Providers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
