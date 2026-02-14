using EShop.Data;
using EShop.Models;
using EShop.Models.Request;
using EShop.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace EShop.Service;

public class CartService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CartService> _logger;

    public CartService(AppDbContext context, ILogger<CartService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 取得使用者的購物車
    public async Task<CartResponse> GetCartAsync(int userId)
    {
        var cart = await GetOrCreateCartAsync(userId);

        return new CartResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.CartItems.Select(item => new CartItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                // Price = item.Price,
                Quantity = item.Quantity,
                // Subtotal = item.Subtotal,
                Stock = item.Product.Stock,
                IsAvailable = item.Product.Stock > 0
                // IsAvailable = item.Product.Stock > 0 && item.Product.IsActive
            }).ToList(),
            // TotalAmount = cart.TotalAmount,
            // TotalItems = cart.TotalItems,
            UpdatedAt = cart.UpdatedAt
        };
    }

    // 取得購物車摘要
    public async Task<CartSummaryResponse> GetCartSummaryAsync(int userId)
    {
        var cart = await GetOrCreateCartAsync(userId);

        return new CartSummaryResponse
        {
            // TotalItems = cart.TotalItems,
            // TotalAmount = cart.TotalAmount
        };
    }

    // 加入商品到購物車
    public async Task<CartResponse> AddToCartAsync(int userId, AddToCartRequest request)
        {
            // 1. 檢查商品是否存在
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"商品 ID {request.ProductId} 不存在");
            }

            // 2. 檢查商品是否可用
            // if (!product.IsActive)
            // {
            //     throw new InvalidOperationException($"商品「{product.Name}」目前無法購買");
            // }

            // 3. 檢查庫存
            if (product.Stock < request.Quantity)
            {
                throw new InvalidOperationException(
                    $"商品「{product.Name}」庫存不足。目前庫存：{product.Stock}");
            }

            // 4. 取得或建立購物車
            var cart = await GetOrCreateCartAsync(userId);

            // 5. 檢查購物車中是否已有此商品
            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // 已存在，更新數量
                var newQuantity = existingItem.Quantity + request.Quantity;

                // 再次檢查庫存
                if (product.Stock < newQuantity)
                {
                    throw new InvalidOperationException(
                        $"加入失敗！購物車中已有 {existingItem.Quantity} 件「{product.Name}」，" +
                        $"目前庫存僅剩 {product.Stock} 件");
                }

                existingItem.Quantity = newQuantity;
                // existingItem.Price = product.Price; // 更新價格

                _logger.LogInformation(
                    "使用者 {UserId} 更新購物車項目：商品 {ProductId}，新數量 {Quantity}",
                    userId, request.ProductId, newQuantity);
            }
            else
            {
                // 新增項目
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    // Price = product.Price,
                    AddedAt = DateTime.UtcNow
                };

                cart.CartItems.Add(cartItem);

                _logger.LogInformation(
                    "使用者 {UserId} 新增購物車項目：商品 {ProductId}，數量 {Quantity}",
                    userId, request.ProductId, request.Quantity);
            }

            // 6. 更新購物車時間並儲存
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 7. 返回更新後的購物車
            return await GetCartAsync(userId);
        }

        /// <summary>
        /// 更新購物車項目數量
        /// </summary>
        public async Task<CartResponse> UpdateCartItemAsync(
            int userId,
            int cartItemId,
            UpdateCartItemRequest request)
        {
            // 1. 取得購物車
            var cart = await GetOrCreateCartAsync(userId);

            // 2. 查找購物車項目
            var cartItem = cart.CartItems.FirstOrDefault(item => item.Id == cartItemId);
            if (cartItem == null)
            {
                throw new KeyNotFoundException($"購物車項目 ID {cartItemId} 不存在");
            }

            // 3. 檢查商品是否存在
            var product = await _context.Products.FindAsync(cartItem.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("商品不存在");
            }

            // 4. 檢查商品是否可用
            // if (!product.IsActive)
            // {
            //     throw new InvalidOperationException($"商品「{product.Name}」目前無法購買");
            // }

            // 5. 檢查庫存
            // if (product.Stock < request.Quantity)
            // {
            //     throw new InvalidOperationException(
            //         $"商品「{product.Name}」庫存不足。目前庫存：{product.Stock}，" +
            //         $"您要購買：{request.Quantity}");
            // }

            // 6. 更新數量和價格
            // cartItem.Quantity = request.Quantity;
            // cartItem.Price = product.Price; // 更新為最新價格
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // _logger.LogInformation(
            //     "使用者 {UserId} 更新購物車項目 {CartItemId}，新數量 {Quantity}",
            //     userId, cartItemId, request.Quantity);

            // 7. 返回更新後的購物車
            return await GetCartAsync(userId);
        }

        /// <summary>
        /// 移除購物車項目
        /// </summary>
        public async Task<bool> RemoveCartItemAsync(int userId, int cartItemId)
        {
            // 1. 取得購物車
            var cart = await GetOrCreateCartAsync(userId);

            // 2. 查找購物車項目
            var cartItem = cart.CartItems.FirstOrDefault(item => item.Id == cartItemId);
            if (cartItem == null)
            {
                return false; // 項目不存在
            }

            // 3. 移除項目
            cart.CartItems.Remove(cartItem);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "使用者 {UserId} 移除購物車項目 {CartItemId}",
                userId, cartItemId);

            return true;
        }

        /// <summary>
        /// 清空購物車
        /// </summary>
        public async Task<bool> ClearCartAsync(int userId)
        {
            // 1. 查找購物車
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // 2. 如果購物車不存在或已經是空的
            if (cart == null || !cart.CartItems.Any())
            {
                return false;
            }

            // 3. 清空所有項目
            cart.CartItems.Clear();
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("使用者 {UserId} 清空購物車", userId);

            return true;
        }

        /// <summary>
        /// 取得或建立購物車（私有方法）
        /// </summary>
        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            // 查詢購物車（包含購物車項目和商品資訊）
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // 如果購物車不存在，建立新的
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                _logger.LogInformation("為使用者 {UserId} 建立新購物車，ID: {CartId}",
                    userId, cart.Id);
            }

            return cart;
        }
}