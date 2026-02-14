using System.Security.Claims;
using EShop.Models.Request;
using EShop.Models.Response;
using EShop.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(CartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    //  GET /api/cart - 查看購物車
    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartResponse>> GetCart()
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得購物車時發生錯誤");
            return StatusCode(500, new { message = "取得購物車失敗" });
        }
    }

    // GET /api/cart/summary - 購物車摘要
    [HttpGet("summary")]
    [ProducesResponseType(typeof(CartSummaryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartSummaryResponse>> GetCartSummary()
    {
        try
        {
            var userId = GetUserId();
            var summary = await _cartService.GetCartSummaryAsync(userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得購物車摘要時發生錯誤");
            return StatusCode(500, new { message = "取得購物車摘要失敗" });
        }
    }

    // POST /api/cart/items - 加入商品
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // ← BadRequest
            }

            var userId = GetUserId();
            var cart = await _cartService.AddToCartAsync(userId, request);

            return Ok(new  // ← Ok() 包裝
            {
                message = "成功加入購物車",
                data = cart
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "商品不存在：{ProductId}", request.ProductId);
            return NotFound(new { message = ex.Message });  // ← NotFound
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "加入購物車失敗");
            return BadRequest(new { message = ex.Message });  // ← BadRequest
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加入購物車時發生錯誤");
            return StatusCode(500, new { message = "加入購物車失敗" });  // ← StatusCode
        }
    }

    // PUT /api/cart/items/{cartItemId} - 更新數量
    [HttpPut("items/{cartItemId}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> UpdateCartItem(
        int cartItemId,
        [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, cartItemId, request);

            return Ok(new
            {
                message = "成功更新購物車",
                data = cart
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "購物車項目不存在：{CartItemId}", cartItemId);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "更新購物車項目失敗");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新購物車項目時發生錯誤");
            return StatusCode(500, new { message = "更新購物車失敗" });
        }
    }

    // DELETE /api/cart/items/{cartItemId} - 移除商品
    [HttpDelete("items/{cartItemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCartItem(int cartItemId)
    {
        try
        {
            var userId = GetUserId();
            var result = await _cartService.RemoveCartItemAsync(userId, cartItemId);

            if (!result)
            {
                return NotFound(new { message = "購物車項目不存在" });
            }

            return Ok(new { message = "成功移除商品" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除購物車項目時發生錯誤");
            return StatusCode(500, new { message = "移除商品失敗" });
        }
    }

    // DELETE /api/cart - 清空購物車
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);

            return Ok(new { message = "成功清空購物車" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清空購物車時發生錯誤");
            return StatusCode(500, new { message = "清空購物車失敗" });
        }
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // 1. 檢查是否存在且為數字
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("無法取得有效的使用者 ID");
        }

        return userId;
    }
}