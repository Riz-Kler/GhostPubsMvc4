using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Common.Helpers;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Managers.Implementation
{
    public class MailSender : IMailSender
    {
        public void Send(MailMessage message, string attachmentPath)
        {
            var connectionString = ConfigurationHelper.GetConnectionStringValue("DatabaseMail");

            var profile = Resources.EmailDatabaseMailProfile;

            var body = message.Body.DoubleApostrophes();

            var commandText = string.Format(@"USE msdb
                    EXEC sp_send_dbmail
                    @profile_name= '{0}' ,
                    @recipients= '{1}' ,
                    @subject= '{3}' ,
                    @body_format = 'HTML',
                    @body= '{2}' ,
                    @file_attachments= '{4}'",
                profile,
                message.To.First().Address,
                body,
                message.Subject,
                attachmentPath
                );

            var useDatabaseMail = ConfigurationHelper.GetValueAsBoolean("UseDatabaseMail");

            if (useDatabaseMail)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand(commandText, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }

            var useSmtpClientMail = ConfigurationHelper.GetValueAsBoolean("UseSmtpClientMail");

            if (!useSmtpClientMail) return;

            // send using mail client
            var client = new SmtpClient();

            // Attempt to send the email
            client.Send(message);
        }
    }
}