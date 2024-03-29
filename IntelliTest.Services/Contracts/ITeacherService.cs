﻿using System;
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
        Task AddTeacher(string userId, string school);
        Guid? GetTeacherId(string userId);
    }
}
