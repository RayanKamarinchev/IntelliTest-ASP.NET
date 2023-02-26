using IntelliTest.Models.Tests;

namespace IntelliTest.Core.Contracts
{
    public interface ITestService
    {
        Task<IEnumerable<TestViewModel>> GetAll();
        Task<IEnumerable<TestViewModel>> GetMy(string userId);
        Task<TestViewModel> GetById(int id);
    }
}
