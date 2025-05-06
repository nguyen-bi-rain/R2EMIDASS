using AutoMapper;
using LMS.DTOs;
using LMS.Exceptions;
using LMS.Models.Enitties;
using LMS.Repositories.Interfaces;
using LMS.Services.Implements;
using LMS.Specifications;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTest.Services
{
    [TestFixture]
    public class CategoryServiceTest
    {
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private Mock<IMapper> _mockMapper;
        private CategoryService _categoryService;

        [SetUp]
        public void Setup()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockMapper.Object);
        }


        [Test]
        public async Task AddAsync_WithValidCategory_ReturnsCategory()
        {
            // Arrange
            var categoryCreateDto = new CategoryCreatDto { Name = "Test Category" };
            var category = new Category { Id = 1, Name = "Test Category" };

            var categoryData = new List<Category>().AsQueryable();
            var mockQuery = new Mock<DbSet<Category>>();
            mockQuery.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categoryData.Provider);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categoryData.Expression);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categoryData.ElementType);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categoryData.GetEnumerator());

            _mockCategoryRepository.Setup(repo => repo.GetQuery())
                .Returns(mockQuery.Object);

            _mockMapper.Setup(m => m.Map<Category>(categoryCreateDto))
                .Returns(category);
            _mockCategoryRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _categoryService.AddAsync(categoryCreateDto);

            // Assert
            Assert.That(result, Is.EqualTo(categoryCreateDto));
            _mockCategoryRepository.Verify(repo => repo.AddAsync(category), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void AddAsync_WithNullCategory_ThrowsArgumentNullException()
        {
            // Arrange
            CategoryCreatDto category = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _categoryService.AddAsync(category));
        }

        [Test]
        public void AddAsync_WithDuplicateName_ThrowsResourceUniqueException()
        {
            // Arrange
            var categoryCreateDto = new CategoryCreatDto { Name = "Test Category" };

            var categoryData = new List<Category>
            {
                new Category { Id = 1, Name = "Test Category" }
            }.AsQueryable();

            var mockQuery = new Mock<DbSet<Category>>();
            mockQuery.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categoryData.Provider);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categoryData.Expression);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categoryData.ElementType);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categoryData.GetEnumerator());

            _mockCategoryRepository.Setup(repo => repo.GetQuery())
                .Returns(mockQuery.Object);

            // Act & Assert
            Assert.ThrowsAsync<ResourceUniqueException>(() => _categoryService.AddAsync(categoryCreateDto));
        }

        [Test]
        public void AddAsync_WithSaveChangesFailed_ThrowsDatabaseBadRequestException()
        {
            // Arrange
            var categoryCreateDto = new CategoryCreatDto { Name = "Test Category" };
            var category = new Category { Id = 1, Name = "Test Category" };

            var categoryData = new List<Category>().AsQueryable(); // Empty list means no duplicates
            var mockQuery = new Mock<DbSet<Category>>();
            mockQuery.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categoryData.Provider);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categoryData.Expression);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categoryData.ElementType);
            mockQuery.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categoryData.GetEnumerator());

            _mockCategoryRepository.Setup(repo => repo.GetQuery())
                .Returns(mockQuery.Object);

            _mockMapper.Setup(m => m.Map<Category>(categoryCreateDto))
                .Returns(category);
            _mockCategoryRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(-1);

            // Act & Assert
            Assert.ThrowsAsync<DatabaseBadRequestException>(() => _categoryService.AddAsync(categoryCreateDto));
        }


        [Test]
        public async Task DeleteAsync_WithValidId_DeletesCategory()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Test Category" };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync(category);
            _mockCategoryRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _categoryService.DeleteAsync(categoryId);

            // Assert
            _mockCategoryRepository.Verify(repo => repo.DeleteAsync(category), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteAsync_WithInvalidId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int categoryId = 99;
            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _categoryService.DeleteAsync(categoryId));
        }

        [Test]
        public void DeleteAsync_WithSaveChangesFailed_ThrowsDatabaseBadRequestException()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Test Category" };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync(category);
            _mockCategoryRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(-1);

            // Act & Assert
            Assert.ThrowsAsync<DatabaseBadRequestException>(() => _categoryService.DeleteAsync(categoryId));
        }



        [Test]
        public async Task GetAllAsync_WithValidParameters_ReturnsPaginatedList()
        {
            // Arrange
            int pageSize = 10;
            int pageIndex = 1;
            int totalCount = 20;
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Category 1" },
                new CategoryDto { Id = 2, Name = "Category 2" }
            };

            _mockCategoryRepository.Setup(repo => repo.GetAllCategoriesAsync(It.IsAny<CategorySpecification>()))
                .ReturnsAsync((categories, totalCount));
            _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(categoryDtos);

            // Act
            var result = await _categoryService.GetAllAsync(pageSize, pageIndex);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.PageSize, Is.EqualTo(pageSize));
            Assert.That(result.PageIndex, Is.EqualTo(pageIndex));
            Assert.That(result.TotalCount, Is.EqualTo(totalCount));
            Assert.That(result.Items.Count, Is.EqualTo(categoryDtos.Count));
        }

        [Test]
        public void GetAllAsync_WithNoCategories_ThrowsResourceNotFoundException()
        {
            // Arrange
            _mockCategoryRepository.Setup(repo => repo.GetAllCategoriesAsync(It.IsAny<CategorySpecification>()))
                .ReturnsAsync((new List<Category>(), 0));

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _categoryService.GetAllAsync(10, 1));
        }

        [Test]
        public void GetAllAsync_WithNullCategories_ThrowsResourceNotFoundException()
        {
            // Arrange
            _mockCategoryRepository.Setup(repo => repo.GetAllCategoriesAsync(It.IsAny<CategorySpecification>()))
                .ReturnsAsync((null, 0));

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _categoryService.GetAllAsync(10, 1));
        }



        [Test]
        public async Task GetByIdAsync_WithValidId_ReturnsCategory()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Test Category" };
            var categoryDto = new CategoryDto { Id = categoryId, Name = "Test Category" };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync(category);
            _mockMapper.Setup(m => m.Map<CategoryDto>(category))
                .Returns(categoryDto);

            // Act
            var result = await _categoryService.GetByIdAsync(categoryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(categoryDto.Id));
            Assert.That(result.Name, Is.EqualTo(categoryDto.Name));
        }

        [Test]
        public void GetByIdAsync_WithInvalidId_ThrowsResourceNotFoundException()
        {
            // Arrange
            int categoryId = 99;
            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _categoryService.GetByIdAsync(categoryId));
        }



        [Test]
        public async Task UpdateAsync_WithValidCategory_UpdatesCategory()
        {
            // Arrange
            var categoryUpdateDto = new CategoryUpdateDto { Id = 1, Name = "Updated Category" };
            var existingCategory = new Category { Id = 1, Name = "Test Category" };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryUpdateDto.Id))
                .ReturnsAsync(existingCategory);
            _mockCategoryRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _categoryService.UpdateAsync(categoryUpdateDto);

            // Assert
            _mockMapper.Verify(m => m.Map(categoryUpdateDto, existingCategory), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.UpdateAsync(existingCategory), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateAsync_WithNullCategory_ThrowsArgumentNullException()
        {
            // Arrange
            CategoryUpdateDto category = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _categoryService.UpdateAsync(category));
        }

        [Test]
        public void UpdateAsync_WithInvalidId_ThrowsResourceNotFoundException()
        {
            // Arrange
            var categoryUpdateDto = new CategoryUpdateDto { Id = 99, Name = "Updated Category" };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryUpdateDto.Id))
                .ReturnsAsync((Category)null);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _categoryService.UpdateAsync(categoryUpdateDto));
        }

        [Test]
        public void UpdateAsync_WithSaveChangesFailed_ThrowsDatabaseBadRequestException()
        {
            // Arrange
            var categoryUpdateDto = new CategoryUpdateDto { Id = 1, Name = "Updated Category" };
            var existingCategory = new Category { Id = 1, Name = "Test Category" };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryUpdateDto.Id))
                .ReturnsAsync(existingCategory);
            _mockCategoryRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(-1);

            // Act & Assert
            Assert.ThrowsAsync<DatabaseBadRequestException>(() => _categoryService.UpdateAsync(categoryUpdateDto));
        }

    }
}