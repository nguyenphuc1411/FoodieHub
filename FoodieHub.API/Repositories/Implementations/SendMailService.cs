using FoodieHub.API.Repositories.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using System.Threading.Tasks;
using FoodieHub.API.Models.DTOs.Authentication;

namespace FoodieHub.API.Repositories.Implementations
{
    public class SendMailService : IMailService
    {
        private readonly IConfiguration _config;

        public SendMailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_config["Smtp:FromName"], _config["Smtp:User"]));
                message.To.Add(new MailboxAddress("", mailRequest.ToEmail));
                message.Subject = mailRequest.Subject;

                // Tạo BodyBuilder để xây dựng nội dung email
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = mailRequest.Body // Nội dung HTML của email
                };

                // Đính kèm các tệp từ chuỗi byte
                if (mailRequest.Attachments != null && mailRequest.Attachments.Count > 0)
                {
                    foreach (var attachment in mailRequest.Attachments)
                    {
                        // Sử dụng AddAttachment thay vì tạo MimePart
                        bodyBuilder.Attachments.Add(attachment.FileName, attachment.FileData);
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();

                // Kết nối với SMTP Server và gửi email
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_config["Smtp:Server"], int.Parse(_config["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_config["Smtp:User"], _config["Smtp:Pass"]);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                return false;
            }
        }
    }
}
