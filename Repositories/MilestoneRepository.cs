using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class MilestoneRepository : Repository<Milestone>
    {
        public MilestoneRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}