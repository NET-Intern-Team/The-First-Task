using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheFirstTask.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskOrderAndTaskOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskContent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    TaskOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskOrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskOrderDetails_TaskOrders_TaskOrderId",
                        column: x => x.TaskOrderId,
                        principalTable: "TaskOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskOrderDetails_OrderId",
                table: "TaskOrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskOrderDetails_TaskOrderId",
                table: "TaskOrderDetails",
                column: "TaskOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskOrderDetails");

            migrationBuilder.DropTable(
                name: "TaskOrders");
        }
    }
}
