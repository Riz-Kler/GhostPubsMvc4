using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carnotaurus.GhostPubsMvc.Web.Models
{

    public interface IOrgModel
    {
        String JumboTitle { get; set; }

        String Action { get; set; }
    }
     
}
