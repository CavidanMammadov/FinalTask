using Microsoft.Extensions.Options;
using NinicoFinalTask.Helpers;
using NinicoFinalTask.Services.Abstracts;
using System.Net;
using System.Net.Mail;

namespace NinicoFinalTask.Services.Implements
{
    public class EmailService : IEmailService
    {
        readonly SmtpClient _client;
        readonly MailAddress _from;
        readonly HttpContext Context;
        public EmailService(IOptions<SmtpOptions> option, IHttpContextAccessor acc)
        {
            var opt = option.Value;
            _client = new(opt.Host, opt.Port);
            _client.Credentials = new NetworkCredential(opt.Sender,opt.Password);
            _client.EnableSsl = true;
            _from = new MailAddress(opt.Sender, "NinicoFurniture");
            Context = acc.HttpContext;
        }

        public void SendEmailConfirmationAsync(string reciever, string name, string token )
        {
            MailAddress to = new(reciever);
            MailMessage  message = new MailMessage(_from, to);
            message.Subject = "Confirm your email";
            string url = Context.Request.Scheme + "://" + Context.Request.Host + "/Account/VerifyEmail?token="+token+"&user="+name ;
            message.Body = EmailTemplates.VerifEmail.Replace("__$name",name ).Replace("__$link",url);
            message.IsBodyHtml = true;
            _client.Send(message);

        }
    }
}
