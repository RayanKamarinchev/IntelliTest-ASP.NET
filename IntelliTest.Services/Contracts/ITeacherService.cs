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
        Task<bool> ExistsByUserId(string id);
        Task AddTeacher(string userId);
    }
}