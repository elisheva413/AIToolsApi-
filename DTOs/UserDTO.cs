using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace DTOs
{
    public record UserDTO
    (
        int UserId,
        string FirstName,
        string LastName,
        string UserName,
        string Password
    );
}
