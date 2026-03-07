using Repositeries;
using Entities;
using DTOs;

namespace Service
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetUsers();
        Task<UserDTO> GetUserById(int id);
        Task<UserDTO> AddUser(UserRegisterDTO user);
        Task<UserPublicDTO?> LogIn(UserLoginDTO exestingUser);
        Task UpdateUser(int id, UserDTO updateUser);
       
    }
}