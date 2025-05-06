using AutoMapper;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Exceptions;
using LMS.Models.Enitties;
using LMS.Repositories.Interfaces;
using LMS.Services.Interfaces;
using LMS.Specifications;

namespace LMS.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ;
            _mapper = mapper ;
        }

        public async Task<CategoryCreatDto> AddAsync(CategoryCreatDto entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if ( _categoryRepository.GetQuery().Any(c => c.Name == entity.Name))
            {
                throw new ResourceUniqueException("Category already exists.");
            }

            var category = _mapper.Map<Category>(entity);
            await _categoryRepository.AddAsync(category);
            if (await _categoryRepository.SaveChangesAsync() < 0)
            {
                throw new DatabaseBadRequestException("Failed to create category.");
            }

            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException("Category not found.");

            await _categoryRepository.DeleteAsync(existingCategory);
            if (await _categoryRepository.SaveChangesAsync() < 0)
            {
                throw new DatabaseBadRequestException("Failed to delete category.");
            }
        }

        public async Task<PaginatedList<CategoryDto>> GetAllAsync(int pageSize = 10, int pageIndex = 1)
        {
            var spec= new CategorySpecification(pageIndex, pageSize);
            var result = await _categoryRepository.GetAllCategoriesAsync(spec);
            if (result.categories == null || !result.categories.Any())
            {
                throw new ResourceNotFoundException("No categories found.");
            }
            var model = _mapper.Map<IEnumerable<CategoryDto>>(result.categories);
            return new PaginatedList<CategoryDto>(model.ToList(), result.totalCount, pageIndex, pageSize);
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException("Category not found.");

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task UpdateAsync(CategoryUpdateDto entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var existingCategory = await _categoryRepository.GetByIdAsync(entity.Id)
                ?? throw new ResourceNotFoundException("Category not found.");

            _mapper.Map(entity, existingCategory);
            await _categoryRepository.UpdateAsync(existingCategory);
            if (await _categoryRepository.SaveChangesAsync() < 0)
            {
                throw new DatabaseBadRequestException("Failed to update category.");
            }
        }
    }
}
