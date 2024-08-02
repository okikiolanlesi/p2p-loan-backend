using System.Threading.Tasks;

namespace P2PLoan.Interfaces;

public interface IEmailService
{
    Task SendHtmlEmailAsync(string toEmail, string subject, string templateFileName, object model);
}