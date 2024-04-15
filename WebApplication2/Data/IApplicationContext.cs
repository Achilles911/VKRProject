using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.Data.Models;

namespace WebApplication2.Data
{
    public interface IApplicationContext
    {
        Task<List<Assets>> GetAssetsAsync();
        DbSet<Assets> Assets { get; }
        DbSet<Users> Users { get; }

        Task<Assets> FindAssetAsync(int id);
        Task SaveChangesAsync();
        Task<Users> FindUserAsync(int id);
    }
}
