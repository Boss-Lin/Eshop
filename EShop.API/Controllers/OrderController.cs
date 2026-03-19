using System.Security.Claims;
using EShop.Models.Response;
using EShop.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(OrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    // POST /api/order/checkout - 結帳
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.CheckoutAsync(userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "結帳時發生錯誤");
            return StatusCode(500, ApiResponse<OrderResponse>.FailureResponse("結帳失敗"));
        }
    }

    // GET /api/order - 查看所有訂單
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.GetOrdersAsync(userId);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得訂單列表時發生錯誤");
            return StatusCode(500, ApiResponse<List<OrderResponse>>.FailureResponse("取得訂單失敗"));
        }
    }

    // GET /api/order/{id} - 查看單筆訂單
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.GetOrderByIdAsync(userId, id);

            if (result.Success)
                return Ok(result);

            return result.Message.Contains("無權限") ? Forbid() : NotFound(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得訂單時發生錯誤");
            return StatusCode(500, ApiResponse<OrderResponse>.FailureResponse("取得訂單失敗"));
        }
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("無法取得有效的使用者 ID");
        }

        return userId;
    }
}
