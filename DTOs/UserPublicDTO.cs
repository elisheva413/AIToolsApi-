using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
   public record UserPublicDTO
   (
        string FirstName,
        string LastName,
        string UserName,
        int UserId,
        string Phone,
        string Address,
        string Role      
   );
    
}
