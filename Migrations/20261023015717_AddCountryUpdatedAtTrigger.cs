using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMFFundingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryUpdatedAtTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER TR_Country_SetUpdatedAt");
        }
    }
}