using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class ClientRepository : Repository<Client>
    {
        public ClientRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}