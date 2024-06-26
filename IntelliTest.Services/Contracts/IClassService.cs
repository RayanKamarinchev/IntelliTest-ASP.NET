﻿using IntelliTest.Core.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Users;

namespace IntelliTest.Core.Contracts
{
    public interface IClassService
    {
        Task<IEnumerable<ClassViewModel>> GetAll(string userId, bool isStudent, bool isTeacher);
        Task<ClassViewModel?> GetById(Guid id);
        Task<bool> IsClassOwner(Guid id, string userId);
        Task Create(ClassViewModel model);
        Task Edit(ClassViewModel model, Guid id);
        Task Delete(Guid id);
        Task<bool?> IsInClass(Guid classId, string userId, bool isStudent, bool isTeacher);
        Task<bool> RemoveStudent(Guid studentId, Guid id);
        Task<bool> AddStudent(Guid studentId, Guid id);
        public Task<List<StudentViewModel>> GetClassStudents(Guid id);

        Task<IEnumerable<ClassViewModel>> GetAllAdmin();
    }
}
