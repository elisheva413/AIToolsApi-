using AutoMapper;
using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AutoMappering : Profile
    {
        public AutoMappering()
        {
        
            CreateMap<User, UserPublicDTO>().ReverseMap();
            CreateMap<UserLoginDTO, User>();
            CreateMap<UserRegisterDTO, User>();
            CreateMap<UserPublicDTO, UserLoginDTO>();

            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<OrdersItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products.ProductsName))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Products.ImgUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Products.Price));

            
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ProductsDescreption, opt => opt.MapFrom(src => src.ProductsDescreption))
                .ReverseMap();

            CreateMap<Category, CategoryDTO>().ReverseMap();

        }
    }
}


