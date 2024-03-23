using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data.Models;
using Xunit;

namespace WebApplication2.Data
{
    public sealed class ApplicationContext : DbContext, IApplicationContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Assets> Assets { get; set; }

        public async Task<List<Assets>> GetAssetsAsync()
        {
            return await Assets.ToListAsync();
        }
        public async Task<Assets> FindAssetAsync(int id)
        {
            return await Assets.FindAsync(id);
        }
        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }
        public async Task<Assets> FindAsync(int id)
        {
            var asset = Assets.FirstOrDefault(a => a.id == id);
            return await Task.FromResult(asset);
        }
    }
    

}
