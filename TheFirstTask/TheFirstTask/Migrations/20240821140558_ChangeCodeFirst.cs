using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheFirstTask.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCodeFirst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskOrderDetails_TaskOrders_TaskOrderId",
                table: "TaskOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_TaskOrderDetails_TaskOrderId",
                table: "TaskOrderDetails");

            migrationBuilder.DropColumn(
                name: "TaskOrderId",
                table: "TaskOrderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_TaskOrderDetails_TaskId",
                table: "TaskOrderDetails",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskOrderDetails_TaskOrders_TaskId",
                table: "TaskOrderDetails",
                column: "TaskId",
                principalTable: "TaskOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskOrderDetails_TaskOrders_TaskId",
                table: "TaskOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_TaskOrderDetails_TaskId",
                table: "TaskOrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "TaskOrderId",
                table: "TaskOrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaskOrderDetails_TaskOrderId",
                table: "TaskOrderDetails",
                column: "TaskOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskOrderDetails_TaskOrders_TaskOrderId",
                table: "TaskOrderDetails",
                column: "TaskOrderId",
                principalTable: "TaskOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
