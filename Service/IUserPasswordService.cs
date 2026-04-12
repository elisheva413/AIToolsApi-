using Entities;
using Repositeries;

namespace Service

{
    //public interface IUserPasswordService
    //{
    //    //int CheckPassword(string password);
    //}

    public interface IUserPasswordService
    {
        UserPassword Level(string pass);
        bool UpdatePassword(int userId, string newPassword);
    }
}