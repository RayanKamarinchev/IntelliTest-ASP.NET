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
        Task<LessonViewModel?>? GetById(Guid lessonId);
        Task<LessonViewModel?>? GetByName(string name);
        Task<bool> ExistsById(Guid lessonId);
        Task Edit(Guid lessonId, EditLessonViewModel model);
        EditLessonViewModel ToEdit(LessonViewModel model);
        Task Create(EditLessonViewModel model, Guid teacherId);
        Task LikeLesson(Guid lessonId, string userId);
        Task UnlikeLesson(Guid lessonId, string userId);
        Task<bool> IsLiked(Guid lessonId, string userId);
        Task Read(Guid lessonId, string userId);
        Task<IEnumerable<LessonViewModel>> ReadLessons(string userId);
        Task<IEnumerable<LessonViewModel>> LikedLessons(string userId);
    }
}
