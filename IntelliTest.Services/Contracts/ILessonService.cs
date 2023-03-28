using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Lessons;

namespace IntelliTest.Core.Contracts
{
    public interface ILessonService
    {
        Task<IEnumerable<LessonViewModel>> GetAll();
        Task<LessonViewModel> GetById(int lessonId);
        Task<bool> ExistsById(int lessonId);
        Task Edit(int lessonId, LessonViewModel model);
        EditLessonViewModel ToEdit(LessonViewModel model);
        Task Create(EditLessonViewModel model, int teacherId);
        Task LikeLesson(int lessonId, string userId);
        Task UnlikeLesson(int lessonId, string userId);
        Task<bool> IsLiked(int lessonId, string userId);
    }
}
