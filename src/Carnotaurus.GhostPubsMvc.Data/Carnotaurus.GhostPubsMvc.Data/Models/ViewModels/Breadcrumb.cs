using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
