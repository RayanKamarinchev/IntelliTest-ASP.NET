﻿using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data.Entities;

namespace IntelliTest.Core.Contracts
{
    public interface ITestService
    {
        Task<QueryModel<TestViewModel>> GetAll(Guid? teacherId, Guid? studentId, QueryModel<TestViewModel> query);
        Task<QueryModel<TestViewModel>> GetMy(Guid? teacherId, Guid? studentId, QueryModel<TestViewModel> query);
        Task<TestViewModel> GetById(Guid id);
        Task<bool> ExistsbyId(Guid id);
        Task Edit(Guid id, TestEditViewModel model, Guid teacherId);
        TestSubmitViewModel ToSubmit(TestViewModel model);
        Task<bool> IsTestTakenByStudentId(Guid testId, Guid studentId);
        Task<QueryModel<TestViewModel>> TestsTakenByStudent(Guid studentId, QueryModel<TestViewModel> query);
        Task<Guid> Create(TestViewModel model, Guid teacherId, string[] classNames);
        public Task<bool> StudentHasAccess(Guid testId, Guid studentId);
        public Task DeleteTest(Guid id);
        Task SaveChanges();
        public Task<QueryModel<TestViewModel>> Filter(IQueryable<Test> testQuery, QueryModel<TestViewModel> query, Guid? teacherId, Guid? studentId);
        public Task<QueryModel<TestViewModel>> FilterMine(IEnumerable<Test> testQuery, QueryModel<TestViewModel> query);
        public Task<bool> IsTestCreator(Guid testId, Guid teacherId);
    }
}
