using IntelliTest.Core.Models.Mails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Contracts
{
    public interface IEmailService
    {
        Task<bool> SendAsync(EmailMessage emailMessage, CancellationToken token);
    }
}
