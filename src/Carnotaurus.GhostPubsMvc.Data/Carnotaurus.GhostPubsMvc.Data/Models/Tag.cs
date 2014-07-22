using System;
 
namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Tag
    {
        public int TagID { get; set; }
        public DateTime LastModified { get; set; }
        public int? OrgID { get; set; }
        public int FeatureID { get; set; }
        public virtual Feature Feature { get; set; }
        public virtual Org Org { get; set; }
    }
}