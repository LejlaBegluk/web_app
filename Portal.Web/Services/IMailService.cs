using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Web.Services
{
    public interface IMailService
    {
        Task<Response> SendEmailAsync(string toEmail, string subject, string content);
    }
}
