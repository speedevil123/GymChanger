using System.ComponentModel.DataAnnotations;

namespace GymChanger.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set ;}
        public string Title { get; set; }
        public string DayStart { get; set; }
        public string MonthStart { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string TrainerId { get; set; }
        public List<AppUser> AppUsers { get; set; } = new List<AppUser>();
    }
}
