using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class addTokuteiOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokuteiOrders",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Job = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PassedQuantity = table.Column<int>(type: "int", nullable: true),
                    Salary = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Caregiver = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApplicationDate = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    InterviewDate = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Expense = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokuteiOrders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokuteiOrders",
                schema: "Catalog");
        }
    }
}
