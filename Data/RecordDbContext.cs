using Microsoft.EntityFrameworkCore;
using PFSign.Models;

namespace PFSign.Data
{
    public class RecordDbContext : DbContext
    {
        public RecordDbContext(DbContextOptions<RecordDbContext> options)
            :base(options)
        {}

        public DbSet<Record> Records { get; set; }
    }
}