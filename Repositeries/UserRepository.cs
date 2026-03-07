using Entities;
using Microsoft.EntityFrameworkCore;
using Repositeries;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Threading.Tasks;



namespace Repositeries
{
    public class UserRipository :  IUserRipository
    {
        Store_215962135Context _store_215962135Context;
        public UserRipository(Store_215962135Context store_215962135Context)
        {
            _store_215962135Context = store_215962135Context;
        }


        public async Task<List<User>> GetUsers()
        {
            return await _store_215962135Context.Users.ToListAsync();
        }
        public async Task<User?> GetUserById(int id)
        {
            return await _store_215962135Context.Users.FindAsync(id);
        }

        public async Task<User> AddUser(User user)
        {
            await _store_215962135Context.Users.AddAsync(user);
            await _store_215962135Context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> LogIn(string userName, string password)
        {
            return await _store_215962135Context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
        }

        public async Task UpdateUser(int id, User updateUser)
        {
            var existingUser = await _store_215962135Context.Users.FindAsync(id);

            if (existingUser == null)
                throw new Exception("User not found");

            existingUser.FirstName = updateUser.FirstName;
            existingUser.LastName = updateUser.LastName;
            existingUser.UserName = updateUser.UserName;
            existingUser.Password = updateUser.Password;
            existingUser.Phone = updateUser.Phone;
            existingUser.Address = updateUser.Address;

            await _store_215962135Context.SaveChangesAsync();
        }




    }
}
