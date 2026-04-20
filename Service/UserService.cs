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
using Repositeries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserPasswordService _passwordservice;
        private readonly IMapper _imapper;

        public UserService(IUserRepository userRepository, IUserPasswordService passwordservice, IMapper imapper)
        {
            _userRepository = userRepository;
            _passwordservice = passwordservice;
            _imapper = imapper;
        }

        public async Task<IEnumerable<UserPublicDTO>> GetUsers()
        {
            IEnumerable<User> users = await _userRepository.GetUsers();
            return _imapper.Map<IEnumerable<User>, IEnumerable<UserPublicDTO>>(users);
            //IEnumerable<User> usersDto = _imapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(users);
            //return usersDto;
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

        public async Task<UserPublicDTO> addUserServices(UserRegisterDTO registerDTO)
        {
            User user = _imapper.Map<UserRegisterDTO,User>(registerDTO);
            User createdUser = await _userRepository.AddUser(user);
            return _imapper.Map<User, UserPublicDTO>(createdUser);
            //var usersList = await _IuserRepository.GetUsers();
            //var existingUser = usersList.FirstOrDefault(u => u.UserName == user.UserName);

            //if (existingUser != null)
            //{
            //    return null;
            //}

            //int score = _Ipasswordservice.Level(user.Password).Strength;
            //if (score < 2)
            //    return null;


            //return await _IuserRepository.AddUser(existingUser);
        }

        public async Task<UserPublicDTO> loginServices(UserLoginDTO LoginDTO)
        {

            User usertoLogin = _imapper.Map<UserLoginDTO,User>(LoginDTO);
            User loggedInUser = await _userRepository.Login(usertoLogin);
            if (loggedInUser == null)
                return null;
            return _imapper.Map<User,UserPublicDTO>(loggedInUser);
        }

     
        public async Task update(UserRegisterDTO userDto, int id)
        {
            User user = _imapper.Map<UserRegisterDTO,User>(userDto);
            await _userRepository.Put(id, user);
        }
    }
}