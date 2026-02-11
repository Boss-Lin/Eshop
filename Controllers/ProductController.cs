using EShop.DTO;
using EShop.Models.Response;
using EShop.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController: ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    // 查看所有商品（公開）
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var result = await _productService.GetAllProductsAsync();

        if (result.Success)
            return Ok(result);

        return StatusCode(500, result);
    }

    // 查看單個商品（公開）
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);

        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }

    // 新增商品（需要管理員身份）
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.FailureResponse("輸入資料無效"));

        var result = await _productService.CreateProductAsync(request);

        if (result.Success)
            return CreatedAtAction(nameof(GetProductById), new { id = result.Data?.Id }, result);

        return BadRequest(result);
    }

    // 修改商品（需要管理員身份）
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.FailureResponse("輸入資料無效"));

        var result = await _productService.UpdateProductAsync(id, request);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    // 刪除商品（需要管理員身份）
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (result.Success)
            return Ok(result);

        return NotFound(result);
    }
}