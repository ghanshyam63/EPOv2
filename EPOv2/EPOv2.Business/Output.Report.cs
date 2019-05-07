namespace EPOv2.Business
{
    using System.Net.Mail;

    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Business.Properties;

    public partial class Output : IOutput
    {
        public void SendReport()
        {
            
        }

        public void SendCapexNotification(User user, string body, string subject)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(Settings.Default.EmailFrom);
            mail.To.Add(new MailAddress(user.Email));
            mail.Bcc.Add(new MailAddress(Settings.Default.DevEmail));
            var client = new SmtpClient();
            mail.Subject = subject;
            mail.Priority=MailPriority.High;
            mail.IsBodyHtml = true;
            mail.Body = body;
            client.Send(mail);
            mail.Dispose();
        }

       
    }
}