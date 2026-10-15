using WebApplication2.Models;

namespace WebApplication2.Interfaces
{
    public interface IStudentService
    {
        Task<DbResponse> GetStudents();

        Task<DbResponse> InsertOrUpdateStudent(Students student);
        Task<DbResponse> GetStudentById(int id);
        Task<DbResponse> DeleteStudent(int id);

    }
}
