using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize(Roles = "SUPER_USER,NORMAL_USER")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedList<CategoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllCategories(int pageSize = 10, int pageIndex = 1)
        {
            var categories = await _categoryService.GetAllAsync(pageSize, pageIndex);
            return Ok(ApiResponse<PaginatedList<CategoryDto>>.Success(categories, "Category list retrieved successfully."));

        }
        [Authorize(Roles = "SUPER_USER,NORMAL_USER")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(ApiResponse<CategoryDto>.Success(category, "Category retrieved successfully."));
        }
        [Authorize("SUPER_USER")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCategory([FromBody] CategoryCreatDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Failure("Invalid data."));
            var createdCategory = await _categoryService.AddAsync(categoryDto);
            return Ok(ApiResponse<CategoryCreatDto>.Success(createdCategory, "Category created successfully."));

        }

        [Authorize("SUPER_USER")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {

            await _categoryService.DeleteAsync(id);
            return Ok(ApiResponse<string>.Success("","Category deleted successfully."));

        }

        [Authorize("SUPER_USER")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Failure("Invalid data."));
            await _categoryService.UpdateAsync(categoryDto);
            return NoContent();

        }
    }
}
