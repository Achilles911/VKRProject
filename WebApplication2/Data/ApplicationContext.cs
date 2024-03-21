using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.Data.Models;

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
    }
    

}
