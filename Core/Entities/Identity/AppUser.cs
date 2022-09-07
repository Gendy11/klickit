using Microsoft.AspNetCore.Identity;


namespace Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string displayName { get;set;}
        public Address Address { get;set;}

    }
}
