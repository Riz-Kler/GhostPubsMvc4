using System;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;

namespace Carnotaurus.GhostPubsMvc.Common.Bespoke
{
    public class StringResult
    {
        public StringResult()
        {
            ResultType = ResultTypeEnum.Fail;

            Result = string.Empty;
        }

        public ResultTypeEnum ResultType { get; set; }

        public String Result { get; set; }
    }
}