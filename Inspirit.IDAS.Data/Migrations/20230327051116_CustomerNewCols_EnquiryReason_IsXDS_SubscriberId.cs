using Microsoft.EntityFrameworkCore.Migrations;

namespace Inspirit.IDAS.Data.Migrations
{
    public partial class CustomerNewCols_EnquiryReason_IsXDS_SubscriberId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnquiryReason",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsXDS",
                table: "Customers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubscriberId",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnquiryReason",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsXDS",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SubscriberId",
                table: "Customers");
        }
    }
}
