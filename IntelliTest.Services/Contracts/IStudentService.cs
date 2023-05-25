﻿using IntelliTest.Core.Models.Users;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Data.Entities;
using IntelliTest.Core.Models.Tests;

namespace IntelliTest.Core.Contracts
{
    public interface IStudentService
    {
        Task AddStudent(UserType model, string userId);
        Guid? GetStudentId(string userId);

        Task<Student> GetStudent(Guid studentId);
        Task<IEnumerable<TestResultsViewModel>> GetAllResults(Guid studentId);
    }
}
