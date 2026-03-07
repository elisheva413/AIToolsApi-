using AutoMapper;
using DTOs;
using Entities;
using Repositeries;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRipository _userRipository;
        IMapper _mapper;

        public UserService(IUserRipository userRipository, IMapper mapper)
        {
            _userRipository = userRipository;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            List<User> users = await _userRipository.GetUsers();
            List<UserDTO>usersDTO =_mapper.Map<List<User>, List < UserDTO >> (users);
            return usersDTO;
        }

        public async Task<UserDTO> GetUserById(int id)
        {
            User userByID = await _userRipository.GetUserById(id);
            UserDTO userDtoByID = _mapper.Map<User, UserDTO>(userByID);
            return userDtoByID;
        }

        public async Task<UserDTO> AddUser(UserRegisterDTO newUser)
        {
            User userRegister = _mapper.Map<UserRegisterDTO, User>(newUser);
            User user = await _userRipository.AddUser(userRegister);
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            return userDTO;
        }

        public async Task<UserPublicDTO?> LogIn(UserLoginDTO existingUser)
        {
            var user = await _userRipository.LogIn(existingUser.UserName, existingUser.Password);
            if (user == null)
                return null;

            return _mapper.Map<UserPublicDTO>(user);
        }


        public async Task UpdateUser(int id, UserDTO updateUser)
        {
            User user = _mapper.Map<UserDTO, User>(updateUser);
            await _userRipository.UpdateUser(id, user);
        }
    }
}