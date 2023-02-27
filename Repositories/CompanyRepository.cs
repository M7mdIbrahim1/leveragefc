using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class CompanyRepository : Repository<Company>
    {
        public CompanyRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}