
namespace WebApplication1
{
    using Microsoft.EntityFrameworkCore;

    public class AccountsContext : DbContext
    {
        public AccountsContext(DbContextOptions<AccountsContext> options)
            : base(options)
        {
        }

        public DbSet<PocoSample> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureAccounts(modelBuilder);
        }

        private void ConfigureAccounts(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PocoSample>(p =>
                {
                    p.Property(p => p.ClientId).ToJsonProperty("id");
                    p.Property(p => p.PartitionKey).ToJsonProperty("pk");
                    p.Property(p => p.State).ToJsonProperty("state");
                    p.Property(p => p.ResourceType).ToJsonProperty("resourceType");
                });

            modelBuilder
                .Entity<PocoSample>()
                .HasNoDiscriminator()
                .ToContainer(Startup.ContainerName)
                .HasPartitionKey(d => d.PartitionKey)
                .HasKey(a => a.ClientId);
        }
    }
}
