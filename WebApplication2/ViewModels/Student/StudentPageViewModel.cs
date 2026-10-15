using WebApplication2.Models;

namespace WebApplication2.ViewModels.Student
{
    public class StudentPageViewModel
    {
        public Students Student { get; set; } = new();

        public List<Students> Students { get; set; } = new();
        public List<TblClass> Classes { get; set; } = new();
    }
}
