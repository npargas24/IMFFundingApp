using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMFFundingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryAndDisbursement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disbursements",
                columns: table => new
                {
                    DisbursementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    TrancheNo = table.Column<int>(type: "int", nullable: false),
                    AmountSDR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReleasedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disbursements", x => x.DisbursementId);
                    table.CheckConstraint("CK_Disbursement_Amount", "[AmountSDR] > 0");
                    table.CheckConstraint("CK_Disbursement_Released", "([Status] = 'Released' AND [ReleasedDate] IS NOT NULL) OR ([Status] = 'Scheduled' AND [ReleasedDate] IS NULL)");
                    table.ForeignKey(
                        name: "FK_Disbursements_FundingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "FundingRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatusHistories",
                columns: table => new
                {
                    HistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ToStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChangedBy = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatusHistories", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_RequestStatusHistories_ApplicationUsers_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "ApplicationUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestStatusHistories_FundingRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "FundingRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disbursements_RequestId_TrancheNo",
                table: "Disbursements",
                columns: new[] { "RequestId", "TrancheNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatusHistories_ChangedBy",
                table: "RequestStatusHistories",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatusHistories_RequestId",
                table: "RequestStatusHistories",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disbursements");

            migrationBuilder.DropTable(
                name: "RequestStatusHistories");
        }
    }
}
