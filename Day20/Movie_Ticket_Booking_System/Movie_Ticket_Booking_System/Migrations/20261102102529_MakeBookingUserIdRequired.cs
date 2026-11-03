using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie_Ticket_Booking_System.Migrations
{
    /// <inheritdoc />
    public partial class MakeBookingUserIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF EXISTS (SELECT 1 FROM [Bookings] WHERE [UserId] IS NULL) " +
                "THROW 51000, 'Backfill all booking owners before making UserId required.', 1;");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
