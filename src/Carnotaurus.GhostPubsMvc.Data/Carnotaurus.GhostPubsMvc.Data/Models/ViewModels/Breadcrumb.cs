namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class Breadcrumb
    {
        public PageLinkModel Region { get; set; }
        public PageLinkModel Authority { get; set; }
        public PageLinkModel Locality { get; set; }
        public PageLinkModel Organisation { get; set; }


        public Breadcrumb Swap(Breadcrumb lineage)
        {
            Region = lineage.Authority;
            Authority = lineage.Locality;
            Locality = lineage.Organisation;

            return this;
        }
    }
}