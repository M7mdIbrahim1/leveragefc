using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class OpportunityRepository : Repository<Opportunity>
    {
        public OpportunityRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}