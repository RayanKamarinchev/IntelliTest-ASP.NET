using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Data.Entities;

namespace IntelliTest.Core.Contracts
{
    public interface ILessonService
    {
        Task<QueryModel<LessonViewModel>> GetAll(Guid? teacherId, QueryModel<LessonViewModel> query, string userId);
        Task<LessonViewModel?>? GetById(Guid lessonId);
        Task<LessonViewModel?>? GetByName(string name);
        Task<bool> ExistsById(Guid teacherId, Guid lessonId);
        Task Edit(Guid lessonId, EditLessonViewModel model);
        EditLessonViewModel ToEdit(LessonViewModel model);
        Task Create(EditLessonViewModel model, Guid teacherId);
        Task LikeLesson(Guid lessonId, string userId);
        Task UnlikeLesson(Guid lessonId, string userId);
        Task<bool> IsLiked(Guid lessonId, string userId);
        Task Read(Guid lessonId, string userId);
        Task<IEnumerable<LessonViewModel>> ReadLessons(string userId);
        Task<IEnumerable<LessonViewModel>> LikedLessons(string userId);

        public Task<QueryModel<LessonViewModel>> Filter(IQueryable<Lesson> lessonQuery,
                                                        QueryModel<LessonViewModel> query, string userId);

        public Task<bool> IsLessonCreator(Guid lessonId, Guid teacherId);

        public Task<QueryModel<LessonViewModel>> GetAllAdmin(QueryModel<LessonViewModel> query);
    }
}
