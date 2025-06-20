using AutoMapper;
using Library.Core.Dtos.BookDto;
using Library.Core.Dtos.RentalDto;
using Library.Core.Dtos.UserDto;
using Library.Core.Entities;

namespace Library.Service.Mapping;

public class LibraryManagementMapping : Profile
{
    public LibraryManagementMapping()
    {
        CreateMap<ApplicationUser, UserDto>().ReverseMap();

        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.RentalHistory,
                opt => opt.MapFrom(src => src.Rentals));

        CreateMap<Rental, BookRentalDto>()
            .ForMember(dest => dest.BookTitle,
                opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.ISBN,
                opt => opt.MapFrom(src => src.Book.ISBN));

        CreateMap<Book, BookDto>().ReverseMap();
        CreateMap<Book, BookCreateUpdateDto>().ReverseMap();
        
    }
}
