using EShop.Data;
using EShop.Model;
using Microsoft.EntityFrameworkCore;

namespace EShop.Service;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<decimal> GetProductPriceAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product?.Price ?? 0;
    }
}