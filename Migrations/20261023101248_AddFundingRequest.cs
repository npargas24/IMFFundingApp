using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMFFundingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFundingRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundingRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    LoanTypeId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestedAmountSDR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    BoardDecisionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BoardOutcome = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ApprovedAmountSDR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVer = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundingRequests", x => x.RequestId);
                    table.CheckConstraint("CK_FundingRequest_ApprovedAmount", "[ApprovedAmountSDR] IS NULL OR ([BoardOutcome] = 'Approved' AND [ApprovedAmountSDR] > 0 AND [ApprovedAmountSDR] <= [RequestedAmountSDR])");
                    table.CheckConstraint("CK_FundingRequest_RequestedAmount", "[RequestedAmountSDR] > 0");
                    table.ForeignKey(
                        name: "FK_FundingRequests_ApplicationUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "ApplicationUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundingRequests_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundingRequests_LoanTypes_LoanTypeId",
                        column: x => x.LoanTypeId,
                        principalTable: "LoanTypes",
                        principalColumn: "LoanTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FundingRequests_CountryId_Status",
                table: "FundingRequests",
                columns: new[] { "CountryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FundingRequests_CreatedBy",
                table: "FundingRequests",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FundingRequests_LoanTypeId",
                table: "FundingRequests",
                column: "LoanTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FundingRequests_ReferenceNo",
                table: "FundingRequests",
                column: "ReferenceNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundingRequests");
        }
    }
}
