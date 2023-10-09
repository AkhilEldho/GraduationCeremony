using Microsoft.AspNetCore.Identity;

namespace GraduationCeremony.Models.DB
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email {  get; set; }
        public string Password { get; set; }

    }
}
