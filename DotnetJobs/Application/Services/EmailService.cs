using DotnetJobs.Application.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace DotnetJobs.Application.Services
{
    public class EmailService
    {
        private string _user;
        private string _password;
        private string _adminEmail;

        public EmailService()
        {
            _user = Environment.GetEnvironmentVariable("EMAIL_USER");
            _password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
            _adminEmail = Environment.GetEnvironmentVariable("EMAIL_ADMIN");
        }

        public void SendAdminAlert(Job job)
        {
            string body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width"" initial-scale=""1"">
                    <title>New Job</title>
                </head>
                <body>
                    <p style=""color:red"">New job created</p>
                    <p>Company ID: {job.Company.Id}</p>
                    <p>Company: {job.Company.Name}</p>
                    <p>Job Id: {job.Id}</p
                    <p>Job Title: {job.Title}</p
                    <p>Job Link: {job.ApplyLink}</p
                </body>
                </html>
            ";
            SendAlert(_adminEmail, $"Dotnet Jobs - New Job Posted by {job.Company.Name}", body, _adminEmail);
        }

        private void SendAlert(string to, string subject, string body, string from)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Wes from DotnetJobs", from));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_user, _password);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
