using ImfFunding.Models;
using Microsoft.EntityFrameworkCore;

//In EF Core, all CRUD operations are performed through the DbContext class. The DbContext tracks entities in memory 
//and interacts with the database through DbSet<TEntity> collections.

//DbContext already implements the Unit of Work pattern: SaveChangesAsync commits all tracked changes in a single transaction

namespace ImfFunding.Context;
public class FundingContext : DbContext
{
    public FundingContext(DbContextOptions<FundingContext> options) : base(options)
    {
    }
    
    #region DbSets
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<LoanType> LoanTypes { get; set; }
    public DbSet<FundingRequest> FundingRequests { get; set; }
    public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; }
public DbSet<Disbursement> Disbursements { get; set; }
    #endregion

    #region Model Configuration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FundingContext).Assembly);
    }
    #endregion

    
}