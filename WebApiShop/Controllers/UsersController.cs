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
        private IUserService _userservice;
        private ILogger<UsersController> _logger;

        public UsersController(IUserService userservice, ILogger<UsersController> logger)
        {
            _userservice = userservice;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Post([FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = new User
            {
                UserName = userDto.UserName, 
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Password = userDto.Password,
                Phone = userDto.Phone,
                Address = userDto.Address
            };

            User acceptedUser = await _userservice.addUserServices(user);

            if (acceptedUser == null)
            {
                return BadRequest("סיסמה חלשה או משתמש כבר קיים במערכת");
            }

            return Ok(acceptedUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] User user)
        {

            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password)) 
            {
                return BadRequest("חובה להזין שם משתמש וסיסמה"); 
            }

            _logger.LogInformation($"Attempting login for: UserName='{user.UserName}'"); 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User _user = await _userservice.loginServices(user);

            if (_user == null)
            {
                _logger.LogWarning($"Login failed for: {user.UserName}"); 
                return Unauthorized("פרטי התחברות שגויים או משתמש לא קיים");
            }

            _logger.LogInformation($"Login success: UserName={_user.UserName}"); 

            return Ok(_user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDTO userDto)
        {
            await _userservice.update(userDto, id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            UserDTO user = await _userservice.GetById(id);
            if (user == null)
                return NoContent();
            return Ok(user);
        }

    }
}