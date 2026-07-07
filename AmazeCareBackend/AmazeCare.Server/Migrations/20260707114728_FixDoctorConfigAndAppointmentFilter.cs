using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazeCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixDoctorConfigAndAppointmentFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_BookedByUserUserId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_BookedByUserUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "BookedByUserUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookedByUserUserId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BookedByUserUserId",
                table: "Appointments",
                column: "BookedByUserUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_BookedByUserUserId",
                table: "Appointments",
                column: "BookedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
