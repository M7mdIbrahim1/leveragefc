using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class LineOfBusinessRepository : Repository<LineOfBusiness>
    {
        public LineOfBusinessRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}