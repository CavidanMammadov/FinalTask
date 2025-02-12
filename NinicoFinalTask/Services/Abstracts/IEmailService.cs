namespace NinicoFinalTask.Services.Abstracts
{
    public interface IEmailService
    {
        void SendEmailConfirmationAsync(string reciever,string  name , string token);
    }
}
