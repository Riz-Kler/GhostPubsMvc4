namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class Breadcrumb
    {
        public PageLinkModel Region { get; set; }
        public PageLinkModel County { get; set; }
        public PageLinkModel Town { get; set; }
        public PageLinkModel Pub { get; set; }
    }
}