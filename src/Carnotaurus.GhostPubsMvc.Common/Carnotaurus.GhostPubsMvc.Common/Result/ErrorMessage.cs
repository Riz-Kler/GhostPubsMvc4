using Carnotaurus.GhostPubsMvc.Common.Extensions;

namespace Carnotaurus.GhostPubsMvc.Common.Result
{
    public class ErrorMessage
    {
        public string PropertyName { get; set; }

        public string Reason { get; set; }

        public override string ToString()
        {
            string result = PropertyName.IsNullOrWhiteSpace() ?
                Reason : string.Format("{0} : {1}", PropertyName, Reason);

            return result;
        }
    }
}