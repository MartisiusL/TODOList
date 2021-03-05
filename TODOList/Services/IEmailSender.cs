using System.Threading.Tasks;
using TODOList.Models;

namespace TODOList.Services
    {
    public interface IEmailSender
        {
        Task SendEmailAsync (Message message);
        }
    }
