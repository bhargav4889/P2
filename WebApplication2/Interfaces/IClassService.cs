using WebApplication2.Models;

namespace WebApplication2.Interfaces
{
    public interface IClassService
    {
        Task<DbResponse> GetClasses();

        Task<DbResponse> GetClassById(int Id);

        Task<DbResponse> InsertOrUpdateClass(TblClass cls);

        Task<DbResponse> DeleteClass(int Id);



    }
}
