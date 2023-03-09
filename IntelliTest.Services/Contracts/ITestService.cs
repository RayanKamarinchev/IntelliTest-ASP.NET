using IntelliTest.Core.Models.Tests;
using IntelliTest.Models.Tests;

namespace IntelliTest.Core.Contracts
{
    public interface ITestService
    {
        Task<IEnumerable<TestViewModel>> GetAll();
        Task<IEnumerable<TestViewModel>> GetMy(string userId);
        Task<TestViewModel> GetById(int id);
        Task<bool> ExistsbyId(int id);
        TestEditViewModel ToEdit(TestViewModel model);
        Task Edit(int id, TestEditViewModel model);
        TestSubmitViewModel ToSubmit(TestViewModel model);
    }
}
