using EShop.Data;
using EShop.Models;
using EShop.Models.Request;
using EShop.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace EShop.Service;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // 查看所有商品
    public async Task<ApiResponse<List<Product>>> GetAllProductsAsync()
    {
        try
        {
            var products = await _context.Products
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return ApiResponse<List<Product>>.SuccessResponse(products, "查詢成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Product>>.FailureResponse($"伺服器錯誤: {ex.Message}");
        }
    }

    // 查看單個商品
    public async Task<ApiResponse<Product>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return ApiResponse<Product>.FailureResponse("商品不存在");

            return ApiResponse<Product>.SuccessResponse(product, "查詢成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<Product>.FailureResponse($"伺服器錯誤: {ex.Message}");
        }
    }

    // 新增商品
    public async Task<ApiResponse<ProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            // 驗證輸入
            var validationError = ValidateProductRequest(request);
            if (validationError != null)
                return validationError;

            // 檢查分類是否存在
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
                return ApiResponse<ProductResponse>.FailureResponse("分類不存在");

            // 檢查商品名稱是否已存在
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == request.Name);
            if (existingProduct != null)
                return ApiResponse<ProductResponse>.FailureResponse("商品名稱已存在");

            // 建立商品
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = MapToProductDto(product);
            return ApiResponse<ProductResponse>.SuccessResponse(productDto, "商品新增成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductResponse>.FailureResponse($"伺服器錯誤: {ex.Message}");
        }
    }

    // 修改商品
    public async Task<ApiResponse<ProductResponse>> UpdateProductAsync(int id, CreateProductRequest request)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return ApiResponse<ProductResponse>.FailureResponse("商品不存在");

            // 驗證輸入
            var validationError = ValidateProductRequest(request);
            if (validationError != null)
                return validationError;

            // 檢查分類是否存在
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
                return ApiResponse<ProductResponse>.FailureResponse("分類不存在");

            // 檢查商品名稱是否已被其他商品使用
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == request.Name && p.Id != id);
            if (existingProduct != null)
                return ApiResponse<ProductResponse>.FailureResponse("商品名稱已被其他商品使用");

            // 更新商品
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.CategoryId = request.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            var productDto = MapToProductDto(product);
            return ApiResponse<ProductResponse>.SuccessResponse(productDto, "商品修改成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductResponse>.FailureResponse($"伺服器錯誤: {ex.Message}");
        }
    }

    // 刪除商品
    public async Task<ApiResponse<object>> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return ApiResponse<object>.FailureResponse("商品不存在");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return ApiResponse<object>.SuccessResponse(null, "商品刪除成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.FailureResponse($"伺服器錯誤: {ex.Message}");
        }
    }

    // 私有方法：驗證商品請求
    private ApiResponse<ProductResponse>? ValidateProductRequest(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return ApiResponse<ProductResponse>.FailureResponse("商品名稱不能為空");

        if (request.Price <= 0)
            return ApiResponse<ProductResponse>.FailureResponse("商品價格必須大於 0");

        if (request.Stock < 0)
            return ApiResponse<ProductResponse>.FailureResponse("庫存不能為負數");

        return null;
    }

    // 私有方法：映射到 DTO
    private ProductResponse MapToProductDto(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}