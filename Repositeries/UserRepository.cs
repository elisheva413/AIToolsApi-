//using Entities;
//using Microsoft.EntityFrameworkCore;
//using Repositeries;
//using System.Reflection.PortableExecutable;
//using System.Text.Json;
//using System.Threading.Tasks;



//namespace Repositeries
//{
//    public class UserRipository :  IUserRipository
//    {
//        Store_215962135Context _store_215962135Context;
//        public UserRipository(Store_215962135Context store_215962135Context)
//        {
//            _store_215962135Context = store_215962135Context;
//        }


//        public async Task<List<User>> GetUsers()
//        {
//            return await _store_215962135Context.Users.ToListAsync();
//        }
//        public async Task<User?> GetUserById(int id)
//        {
//            return await _store_215962135Context.Users.FindAsync(id);
//        }

//        public async Task<User> AddUser(User user)
//        {
//            await _store_215962135Context.Users.AddAsync(user);
//            await _store_215962135Context.SaveChangesAsync();
//            return user;
//        }
//        public async Task<User?> LogIn(string userName, string password)
//        {
//            return await _store_215962135Context.Users
//                .FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
//        }

//        public async Task UpdateUser(int id, User updateUser)
//        {
//            var existingUser = await _store_215962135Context.Users.FindAsync(id);

//            if (existingUser == null)
//                throw new Exception("User not found");

//            existingUser.FirstName = updateUser.FirstName;
//            existingUser.LastName = updateUser.LastName;
//            existingUser.UserName = updateUser.UserName;
//            existingUser.Password = updateUser.Password;
//            existingUser.Phone = updateUser.Phone;
//            existingUser.Address = updateUser.Address;

//            await _store_215962135Context.SaveChangesAsync();
//        }




//    }
//}
using Entities; 
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repositeries 
{
    public class UserRepository : IUserRepository
    {
        private readonly Store_215962135Context _store_215962135Context;

        public UserRepository(Store_215962135Context store_215962135Context)
        {
            _store_215962135Context = store_215962135Context;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _store_215962135Context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _store_215962135Context.Users.FindAsync(id);
        }

        public async Task<User> AddUser(User user)
        {
            await _store_215962135Context.Users.AddAsync(user);

          
            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = "User";
            }

            await _store_215962135Context.SaveChangesAsync();
            return user;
        }
        public async Task<ActionResult<User>> Put(int id, [FromBody] User updatedUser)
        {
            var existingUser = await _store_215962135Context.Users.FindAsync(id);

            if (existingUser != null)
            {
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.Phone = updatedUser.Phone;
                existingUser.Address = updatedUser.Address;
                existingUser.UserName = updatedUser.UserName;
                existingUser.Password = updatedUser.Password;

                _store_215962135Context.Users.Update(existingUser);
                await _store_215962135Context.SaveChangesAsync();
            }

            return existingUser;
        }

        public async Task<User> Login(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return null;
            }

            string userName = user.UserName.Trim();
            string password = user.Password;

            return await _store_215962135Context.Users
                .FirstOrDefaultAsync(x => x.UserName.Trim() == userName && x.Password == password);
        }
    }
}
