using ImfFunding.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Context;

public static class DbSeeder
{
    public static async Task SeedAsync(FundingContext context)
    {
        // Only create the staff account if there isn't one yet
        if (await context.ApplicationUsers.AnyAsync(u => u.Role == UserRole.IMFStaff))
            return;

        var staff = new ApplicationUser
        {
            Email = "staff1@imf.org",
            FullName = "IMF Staff One",
            Role = UserRole.IMFStaff,
            CountryId = null
        };
        staff.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(staff, "ChangeMe123!");

        context.ApplicationUsers.Add(staff);
        await context.SaveChangesAsync();
    }
}
