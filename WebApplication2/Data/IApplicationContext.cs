using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.Data.Models;

namespace WebApplication2.Data
{
    public interface IApplicationContext
    {
        Task<List<Assets>> GetAssetsAsync();
    }
}
