using IntelliTest.Models.Tests;

namespace IntelliTest.Contracts
{
    public interface ITestService
    {
        public Task<IEnumerable<TestViewModel>> GetAll();
        public Task<IEnumerable<TestViewModel>> GetMy(string userId);
    }
}
