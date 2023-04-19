using IntelliTest.Core.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Contracts
{
    public interface IClassService
    {
        Task<IEnumerable<ClassViewModel>> GetAll();
        Task Create(ClassViewModel model);
    }
}
