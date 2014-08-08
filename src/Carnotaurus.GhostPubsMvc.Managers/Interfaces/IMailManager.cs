using System.Net.Mail;
using Carnotaurus.GhostPubsMvc.Common.Result;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface IMailManager
    {
        CommandResult Send(MailMessage message, string attachmentPath);
    }
}