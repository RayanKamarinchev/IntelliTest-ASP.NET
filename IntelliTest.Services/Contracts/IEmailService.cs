using IntelliTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Contracts
{
    public interface IEmailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}
