//using Microsoft.AspNetCore.Mvc;
//using Entities;
//using System.Collections.Generic;
//using Repositeries;
//using Service;
//using DTOs;
//using NLog.Web;
//using System.Linq;




//namespace WebApiShop.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly IUserPasswordService _userPasswordService;
//        private readonly ILogger<UsersController> _logger;

//        public UsersController(IUserService userService, IUserPasswordService userPasswordService, ILogger<UsersController> logger)
//        {
//            _userService = userService;
//            _userPasswordService = userPasswordService;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
//        {
//            var users = await _userService.GetUsers();
//            if (users == null || !users.Any())
//                return NoContent();
//            return Ok(users);
//        }

//        [HttpGet("{id}")]

//        public async Task<ActionResult<UserDTO>> GetById(int id)
//        {
//            var userDto = await _userService.GetUserById(id);
//            if (userDto == null)
//                return NotFound();
//            return Ok(userDto);
//        }


//        [HttpPost]
//        public async Task<ActionResult<UserDTO>> AddUser([FromBody] UserRegisterDTO newUser)
//        {
//            int passwordScore = _userPasswordService.CheckPassword(newUser.Password);
//            if (passwordScore < 2)
//                return BadRequest("Password is too weak.");
//            var user = await _userService.AddUser(newUser);
//            return CreatedAtAction(nameof(GetById), new { id = user.UserId}, user);
//        }

//        [HttpPost("login")]
//        public async Task<ActionResult<UserPublicDTO>> LogIn([FromBody] UserLoginDTO existingUser)
//        {
//            var user = await _userService.LogIn(existingUser);
//            if (user == null)
//                return Unauthorized("Invalid credentials.");

//            _logger.LogInformation($"Login attempted with User Name {existingUser.UserName}");
//            return Ok(user);
//        }


//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put(int id, [FromBody] UserDTO updateUser)
//        {
//            int passwordScore = _userPasswordService.CheckPassword(updateUser.Password);
//            if (passwordScore < 2)
//                return BadRequest("Password is too weak.");

//            await _userService.UpdateUser(id, updateUser);
//            return NoContent();
//        }


//    }
//}

using Microsoft.AspNetCore.Mvc;
using Service;
using Repositeries;
using Entities;
using System.Threading.Tasks;
using DTOs;
using Microsoft.Extensions.Logging;

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserPasswordService _passwordService;



        public UsersController(IUserService userservice, ILogger<UsersController> logger, IUserPasswordService passwordService)
        {
            _userservice = userservice;
            _passwordService = passwordService; 
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserPublicDTO>> Post([FromBody] UserRegisterDTO userRegistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int passwordScore = _passwordService.Level(userRegistDto.Password).Strength;
            if (passwordScore < 2)
            {
                return BadRequest("הסיסמה חלשה מדי. אנא בחר סיסמה חזקה יותר.");
            }

            bool isExists = await _userservice.IsUserNameExists(userRegistDto.UserName);
            if (isExists)
            {
                return BadRequest(" המשתמש כבר קיים במערכת. אנא בחר שם משתמש אחר.");
            }
          
            UserPublicDTO acceptedUser = await _userservice.addUserServices(userRegistDto);

            return CreatedAtAction(nameof(Get), new { id = acceptedUser.UserId }, acceptedUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserPublicDTO>> Login([FromBody] UserLoginDTO userLoginDTO)
        {

            if (string.IsNullOrEmpty(userLoginDTO.UserName) || string.IsNullOrEmpty(userLoginDTO.Password)) 
            {
                return BadRequest("חובה להזין שם משתמש וסיסמה"); 
            }

            _logger.LogInformation($"Attempting login for: UserName='{userLoginDTO.UserName}'"); 

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserPublicDTO _user = await _userservice.loginServices(userLoginDTO);

            if (_user == null)
            {
                _logger.LogWarning($"Login failed for: {userLoginDTO.UserName}"); 
                return Unauthorized("פרטי התחברות שגויים או משתמש לא קיים");
            }

            _logger.LogInformation($"Login success: UserName={_user.UserName}"); 

            return Ok(_user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserRegisterDTO userRegistDto)
        {
            int passwordScore = _passwordService.Level(userRegistDto.Password).Strength;
            if (passwordScore < 2)
            {
                return BadRequest("הסיסמה חלשה מדי.");
            }
            await _userservice.update(userRegistDto, id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserPublicDTO>> Get(int id)
        {
            UserPublicDTO user = await _userservice.GetById(id);
            if (user == null)
                return NoContent();
            return Ok(user);
        }

    }
}