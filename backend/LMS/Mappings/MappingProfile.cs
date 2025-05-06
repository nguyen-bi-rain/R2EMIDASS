using AutoMapper;
using LMS.DTOs;
using LMS.Models.Enitties;

namespace LMS.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CategoryCreatDto, Category>().ReverseMap();
        CreateMap<CategoryUpdateDto, Category>().ReverseMap();
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => 3));
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Book,BookResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<BookRequest,Book>().ReverseMap();
        CreateMap<Book,BookDto>().ReverseMap();
        CreateMap<BookUpdateRequest,Book>().ReverseMap();

        CreateMap<BookBorrowingRequestDtoCreate,BookBorrowingRequest>()
            .ForMember(dest => dest.BookBorrowingRequestDetails, opt => opt.MapFrom(src => src.BookBorrowingRequestDetails))
            .ForMember(dest => dest.DateRequest, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.DateExpired, opt => opt.MapFrom(src => DateTime.Now.AddDays(30)))
            .ReverseMap();
        CreateMap<BookBorrowingRequest,BookBorrowingRequestDtoUpdate>().ReverseMap();
        CreateMap<BookBorrowingRequestDetails,BookBorrowingRequestDetailsDtoCreate>().ReverseMap();
        CreateMap<BookBorrowingRequestDetails,BookBorrowingRequestDetailsDto>()
            .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
        .ReverseMap();
        CreateMap<BookBorrowingRequest,BookBorrowingRequestDto>()
            .ForMember(dest => dest.BookBorrowingRequestDetails, opt => opt.MapFrom(src => src.BookBorrowingRequestDetails))
            .ForMember(dest => dest.DateRequest, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.DateExpired, opt => opt.MapFrom(src => DateTime.Now.AddDays(30)))
            .ReverseMap();
        
    }
}