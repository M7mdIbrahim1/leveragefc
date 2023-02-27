using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class ProjectRepository : Repository<Project>
    {
        public ProjectRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}