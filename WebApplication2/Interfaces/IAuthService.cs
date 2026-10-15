using WebApplication2.Models;
using WebApplication2.ViewModels.Account;

namespace WebApplication2.Interfaces
{
    public interface IAuthService
    {
        Task<DbResponse> RegisterAsync(SignupViewModel model);
        Task<DbResponse> LoginAsync(LoginViewModel model);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
