using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
using AutoMapper;

namespace Alee_BookEcommerceAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Category, CategoryCreateDTO>().ReverseMap();
        CreateMap<Category, CategoryUpdateDTO>().ReverseMap();
        CreateMap<Category, CategoryDTO>().ReverseMap();

        CreateMap<Product, ProductCreateDTO>().ReverseMap();
        CreateMap<Product, ProductUpdateDTO>().ReverseMap();
        CreateMap<Product, ProductDTO>().ReverseMap();
    }
}