using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Result;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class CommandResultExtensions
    {
        public static CommandResult ValidateModel(this Object model)
        {
            if (model == null) return new CommandResult(null, "The model cannot be null");

            var result = new CommandResult();

            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);

            if (validationResults.Count > 0)
            {
                result = new CommandResult(null, "There were errors reported", validationResults)
                {
                    Errors = validationResults.Select(x => new ErrorMessage
                    {
                        PropertyName = x.MemberNames.FirstOrDefault(),
                        Reason = x.ErrorMessage
                    })
                        .ToList()
                };
            }

            return result;
        }
    }
}