using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToServiceRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ServiceRequests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_CategoryId",
                table: "ServiceRequests",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_DepartmentId",
                table: "ServiceRequests",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Categories_CategoryId",
                table: "ServiceRequests",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Departments_DepartmentId",
                table: "ServiceRequests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Categories_CategoryId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Departments_DepartmentId",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_CategoryId",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_DepartmentId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ServiceRequests");
        }
    }
}
