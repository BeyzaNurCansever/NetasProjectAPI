using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data;

public class CalculatorDbContext:DbContext
{
    public CalculatorDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<CalculateModel> CalculateModels { get; set; }

}