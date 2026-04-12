using Entities;
using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface IUserService
    {
        Task<User> addUserServices(User user);
        Task<UserDTO> GetById(int id);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<User> loginServices(User user);
        Task update(UserDTO userDto, int id);
    }
}