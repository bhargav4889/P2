using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApplication2.Data;
using WebApplication2.Interfaces;
using WebApplication2.Models; // add this

namespace WebApplication2.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse> GetStudents()
        {
            try
            {
                //var students = await _context.Students.AsNoTracking().ToListAsync();

                var students = await (from s in _context.Students.AsNoTracking()
                                   join classes in _context.Classes.AsNoTracking()
                                   on s.ClassId equals classes.Id into classGroup
                                   from c in classGroup.DefaultIfEmpty()
                                   select new Students
                                   {
                                       Id = s.Id,
                                       ClassId = c.Id,
                                       ClassName = c.Name,
                                       FirstName = s.FirstName,
                                       LastName = s.LastName,
                                       Email = s.Email,
                                       Age = s.Age
                                    }).ToListAsync();


                if (!students.Any())
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "No students found."
                    };
                }

                return new DbResponse
                {
                    Success = true,
                    Message = "Students retrieved successfully.",
                    Data = students
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

        public async Task<DbResponse> InsertOrUpdateStudent(Students student)
        {
            try
            {
                if (student == null)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Student details not found!",
                        Data = null
                    };
                }

                if (student.Id > 0)
                {
                    var existingStudent = await _context.Students.FindAsync(student.Id);

                    if (existingStudent == null)
                    {
                        return new DbResponse
                        {
                            Success = false,
                            Message = "Student not found!",
                            Data = null
                        };
                    }

                    existingStudent.FirstName = student.FirstName;
                    existingStudent.LastName = student.LastName;
                    existingStudent.Age = student.Age;
                    existingStudent.Email = student.Email;
                    existingStudent.ClassId = student.ClassId;
                }
                else
                {
                    var email = student.Email.Trim().ToLower();

                    var isDuplicateEmail = await _context.Students.AnyAsync(x =>
                        x.Email.Trim().ToLower() == email);

                    if (isDuplicateEmail)
                    {
                        return new DbResponse
                        {
                            Success = false,
                            Message = "Email already exists!"
                        };
                    }

                    await _context.Students.AddAsync(student);
                }

                await _context.SaveChangesAsync();

                return new DbResponse
                {
                    Success = true,
                    Message = "Student Save Successfully !!",
                    Data = student
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

        public async Task<DbResponse> GetStudentById(int id)
        {
            try
            {
                var student = await _context.Students
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (student == null)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Student not found!"
                    };
                }

                return new DbResponse
                {
                    Success = true,
                    Message = "Student retrieved successfully.",
                    Data = student
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

        public async Task<DbResponse> DeleteStudent(int id)
        {
            try
            {

                var student = await _context.Students.FindAsync(id);

                if (student == null)
                {
                    return new DbResponse
                    {
                        Success = false,
                        Message = "Student not found!"
                    };
                }

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                return new DbResponse
                {
                    Success = true,
                    Message = "Student deleted successfully.",
                    Data = student
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
