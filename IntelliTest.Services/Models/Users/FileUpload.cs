using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IntelliTest.Core.Models.Users
{
    public class FileUpload
    {
        public IFormFile file { get; set; }
    }
}
