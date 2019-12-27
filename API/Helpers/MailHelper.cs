using Microsoft.Extensions.Configuration;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API.Helpers
{
    public interface IMailHelper
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailRangeAsync(List<string> emails, string subject, string message);
    }
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorMessageService _errorMessageService;

        public MailHelper(IConfiguration configuration, IErrorMessageService errorMessageService)
        {
            _configuration = configuration;
            _errorMessageService = errorMessageService;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient(_configuration["MailSettings:Server"])
            {

                UseDefaultCredentials = bool.Parse(_configuration["MailSettings:UseDefaultCredentials"]),
                Port = int.Parse(_configuration["MailSettings:Port"]),
                EnableSsl = bool.Parse(_configuration["MailSettings:EnableSsl"]),
                Credentials = new NetworkCredential(_configuration["MailSettings:UserName"], _configuration["MailSettings:Password"])
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["MailSettings:FromEmail"], _configuration["MailSettings:FromName"]),
            };
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.High;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            try
            {
                client.Send(mailMessage);

            }
            catch (Exception ex)
            {

                _errorMessageService.Add(new Models.EF.ErrorMessage { Function = subject, Name = ex.Message });

            }
            return Task.CompletedTask;
        }

        public Task SendEmailRangeAsync(List<string> emails, string subject, string message)
        {
            SmtpClient client = new SmtpClient(_configuration["MailSettings:Server"])
            {
                UseDefaultCredentials = bool.Parse(_configuration["MailSettings:UseDefaultCredentials"]),
                Port = int.Parse(_configuration["MailSettings:Port"]),
                EnableSsl = bool.Parse(_configuration["MailSettings:EnableSsl"]),
                Credentials = new NetworkCredential(_configuration["MailSettings:UserName"], _configuration["MailSettings:Password"])
            };
            emails.Add(_configuration["MailSettings:TestEmail"].ToString());
            using MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(_configuration["MailSettings:FromEmail"], _configuration["MailSettings:FromName"]),
                IsBodyHtml = true,
                Body = message,
                Subject = subject,
                Priority = MailPriority.High,
                BodyEncoding = System.Text.Encoding.UTF8
            };
            foreach (var email in emails)
            {
                mailMessage.To.Add(email);
            }

            try
            {
                client.Send(mailMessage);
                _errorMessageService.Add(new Models.EF.ErrorMessage { Function = subject, Name = subject + ": successfully!" });

            }
            catch (Exception ex)
            {
                _errorMessageService.Add(new Models.EF.ErrorMessage { Function = subject, Name = ex.Message });

            }
            return Task.CompletedTask;
        }
    }
}
