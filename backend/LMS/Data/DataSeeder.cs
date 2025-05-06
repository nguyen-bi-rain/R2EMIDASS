using LMS.Models.Enums;
using LMS.Services.Interfaces;
using LMS.DTOs;

namespace LMS.Data
{
    public class DataSeeder
    {
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;

        public DataSeeder(
            IUserService userService,
            ICategoryService categoryService,
            IBookService bookService)
        {
            _userService = userService;
            _categoryService = categoryService;
            _bookService = bookService;
        }

        public async Task SeedDataAsync()
        {
            await SeedCategoriesAsync();
            await SeedUsersAsync();
            await SeedBooksAsync();
        }

        private async Task SeedCategoriesAsync()
        {
            if ((await _categoryService.GetAllAsync()).Items.Count > 0)
            {
                return; // Categories already exist, no need to seed
            }
            var categories = new List<CategoryCreatDto>
            {
                new CategoryCreatDto { Name = "Fiction", Description = "Literary works created from imagination" },
                new CategoryCreatDto { Name = "Science Fiction", Description = "Fiction dealing with scientific advancements and the future" },
                new CategoryCreatDto { Name = "Mystery", Description = "Fiction dealing with the solution of a crime or puzzle" },
                new CategoryCreatDto { Name = "Biography", Description = "Account of someone's life written by someone else" },
                new CategoryCreatDto { Name = "Self-Help", Description = "Books aimed at helping readers improve their lives" }
            };

            foreach (var category in categories)
            {
                await _categoryService.AddAsync(category);
            }
        }

        private async Task SeedUsersAsync()
        {
            // Create admin user
            var adminUser = new RegisterRequest
            {
                UserName = "admin",
                Email = "admin@library.com",
                PhoneNumber = "1234567890",
                Gender = Gender.Other,
                Password = "Admin123@",
            };

            // Create regular user
            var regularUser = new RegisterRequest
            {
                UserName = "user",
                Email = "user@library.com",
                PhoneNumber = "0987654321",
                Gender = Gender.Other,
                Password = "Test123@"
            };

            await _userService.RegisterAsync(regularUser);
            await _userService.RegisterAsync(adminUser);
        }

        private async Task SeedBooksAsync()
        {
            if((await _bookService.GetAllBookAsync()).Items.Count > 0)
            {
                return; // Books already exist, no need to seed
            }

            var categories = await _categoryService.GetAllAsync();
            var categoryDict = categories.Items.ToDictionary(c => c.Name, c => c.Id);

            var books = new List<BookRequest>
            {
                new BookRequest
                {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Description = "A classic novel about racial inequality in the American South",
                    Quantity = 10,
                    Available = 10,
                    PublishedDate = new DateTime(1960, 7, 11, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryDict["Fiction"]
                },
                new BookRequest
                {
                    Title = "1984",
                    Author = "George Orwell",
                    Description = "A dystopian novel set in a totalitarian society",
                    Quantity = 15,
                    Available = 15,
                    PublishedDate = new DateTime(1949, 6, 8, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryDict["Fiction"]
                },
                new BookRequest
                {
                    Title = "Dune",
                    Author = "Frank Herbert",
                    Description = "A science fiction novel set in a distant future",
                    Quantity = 8,
                    Available = 8,
                    PublishedDate = new DateTime(1965, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryDict["Science Fiction"]
                },
                new BookRequest
                {
                    Title = "Steve Jobs",
                    Author = "Walter Isaacson",
                    Description = "Biography of Apple co-founder Steve Jobs",
                    Quantity = 12,
                    Available = 12,
                    PublishedDate = new DateTime(2011, 10, 24, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryDict["Biography"]
                },
                new BookRequest
                {
                    Title = "The Girl with the Dragon Tattoo",
                    Author = "Stieg Larsson",
                    Description = "A mystery novel about a journalist and a hacker",
                    Quantity = 7,
                    Available = 7,
                    PublishedDate = new DateTime(2005, 8, 16, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = categoryDict["Mystery"]
                }
            };

            foreach (var book in books)
            {
                await _bookService.AddBookAsync(book);
            }
        }

        // Extension method to register the seeder in Program.cs

    }
    public static class DataSeederExtensions
    {
        public static async Task SeedData(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.SeedDataAsync();
        }

        public static IServiceCollection AddDataSeeder(this IServiceCollection services)
        {
            services.AddTransient<DataSeeder>();
            return services;
        }
    }
}
