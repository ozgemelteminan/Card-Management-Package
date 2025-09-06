using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerchantApp.API.Migrations
{
    // Migration to add stock tracking to products and UpdatedAt column to transactions
    public partial class AddStockToProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add a nullable UpdatedAt column to the Transactions table
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            // Add a Stock column to the Products table with default value 0
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the UpdatedAt column from Transactions
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Transactions");

            // Remove the Stock column from Products
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");
        }
    }
}
