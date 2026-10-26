using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMFFundingApp.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyFundingRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FundingRequests_ReferenceNo",
                table: "FundingRequests");

            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                table: "FundingRequests");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "FundingRequests");

            migrationBuilder.DropColumn(
                name: "RowVer",
                table: "FundingRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                table: "FundingRequests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "RequestDate",
                table: "FundingRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVer",
                table: "FundingRequests",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_FundingRequests_ReferenceNo",
                table: "FundingRequests",
                column: "ReferenceNo",
                unique: true);
        }
    }
}
