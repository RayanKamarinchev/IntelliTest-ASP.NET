using IntelliTest.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Contracts
{
    public interface IStudentService
    {
        Task<bool> ExistsByUserId(string id);
        Task AddStudent(UserType model, string userId);
    }
}
