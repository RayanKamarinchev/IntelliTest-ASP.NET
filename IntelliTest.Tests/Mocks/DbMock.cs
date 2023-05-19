using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Tests.Mocks
{
    public static class DbMock
    {
        public static IntelliTestDbContext Instance
        {
            get
            {
                var dbContextOptions = new DbContextOptionsBuilder<IntelliTestDbContext>()
                    .UseInMemoryDatabase("IntelliTestInMemoryDb" + DateTime.Now.Ticks.ToString())
                    .Options;
                return new IntelliTestDbContext(dbContextOptions);
            }
        }
    }
}
