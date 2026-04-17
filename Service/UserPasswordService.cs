//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Entities;
//using Repositeries;
//using Zxcvbn;

//namespace Service
//{
//    public class UserPasswordService : IUserPasswordService
//    {
//        private readonly IUserPasswordRipository _userPasswordRipo;

//        public UserPasswordService(IUserPasswordRipository userPassword)
//        {
//            _userPasswordRipo = userPassword;
//        }

//        public int CheckPassword(string password)
//        {
//            return Zxcvbn.Core.EvaluatePassword(password).Score;
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Repositeries;
using Zxcvbn;

namespace Service
{
    public class UserPasswordService : IUserPasswordService
    {
        private readonly IUserPasswordRipository _userPasswordRipo;

        public UserPasswordService(IUserPasswordRipository userPassword)
        {
            _userPasswordRipo = userPassword;
        }

        public UserPassword Level(string pass)
        {
            var result = Zxcvbn.Core.EvaluatePassword(pass);
            int strength = result.Score;

            UserPassword pass1 = new UserPassword();
            pass1.Password = pass;
            pass1.Strength = strength;

            return pass1;
        }

        private const int MIN_REQUIRED_STRENGTH = 3;

        public bool UpdatePassword(int userId, string newPassword)
        {
            var strengthResult = Level(newPassword);

            if (strengthResult.Strength < MIN_REQUIRED_STRENGTH)
            {
                return false;
            }
            return true;
        }
    }
}