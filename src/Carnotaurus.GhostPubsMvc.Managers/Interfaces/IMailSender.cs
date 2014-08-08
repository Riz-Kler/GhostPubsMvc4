using System.Net.Mail;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface IMailSender
    {
        void Send(MailMessage message, string attachmentpath);
    }
}