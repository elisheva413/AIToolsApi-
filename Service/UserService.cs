//using AutoMapper;
//using DTOs;
//using Entities;
//using Repositeries;
//using System.Collections.Generic;
//using System.Runtime.Intrinsics.X86;

//namespace Service
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserRipository _userRipository;
//        IMapper _mapper;

//        public UserService(IUserRipository userRipository, IMapper mapper)
//        {
//            _userRipository = userRipository;
//            _mapper = mapper;
//        }

//        public async Task<List<UserDTO>> GetUsers()
//        {
//            List<User> users = await _userRipository.GetUsers();
//            List<UserDTO>usersDTO =_mapper.Map<List<User>, List < UserDTO >> (users);
//            return usersDTO;
//        }

//        public async Task<UserDTO> GetUserById(int id)
//        {
//            User userByID = await _userRipository.GetUserById(id);
//            UserDTO userDtoByID = _mapper.Map<User, UserDTO>(userByID);
//            return userDtoByID;
//        }

//        public async Task<UserDTO> AddUser(UserRegisterDTO newUser)
//        {
//            User userRegister = _mapper.Map<UserRegisterDTO, User>(newUser);
//            User user = await _userRipository.AddUser(userRegister);
//            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
//            return userDTO;
//        }

//        public async Task<UserPublicDTO?> LogIn(UserLoginDTO existingUser)
//        {
//            var user = await _userRipository.LogIn(existingUser.UserName, existingUser.Password);
//            if (user == null)
//                return null;

//            return _mapper.Map<UserPublicDTO>(user);
//        }


//        public async Task UpdateUser(int id, UserDTO updateUser)
//        {
//            User user = _mapper.Map<UserDTO, User>(updateUser);
//            await _userRipository.UpdateUser(id, user);
//        }
//    }
//}

using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositeries;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserPasswordService _passwordservice;
        private readonly IMapper _imapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IUserPasswordService passwordservice, IMapper imapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordservice = passwordservice;
            _imapper = imapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UserPublicDTO>> GetUsers()
        {
            IEnumerable<User> users = await _userRepository.GetUsers();
            return _imapper.Map<IEnumerable<User>, IEnumerable<UserPublicDTO>>(users);
        }

        public async Task<UserPublicDTO> GetById(int id)
        {
            User user = await _userRepository.GetById(id);
            UserPublicDTO userpublicDto = _imapper.Map<User, UserPublicDTO>(user);
            return userpublicDto;
        }
        public async Task<bool> IsUserNameExists(string userName)
        {
            return await _userRepository.IsUserNameExists(userName);
        }

        public async Task<AuthResponseDTO> addUserServices(UserRegisterDTO registerDTO)
        {
            User user = _imapper.Map<UserRegisterDTO,User>(registerDTO);
            User createdUser = await _userRepository.AddUser(user);
            UserPublicDTO userDto = _imapper.Map<User, UserPublicDTO>(createdUser);
            (string token, DateTime expiresAtUtc) = GenerateJwtToken(createdUser);
            return new AuthResponseDTO(userDto, token, expiresAtUtc);
           
        }

        public async Task<AuthResponseDTO> loginServices(UserLoginDTO LoginDTO)
        {

            User usertoLogin = _imapper.Map<UserLoginDTO,User>(LoginDTO);
            User loggedInUser = await _userRepository.Login(usertoLogin);
            if (loggedInUser == null)
                return null;

            UserPublicDTO userDto = _imapper.Map<User,UserPublicDTO>(loggedInUser);
            (string token, DateTime expiresAtUtc) = GenerateJwtToken(loggedInUser);
            return new AuthResponseDTO(userDto, token, expiresAtUtc);
        }

     
        public async Task update(UserRegisterDTO userDto, int id)
        {
            User user = _imapper.Map<UserRegisterDTO,User>(userDto);
            await _userRepository.Put(id, user);
        }

        private (string token, DateTime expiresAtUtc) GenerateJwtToken(User user)
        {
            string key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing in configuration.");
            string issuer = _configuration["Jwt:Issuer"] ?? "WebApiShop";
            string audience = _configuration["Jwt:Audience"] ?? "WebApiShopClient";
            int expiryMinutes = _configuration.GetValue<int?>("Jwt:ExpiryMinutes") ?? 60;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.UserName ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Role, string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role),
                new("firstName", user.FirstName ?? string.Empty),
                new("lastName", user.LastName ?? string.Empty)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return (token, expiresAtUtc);
        }
    }
}