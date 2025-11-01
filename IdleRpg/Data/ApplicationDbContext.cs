using IdleRpg.Data.Db;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdleRpg.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<CharacterStat> CharacterStats => Set<CharacterStat>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();


    }
}
