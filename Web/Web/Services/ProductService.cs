using Microsoft.EntityFrameworkCore;
using Web.Client.Models;
using Web.Data;

namespace Web.Services;

public class ProductService
{
    private ApplicationDbContext _context { get; set; }
    
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProductsFromIds(List<int> productIds)
    {
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
        return products;
    }
    public async Task<Product> GetProductFromId(int productId)
    {
        var products = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
        return products;
    }
    public async Task<List<Product>> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return products;
    }
}