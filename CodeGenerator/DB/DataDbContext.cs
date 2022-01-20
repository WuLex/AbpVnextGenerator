using Microsoft.EntityFrameworkCore;

namespace CodeGenerator.DB
{
    public class DataDbContext: DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options): base(options)
        { 
        
        }
    }
}
