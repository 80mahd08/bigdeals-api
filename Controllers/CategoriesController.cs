using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Categories;
using api.Interfaces.Categories;

namespace api.Controllers;

[Route("api/categories")]
[ApiController]
[AllowAnonymous]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryDto>>>> GetAll()
    {
        var result = await _service.GetAllCategoriesAsync();
        return Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDetailsDto>>> GetById(int id)
    {
        var result = await _service.GetCategoryByIdAsync(id);
        return Ok(ApiResponse<CategoryDetailsDto>.Ok(result));
    }

    [HttpGet("{id}/attributes")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AttributeCategoryDto>>>> GetAttributes(int id)
    {
        var result = await _service.GetCategoryAttributesAsync(id);
        return Ok(ApiResponse<IReadOnlyList<AttributeCategoryDto>>.Ok(result));
    }

    [HttpGet("{id}/schema")]
    public async Task<ActionResult<ApiResponse<CategorySchemaDto>>> GetSchema(int id)
    {
        var result = await _service.GetCategorySchemaAsync(id);
        return Ok(ApiResponse<CategorySchemaDto>.Ok(result));
    }
}
