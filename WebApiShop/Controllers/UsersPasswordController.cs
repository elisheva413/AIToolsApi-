//using Microsoft.AspNetCore.Mvc;
//using Repositeries;
//using Entities;
//using Service;
//using System.Collections.Generic;
//using System.Text.Json;


//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace WebApiShop.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersPasswordController : ControllerBase
//    {
//        IUserPasswordService _userPasswordService;

//        public UsersPasswordController(IUserPasswordService userPassword)
//        {
//            _userPasswordService = userPassword;
//        }

//        // GET: api/<UsersPasswordController>
//        [HttpGet]
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET api/<UsersPasswordController>/5
//        [HttpGet("{id}")]
//        public string Get(UserPassword password)
//        {
//            return "value";

//        }

//        // POST api/<UsersPasswordController>
//        [HttpPost]
//        public ActionResult<int> CheckPassword([FromBody] UserPassword password)
//        {
//            int score = _userPasswordService.CheckPassword(password.Password);
//            if (score > 1)
//                return Ok(score);
//            return BadRequest();
//        }

//        // PUT api/<UsersPasswordController>/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE api/<UsersPasswordController>/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
//    }
//}
using Entities;
using Microsoft.AspNetCore.Mvc;
using Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordsController : ControllerBase
    {
        IUserPasswordService _passwordservice;//

        public PasswordsController(IUserPasswordService passwordservice)
        {
            _passwordservice = passwordservice;
        }


        [HttpGet]
        public void Get(string pass)
        {

        }

        // GET api/<passworsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<passworsController>
        [HttpPost]
        public ActionResult<UserPassword> Post([FromBody] string value)
        {

            UserPassword resPas = _passwordservice.Level(value);
            if (resPas == null)
                return NoContent();
            return Ok(resPas);
        }

        // PUT api/<passworsController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("הסיסמה החדשה לא יכולה להיות ריקה.");
            }

            bool isUpdated = _passwordservice.UpdatePassword(id, newPassword);

            if (isUpdated)
            {
                return Ok($"הסיסמה למשתמש {id} עודכנה בהצלחה.");
            }
            else
            {
                return BadRequest("הסיסמה שנבחרה חלשה מדי. נדרש חוזק של 3 ומעלה.");
            }

            // DELETE api/<passworsController>/5
            //[HttpDelete("{id}")]
            //public void Delete(int id)
            //{

            //}
        }
    }
}