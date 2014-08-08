using System;
using System.Net.Mail;
using Carnotaurus.GhostPubsMvc.Common.Result;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Managers.Implementation
{
    public class MailManager : IMailManager
    {
        private readonly IMailSender _sender;

        public MailManager(IMailSender sender)
        {
            _sender = sender;
        }

        public CommandResult Send(MailMessage message, string attachmentPath)
        {
            if (message == null) return new CommandResult(String.Empty, "There is no message to send.");

            var result = new CommandResult();

            _sender.Send(message, attachmentPath);

            return result;
        }
    }
}