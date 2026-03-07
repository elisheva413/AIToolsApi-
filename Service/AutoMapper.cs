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
            CreateMap<UserDTO, User>();
            CreateMap<UserLoginDTO, User>();
            CreateMap<UserRegisterDTO, User>();
            //CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<User, UserDTO>();
            CreateMap<User, UserPublicDTO>();

        }

    }
}


