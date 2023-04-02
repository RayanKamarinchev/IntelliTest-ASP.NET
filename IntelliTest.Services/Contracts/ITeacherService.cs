using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data.Entities;

namespace IntelliTest.Core.Contracts
{
    public interface ITeacherService
    {
        Task AddTeacher(string userId);
        Task<bool> IsTestCreator(Guid testId, Guid teacherId);
        Task<bool> IsLessonCreator(Guid lessonId, Guid teacherId);
        Task<Guid> GetTeacherId(string userId);
    }
}
