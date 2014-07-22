namespace Carnotaurus.GhostPubsMvc.Common.Result
{
    public class ErrorMessage
    {
        public string PropertyName { get; set; }

        public string Reason { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(PropertyName)) return Reason;
            return PropertyName + " : " + Reason;
        }
    }
}