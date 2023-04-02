using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Mails
{
    public class EmailMessage
    {
        // Receiver
        public string To { get; }

        // Content
        public string Subject { get; }

        public string? Body { get; }

        public EmailMessage(string to, string subject, string? body = null)
        {
            // Receiver
            To = to;

            // Content
            Subject = subject;
            Body = body;
        }
    }
}
