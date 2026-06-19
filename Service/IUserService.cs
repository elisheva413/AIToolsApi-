using Entities;
using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface IUserService
    {
        Task<AuthResponseDTO> addUserServices(UserRegisterDTO registerDTO);
        Task<UserPublicDTO> GetById(int id);
        Task<IEnumerable<UserPublicDTO>> GetUsers();
        Task<AuthResponseDTO> loginServices(UserLoginDTO loginDTO);
        Task update(UserRegisterDTO userDto, int id);
        Task<bool> IsUserNameExists(string userName);
    }
}