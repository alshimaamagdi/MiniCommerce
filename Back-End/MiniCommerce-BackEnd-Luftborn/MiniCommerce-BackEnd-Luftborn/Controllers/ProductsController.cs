using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniCommerce.Application.Products;
namespace MiniCommerce_BackEnd_Luftborn.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var list = await _productService.GetAllAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _productService.GetByIdAsync(id, cancellationToken);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("by-category/{categoryId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var list = await _productService.GetByCategoryIdAsync(categoryId, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
    {
        var created = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
    {
        var updated = await _productService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _productService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
