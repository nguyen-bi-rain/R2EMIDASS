using LMS.DTOs;
using LMS.DTOs.Shared;

namespace LMS.Services.Interfaces
{
    public interface ICategoryService 
    {
        Task<CategoryDto> GetByIdAsync(int id);
        Task<PaginatedList<CategoryDto>> GetAllAsync(int pageSize = 10, int pageIndex = 1);
        Task<CategoryCreatDto> AddAsync(CategoryCreatDto entity);
        Task UpdateAsync(CategoryUpdateDto entity);
        Task DeleteAsync(int id);
    }
}