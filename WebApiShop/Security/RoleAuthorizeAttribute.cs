using Microsoft.AspNetCore.Authorization;

namespace WebApiShop.Security
{
    public sealed class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public RoleAuthorizeAttribute(string role)
        {
            Roles = role;
        }
    }
}
