using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskOneDraft.Migrations
{
    /// <inheritdoc />
    public partial class p3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OvertimeHours",
                table: "Claims",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OvertimeRate",
                table: "Claims",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OvertimeHours",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "OvertimeRate",
                table: "Claims");
        }
    }
}
