using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Helpers;
using WebApplication2.Interfaces;
using WebApplication2.Models;
using WebApplication2.ViewModels.Account;

namespace WebApplication2.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse> RegisterAsync(SignupViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Registration details are required."
                    };
                }

                var normalizedEmail = model.Email.Trim().ToLower();

                // Check if email already registered via EF Core
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

                if (emailExists)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "An account with this email already exists."
                    };
                }

                // Create user with hashed password
                var user = new User
                {
                    FullName = model.FullName.Trim(),
                    Email = normalizedEmail,
                    PasswordHash = PasswordHelper.HashPassword(model.Password),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return new DbResponse
                {
                    Success = true,
                    Message = "Registration completed successfully! You can now log in.",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                var method = MethodBase.GetCurrentMethod();
                return new DbResponse
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message} (at {method?.DeclaringType?.FullName}.{method?.Name})"
                };
            }
        }

        public async Task<DbResponse> LoginAsync(LoginViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Login details are required."
                    };
                }

                var normalizedEmail = model.Email.Trim().ToLower();

                // Retrieve user via EF Core
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                // Verify password hash
                var isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);
                if (!isPasswordValid)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                return new DbResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                var method = MethodBase.GetCurrentMethod();
                return new DbResponse
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message} (at {method?.DeclaringType?.FullName}.{method?.Name})"
                };
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        }
    }
}
