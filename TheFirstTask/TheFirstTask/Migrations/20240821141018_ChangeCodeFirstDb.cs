using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheFirstTask.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCodeFirstDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskOrderDetails_Orders_OrderId",
                table: "TaskOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskOrderDetails_TaskOrders_TaskId",
                table: "TaskOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_TaskOrderDetails_OrderId",
                table: "TaskOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_TaskOrderDetails_TaskId",
                table: "TaskOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TaskOrderDetails_OrderId",
                table: "TaskOrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskOrderDetails_TaskId",
                table: "TaskOrderDetails",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskOrderDetails_Orders_OrderId",
                table: "TaskOrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskOrderDetails_TaskOrders_TaskId",
                table: "TaskOrderDetails",
                column: "TaskId",
                principalTable: "TaskOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
