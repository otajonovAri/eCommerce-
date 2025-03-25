using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Models;

namespace ProductApi.Infrastructure.Data;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}
