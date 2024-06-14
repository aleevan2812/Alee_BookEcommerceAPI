using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
using Alee_BookEcommerceAPI.Model.Dto.ProductImage;
using AutoMapper;

namespace Alee_BookEcommerceAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Category, CategoryCreateDTO>().ReverseMap();
        CreateMap<Category, CategoryUpdateDTO>().ReverseMap();
        CreateMap<Category, CategoryDTO>().ReverseMap();

        CreateMap<ProductCreateDTO, Product>().ForMember(dest => dest.ProductImages,  opt => opt.Ignore());
        
        CreateMap<ProductUpdateDTO, Product>().ForMember(dest => dest.ProductImages,  opt => opt.Ignore());
        CreateMap<Product, ProductDTO>().ReverseMap();
        
        CreateMap<ProductImage, ProductImageCreateDTO>().ReverseMap();
        CreateMap<ProductImage, ProductImageDTO>().ReverseMap();
    }
}