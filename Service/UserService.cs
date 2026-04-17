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
        private readonly IUserRepository _IuserRepository;
        private readonly IUserPasswordService _Ipasswordservice;
        private readonly IMapper _imapper;

        public UserService(IUserRepository userRepository, IUserPasswordService passwordservice, IMapper imapper)
        {
            _IuserRepository = userRepository;
            _Ipasswordservice = passwordservice;
            _imapper = imapper;
        }

        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            IEnumerable<User> users = await _IuserRepository.GetUsers();
            IEnumerable<UserDTO> usersDto = _imapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(users);
            return usersDto;
        }

        public async Task<UserDTO> GetById(int id)
        {
            User user = await _IuserRepository.GetById(id);
            UserDTO userDto = _imapper.Map<User, UserDTO>(user);
            return userDto;
        }

        //public async Task<User> addUserServices(User user)
        //{
        //    // לוגיקת בדיקת חוזק סיסמה של חברה שלך
        //    int score = _Ipasswordservice.Level(user.Password).Strength;
        //    if (score < 2)
        //        return null;

        //    return await _IuserRepository.AddUser(user);
        //}
        ///שזהבי תוסיף...
        public async Task<User> addUserServices(User user)
        {
            var usersList = await _IuserRepository.GetUsers();
            var existingUser = usersList.FirstOrDefault(u => u.UserName == user.UserName);

            if (existingUser != null)
            {
                return null;
            }

            int score = _Ipasswordservice.Level(user.Password).Strength;
            if (score < 2)
                return null;

            
            return await _IuserRepository.AddUser(user);
        }

        public async Task<User> loginServices(User user)
        {
            return await _IuserRepository.Login(user);
        }

     
        public async Task update(UserDTO userDto, int id)
        {
            User user = _imapper.Map<User>(userDto);
            await _IuserRepository.Put(id, user);
        }
    }
}