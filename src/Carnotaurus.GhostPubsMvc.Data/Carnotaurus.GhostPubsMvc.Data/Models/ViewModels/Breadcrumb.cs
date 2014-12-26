using System;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    [Serializable]
    public class Breadcrumb
    {
        public PageLinkModel Region { get; set; }
        public PageLinkModel Authority { get; set; }
        public PageLinkModel Locality { get; set; }
        public PageLinkModel Organisation { get; set; }
    }
}