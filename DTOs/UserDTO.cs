//using System.ComponentModel.DataAnnotations;
//using Entities;

//namespace DTOs
//{
 
//    public record UserDTO
//    {
//        public int UserID { get; init; }

//        [Required(ErrorMessage = "שדה שם פרטי הוא חובה")]
//        public string FirstName { get; init; } = string.Empty;
//        public string LastName { get; init; } = string.Empty;

//        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
//        [EmailAddress(ErrorMessage = "אימייל לא תקין - חסר @ או דומיין")]
//        public string UserName { get; init; } = string.Empty;

//        public string? Password { get; init; }

//        public string? Phone { get; init; }
//        public string? Address { get; init; }

//       public UserDTO() { }

//        public UserDTO(int id, string firstName, string lastName, string email, string? password, string? phone, string? address)
//        {
//            UserID = id;
//            FirstName = firstName;
//            LastName = lastName;
//            UserName = email;
//            Password = password;
//            Phone = phone;
//            Address = address;
//        }
//    }
//}

