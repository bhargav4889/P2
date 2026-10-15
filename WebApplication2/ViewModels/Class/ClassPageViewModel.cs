using WebApplication2.Models;

namespace WebApplication2.ViewModels.Class
{
    public class ClassPageViewModel
    {
        public Models.TblClass Class { get; set; } = new();

        public List<Models.TblClass> Classes { get; set; } = new();
    }
}
