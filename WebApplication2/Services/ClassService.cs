using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApplication2.Data;
using WebApplication2.Interfaces;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class ClassService : IClassService
    {
        private readonly ApplicationDbContext _context;

        public ClassService(ApplicationDbContext context)
        {
            _context = context;
        }


        async Task<DbResponse> IClassService.GetClasses()
        {
            var allClasses = await _context.Classes.AsNoTracking().ToListAsync();

            if (!allClasses.Any())
                return new DbResponse { Data = null, Success = false, Message = "Classes Not Found !!" };

            return new DbResponse
            {
                Data = allClasses,
                Success = true,
                Message = "Classes Retrive Successfully !!"
            };
        }


        async Task<DbResponse> IClassService.GetClassById(int Id)
        {
            try
            {

                if (Id <= 0)
                    return new DbResponse { Data = null, Success = false, Message = "Class Details Not Found !!" };

                var ClassInfo = await _context.Classes.FirstOrDefaultAsync(x => x.Id == Id);

                if (ClassInfo == null)
                    return new DbResponse { Data = null, Success = false, Message = "Class Details Not Found !!" };

                return new DbResponse
                {
                    Data = ClassInfo,
                    Success = true,
                    Message = "Class Details Founded !!"
                };
            }
            catch (Exception ex)
            {
                var method = MethodBase.GetCurrentMethod();
                return new DbResponse
                {
                    Success = false,
                    Message = $"{ex.Message} (at {method?.DeclaringType?.FullName}.{method?.Name})"
                };
            }

        }

        async Task<DbResponse> IClassService.InsertOrUpdateClass(TblClass cls)
        {

            try
            {
                if (cls == null)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Class details not found!",
                        Data = null
                    };
                }

                if (cls.Id > 0)
                {
                    var existingClass = await _context.Classes.FindAsync(cls.Id);

                    if (existingClass == null)
                    {
                        return new DbResponse
                        {
                            Success = false,
                            Message = "Class not found!",
                            Data = null
                        };
                    }
                    existingClass.Name = cls.Name;
                }
                else
                {
                    var clsName = cls.Name.Trim().ToLower();

                    var isDuplicate = await _context.Classes.AnyAsync(x => x.Name.Trim().ToLower() == clsName);

                    if (isDuplicate)
                    {
                        return new DbResponse
                        {
                            Success = false,
                            Message = "Class already exists!"
                        };
                    }

                    await _context.Classes.AddAsync(cls);
                }

                await _context.SaveChangesAsync();

                return new DbResponse
                {
                    Success = true,
                    Message = "Class Save Successfully !!",
                    Data = cls
                };

            }
            catch (Exception ex)
            {
                var method = MethodBase.GetCurrentMethod();
                return new DbResponse
                {
                    Success = false,
                    Message = $"{ex.Message} (at {method?.DeclaringType?.FullName}.{method?.Name})"
                };
            }

        }

        async Task<DbResponse> IClassService.DeleteClass(int Id)
        {
            try
            {
                var _class = await _context.Classes.FirstOrDefaultAsync(x => x.Id == Id);

                if (_class == null)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Class not found!"
                    };
                }

                _context.Classes.Remove(_class);
                await _context.SaveChangesAsync();

                return new DbResponse
                {
                    Success = true,
                    Message = "Class deleted successfully.",
                    Data = _class
                };
            }
            catch (Exception ex)
            {
                var method = MethodBase.GetCurrentMethod();
                return new DbResponse
                {
                    Success = false,
                    Message = $"{ex.Message} (at {method?.DeclaringType?.FullName}.{method?.Name})"
                };
            }
        }

    }
}
