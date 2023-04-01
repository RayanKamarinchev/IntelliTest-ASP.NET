using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Contracts
{
    public interface IClassService
    {
        public Task<bool> ContatinsTest();
    }
}
