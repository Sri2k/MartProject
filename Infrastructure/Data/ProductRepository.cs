using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;
        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            return await _context.ProductBrands.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
            .Include(p => p.ProductType)
            .Include(p => p.ProductBrand)
            .FirstOrDefaultAsync(p => p.Id == id);
        }


     public async Task<IReadOnlyList<Product>> GetProductsAsync(string? sortBy, int? brandId, int? typeId)
{
    var query = _context.Products
        .Include(p => p.ProductType)
        .Include(p => p.ProductBrand)
        .AsQueryable(); // Cast to IQueryable<Product>

    if (brandId.HasValue)
    {
        query = query.Where(p => p.ProductBrandId == brandId.Value);
    }

    if (typeId.HasValue)
    {
        query = query.Where(p => p.ProductTypeId == typeId.Value);
    }

    var products = await query.ToListAsync();

    switch (sortBy)
    {
        case "priceAsc":
            products = products.OrderBy(p => p.Price).ToList();
            break;
        case "nameDesc":
            products = products.OrderByDescending(p => p.Name).ToList();
            break;
        case "priceDesc":
            products = products.OrderByDescending(p => p.Price).ToList();
            break;
        default:
            products = products.OrderBy(p => p.Name).ThenBy(p => p.Price).ToList();
            break;
    }

    return products;
}






        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();

        }
    }
}