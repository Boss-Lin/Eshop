using EShop.Data;
using EShop.Models;
using EShop.Models.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EShop.Service;

public class OrderService
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(AppDbContext context, ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<OrderResponse>> CheckoutAsync(int userId)
    {
        // 1. 取得購物車（含 CartItems → Product）
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        // 2. 驗證購物車不為空
        if (cart == null || !cart.CartItems.Any())
        {
            return ApiResponse<OrderResponse>.FailureResponse("購物車是空的，無法結帳");
        }

        // 3. 逐一驗證庫存
        foreach (var item in cart.CartItems)
        {
            if (item.Product.Stock < item.Quantity)
            {
                return ApiResponse<OrderResponse>.FailureResponse(
                    $"商品「{item.Product.Name}」庫存不足。目前庫存：{item.Product.Stock}，您要購買：{item.Quantity}");
            }
        }

        // 4. 建立 Order
        var totalPrice = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);
        var order = new Order
        {
            UserId = userId,
            TotalPrice = totalPrice,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // 5. 建立 OrderItems 並扣減庫存
        foreach (var item in cart.CartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price,
                TotalPrice = item.Product.Price * item.Quantity
            };
            _context.OrderItems.Add(orderItem);

            // 6. 扣減庫存
            item.Product.Stock -= item.Quantity;
        }

        // 7. 清空購物車
        cart.CartItems.Clear();
        cart.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("使用者 {UserId} 完成結帳，訂單 ID: {OrderId}，金額: {TotalPrice}",
            userId, order.Id, totalPrice);

        // 8. 重新載入訂單（含 OrderItems + Product）回傳
        var createdOrder = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstAsync(o => o.Id == order.Id);

        return ApiResponse<OrderResponse>.SuccessResponse(MapToOrderResponse(createdOrder), "結帳成功");
    }

    public async Task<ApiResponse<List<OrderResponse>>> GetOrdersAsync(int userId)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var result = orders.Select(MapToOrderResponse).ToList();
        return ApiResponse<List<OrderResponse>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<OrderResponse>> GetOrderByIdAsync(int userId, int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return ApiResponse<OrderResponse>.FailureResponse($"訂單 ID {orderId} 不存在");
        }

        if (order.UserId != userId)
        {
            return ApiResponse<OrderResponse>.FailureResponse("無權限查看此訂單");
        }

        return ApiResponse<OrderResponse>.SuccessResponse(MapToOrderResponse(order));
    }

    private static OrderResponse MapToOrderResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice
            }).ToList()
        };
    }
}
