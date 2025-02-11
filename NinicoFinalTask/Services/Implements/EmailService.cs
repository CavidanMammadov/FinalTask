using Microsoft.Extensions.Options;
using NinicoFinalTask.Helpers;
using NinicoFinalTask.Services.Abstracts;

namespace NinicoFinalTask.Services.Implements
{
    public class EmailService(SmtpOptions opt) : IEmailService
    {
        readonly SmtpOptions _smtpOptions;
        public EmailService(IOptions<SmtpOptions> options)
        {
            _smtpOptions = options.Value; 
        }
        public Task SendAsync()
        {
            throw new NotImplementedException();
        }
    }
}
