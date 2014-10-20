namespace Carnotaurus.GhostPubsMvc.Common.Result
{
    public class ErrorMessage
    {
        public string PropertyName { get; set; }

        public string Reason { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(PropertyName)) return Reason;
            return string.Format("{0} : {1}", PropertyName, Reason);
        }
    }
}