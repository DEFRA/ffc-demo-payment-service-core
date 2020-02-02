using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace FFCDemoPaymentService.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddDateToSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "value",
                table: "schedule");

            migrationBuilder.AddColumn<DateTime>(
                name: "paymentDate",
                table: "schedule",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paymentDate",
                table: "schedule");

            migrationBuilder.AddColumn<decimal>(
                name: "value",
                table: "schedule",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
