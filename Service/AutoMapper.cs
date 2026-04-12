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
            //CreateMap<UserDTO, User>();
            //CreateMap<UserLoginDTO, User>();
            //CreateMap<UserRegisterDTO, User>();
            ////CreateMap<Product, ProductDTO>().ReverseMap();
            //CreateMap<Category, CategoryDTO>().ReverseMap();
            //CreateMap<Product, ProductDTO>();
            //CreateMap<Order, OrderDTO>().ReverseMap();
            //CreateMap<User, UserDTO>(); 
            //CreateMap<User, UserPublicDTO>();
            //CreateMap<User, UserDTO>()
            //        .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId));
            //CreateMap<OrdersItem, OrderItemDTO>()
            //    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products.ProductsName))
            //    .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Products.ImgUrl))
            //    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Products.Price));

            //CreateMap<Order, OrderDTO>();
            // משתמשים
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserPublicDTO>();
            CreateMap<UserLoginDTO, User>();
            CreateMap<UserRegisterDTO, User>();

            // מוצרים וקטגוריות
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();

            // המיפוי הקריטי להזמנות - שימי לב ל-ProductName ו-ProductImage
            CreateMap<OrdersItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products.ProductsName))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Products.ImgUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Products.Price));

            // מיפוי הזמנה ראשית
            CreateMap<Order, OrderDTO>().ReverseMap();

            // מיפוי בין Product ל-ProductDTO
            CreateMap<Product, ProductDTO>()
                // אם ב-Entity זה Descreption (עם שגיאת כתיב) וב-DTO זה משהו אחר, הגדירי כך:
                .ForMember(dest => dest.ProductsDescreption, opt => opt.MapFrom(src => src.ProductsDescreption))
                // הוספת ReverseMap מאפשרת מיפוי דו-כיווני (מה-DTO ל-Entity ולהיפך)
                .ReverseMap();

            // ודאי שיש לך גם מיפוי לקטגוריה אם את מציגה שם קטגוריה בניהול
            CreateMap<Category, CategoryDTO>().ReverseMap();

        }
    }
}


