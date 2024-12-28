using System.Net.Mail;
using System.Net;
using System.Diagnostics;


namespace BussinessLogicLayer_BLL.Services
{
	public class EmailService
	{
		
			private const string SmtpServer = "smtp.gmail.com";
			private const int Port = 587; 
			private const string SenderEmail = "duong682@gmail.com"; 
			private const string SenderPassword = "vruz hueh ztlz tnrv"; 
			private const bool EnableSsl = true;

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			try
			{
				using (var smtpClient = new SmtpClient(SmtpServer)
				{
					Port = Port,
					Credentials = new NetworkCredential(SenderEmail, SenderPassword),
					EnableSsl = EnableSsl,
				})
				using (var mailMessage = new MailMessage
				{
					From = new MailAddress(SenderEmail),
					Subject = subject,
					Body = body,
					IsBodyHtml = true,
				})
				{
					mailMessage.To.Add(toEmail);
					await smtpClient.SendMailAsync(mailMessage);
				}
			}
			catch (SmtpException ex)
			{
				Debug.WriteLine($"SMTP Error: {ex.Message}");
				if (ex.InnerException != null)
				{
					Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
				}
				throw;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"General Error: {ex.Message}");
				throw;
			}
		}
	}

	
}
