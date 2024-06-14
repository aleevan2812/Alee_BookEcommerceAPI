using System.Linq.Expressions;
using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace Alee_BookEcommerceAPI.Repository.IRepository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;
    private IProductRepository _productRepositoryImplementation;

    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task UpdateAsync(Product entity)
    {
        _db.Products.Update(entity);
    }

    public async Task<Product> GetAsyncWithProductImages(Expression<Func<Product, bool>> filter = null,
        bool tracked = true,
        string? includeProperties = null)
    {
        var product = await GetAsync(filter, tracked, includeProperties);
        if (product != null)
        {
            var productImages = _db.ProductsImages.Where(u => u.ProductId == product.Id).ToList();
            if (productImages != null) product.ProductImages = productImages;
        }

        return product;
    }

    public async Task<List<Product>> GetAllAsyncWithProductImages(Expression<Func<Product, bool>>? filter = null,
        string? includeProperties = null, int pageSize = 0,
        int pageNumber = 1)
    {
        var products = await GetAllAsync(filter, includeProperties, pageSize, pageNumber);
        IEnumerable<ProductImage> productImages = _db.ProductsImages.Include("Product");

        foreach (var image in productImages) products.FirstOrDefault(u => u.Id == image.ProductId);

        return products;
    }
}