using Microsoft.AspNetCore.Identity;

namespace GymChanger.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string BirthDay { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
