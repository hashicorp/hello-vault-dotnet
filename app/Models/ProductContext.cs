using Microsoft.EntityFrameworkCore;
using app.Vault;

namespace app.Models
{
  public class ProductContext : DbContext
  {
    public DbSet<Product> Products { get; set; }
    private VaultWrapper _vault;

    public ProductContext(DbContextOptions<ProductContext> options, VaultWrapper vault) : base(options)
    {
        _vault = vault;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 
        optionsBuilder.UseSqlServer();
    }
  }
}