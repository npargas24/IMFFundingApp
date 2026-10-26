using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMFFundingApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER TR_Country_SetUpdatedAt");
            migrationBuilder.DropCheckConstraint(
                name: "CK_FundingRequest_ApprovedAmount",
                table: "FundingRequests");

            migrationBuilder.DropColumn(
                name: "BoardDecisionDate",
                table: "FundingRequests");

            migrationBuilder.DropColumn(
                name: "BoardOutcome",
                table: "FundingRequests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FundingRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FundingRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Countries");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FundingRequest_ApprovedAmount",
                table: "FundingRequests",
                sql: "[ApprovedAmountSDR] IS NULL OR ([ApprovedAmountSDR] > 0 AND [ApprovedAmountSDR] <= [RequestedAmountSDR])");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FundingRequest_ApprovedAmount",
                table: "FundingRequests");

            migrationBuilder.AddColumn<DateOnly>(
                name: "BoardDecisionDate",
                table: "FundingRequests",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoardOutcome",
                table: "FundingRequests",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FundingRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FundingRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Countries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: 1,
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: 3,
                column: "UpdatedAt",
                value: null);

            migrationBuilder.AddCheckConstraint(
                name: "CK_FundingRequest_ApprovedAmount",
                table: "FundingRequests",
                sql: "[ApprovedAmountSDR] IS NULL OR ([BoardOutcome] = 'Approved' AND [ApprovedAmountSDR] > 0 AND [ApprovedAmountSDR] <= [RequestedAmountSDR])");
            migrationBuilder.Sql(@"
                CREATE TRIGGER TR_Country_SetUpdatedAt
                ON Countries
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE c
                    SET UpdatedAt = SYSUTCDATETIME()
                    FROM Countries c
                    INNER JOIN inserted i ON c.CountryId = i.CountryId;
                END");
        }
    }
}
