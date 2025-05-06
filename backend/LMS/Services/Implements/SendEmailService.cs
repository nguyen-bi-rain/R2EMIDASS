
using MailKit.Net.Smtp;
using LMS.Services.Interfaces;
using MailKit.Security;
using MimeKit;

namespace LMS.Services.Implements
{
    public class SendEmailService : ISendEmailSerivce
    {
        private readonly string _emailHost;
        private readonly int _emailPort;
        private readonly string _emailUsername;
        private readonly string _emailPassword;

        public SendEmailService()
        {

            _emailHost = Environment.GetEnvironmentVariable("EMAILSETTINGS__HOST");
            _emailPort = int.Parse(Environment.GetEnvironmentVariable("EMAILSETTINGS__PORT"));
            _emailUsername = Environment.GetEnvironmentVariable("EMAILSETTINGS__USERNAME");
            _emailPassword = Environment.GetEnvironmentVariable("EMAILSETTINGS__PASSWORD");
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Library Management System", _emailUsername));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            email.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_emailHost, _emailPort, SecureSocketOptions.StartTls);
                client.Authenticate(_emailUsername, _emailPassword);
                await client.SendAsync(email);
                client.Disconnect(true);
            }
        }

        public async Task SendEmalWithBodyAsync(string to, string subject, string templatePath, object model)
        {
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemplate.html");
            var template = await File.ReadAllTextAsync(emailTemplatePath);
            var userModel = model as dynamic;
            var body = template.Replace("{{UserName}}", userModel.UserName.ToString())
                                .Replace("{{Id}}", userModel.Id.ToString())
                                .Replace("{{DateRequest}}", userModel.DateRequest.ToString())
                                .Replace("{{ApprovalStatus}}", userModel.Status.ToString())
                                .Replace("{{StatusColor}}", userModel.StatusColor.ToString())
                                .Replace("{{CustomMessage}}", userModel.Message.ToString());
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Library Management System", _emailUsername));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;



            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            email.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_emailHost, _emailPort, SecureSocketOptions.StartTls);
                client.Authenticate(_emailUsername, _emailPassword);
                await client.SendAsync(email);
                client.Disconnect(true);
            }
        }
    }
}