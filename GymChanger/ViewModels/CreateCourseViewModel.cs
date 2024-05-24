using GymChanger.Models;

namespace GymChanger.ViewModels
{
    public class CreateCourseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DayStart { get; set; }
        public string MonthStart { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string TrainerId { get; set; }
    }
}
