using System.Collections.Generic;
using System.Text;

namespace Carnotaurus.GhostPubsMvc.Common.Result
{
    public class CommandResult
    {
        public CommandResult()
        {
            Errors = new List<ErrorMessage>();
            Warnings = new List<string>();
        }

        public CommandResult(string propertyName, string reason, params object[] args)
            : this()
        {
            AddFormat(propertyName, reason, args);
        }


        public bool Success
        {
            get { return Errors.Count == 0; }
        }

        public List<ErrorMessage> Errors { get; set; }
        public List<string> Warnings { get; set; }

        public CommandResult Warn(string warning)
        {
            Warnings.Add(warning);
            return this;
        }

        public CommandResult Add(CommandResult domainResult)
        {
            Warnings.AddRange(domainResult.Warnings);
            Errors.AddRange(domainResult.Errors);
            return this;
        }

        public CommandResult AddFormat(string propertyName, string reasonFormat, params object[] args)
        {
            return Add(string.Format(reasonFormat, args), propertyName);
        }

        public CommandResult AddFormat(string reasonFormat, params object[] args)
        {
            return Add(string.Format(reasonFormat, args), null);
        }

        public CommandResult Add(string reason, string propertyName)
        {
            Errors.Add(new ErrorMessage {PropertyName = propertyName, Reason = reason});
            return this;
        }

        public CommandResult Add(string reason)
        {
            return Add(reason, null);
        }

        public override string ToString()
        {
            var msg = new StringBuilder();

            if (Success)
            {
                if (Warnings.Count == 0) msg.AppendLine("Success");
                else msg.AppendLine("Success With Warnings");
            }
            else
            {
                msg.AppendLine("Failed");
            }

            if (Errors.Count > 0)
            {
                msg.AppendLine("\nErrors\n-----");
                foreach (var ruleFailure in Errors)
                {
                    msg.AppendLine(ruleFailure.ToString());
                }
            }

            if (Warnings.Count > 0)
            {
                msg.AppendLine("\nWarnings\n-------");
                foreach (var ruleFailure in Errors)
                {
                    msg.AppendLine(ruleFailure.ToString());
                }
            }

            return msg.ToString();
        }
    }
}