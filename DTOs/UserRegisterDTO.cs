using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record UserRegisterDTO(
        [Required(ErrorMessage = "אימייל (שם משתמש) הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "אימייל לא תקין")]
        string UserName,

         [Required(ErrorMessage = "סיסמה היא שדה חובה")]
        string Password,

         [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
        string FirstName,

         [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
        string LastName,

        string? Phone,
        string? Address
     );

}

