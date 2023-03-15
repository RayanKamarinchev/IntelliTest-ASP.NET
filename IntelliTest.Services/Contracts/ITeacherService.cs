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
        Task<bool> ExistsByUserId(string id);
        Task AddTeacher(string userId);
        Task<bool> IsTestCreator(int testId, int teacherId);
        Task<bool> IsLessonCreator(int lessonId, int teacherId);
        Task<int> GetTeacherId(string userId);
    }
}
