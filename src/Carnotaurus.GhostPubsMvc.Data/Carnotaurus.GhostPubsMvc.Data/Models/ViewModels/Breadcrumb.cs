using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class Breadcrumb
    {
        public LinkModel Region { get; set; }
        public LinkModel County { get; set; }
        public LinkModel Town { get; set; }
        public LinkModel Pub { get; set; }

    }

}
