using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finerd.Api.Migrations
{
    public partial class PushNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_PaymentMethod_PaymentMethodId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_PaymentMethodId",
                table: "Transaction");

            migrationBuilder.CreateTable(
                name: "PushSubscriptions",
                columns: table => new
                {
                    Endpoint = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    P256DH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Auth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushSubscriptions", x => x.Endpoint);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PushSubscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PaymentMethodId",
                table: "Transaction",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_PaymentMethod_PaymentMethodId",
                table: "Transaction",
                column: "PaymentMethodId",
                principalTable: "PaymentMethod",
                principalColumn: "Id");
        }
    }
}
