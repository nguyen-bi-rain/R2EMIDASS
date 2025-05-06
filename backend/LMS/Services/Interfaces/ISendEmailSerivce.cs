namespace LMS.Services.Interfaces
{
    public interface ISendEmailSerivce
    {
        Task SendEmailAsync(string to,string subject, string body);
        Task SendEmalWithBodyAsync(string to,string subject,string templatePath,object model);
    }
}