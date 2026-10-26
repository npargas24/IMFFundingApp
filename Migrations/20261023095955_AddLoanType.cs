using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IMFFundingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanTypes",
                columns: table => new
                {
                    LoanTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanTypes", x => x.LoanTypeId);
                });

            migrationBuilder.InsertData(
                table: "LoanTypes",
                columns: new[] { "LoanTypeId", "Code", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "SBA", "Short-term loan for a temporary balance of payments problem. Market-based interest rate.", true, "Stand-By Arrangement" },
                    { 2, "ECF", "Medium to long-term loan for low-income countries with lasting balance of payments problems. Zero or very low interest.", true, "Extended Credit Facility" },
                    { 3, "RSF", "Long-term, affordable financing for climate change and pandemic preparedness.", true, "Resilience and Sustainability Facility" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanTypes_Code",
                table: "LoanTypes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanTypes");
        }
    }
}
