using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carnotaurus.GhostPubsMvc.Common.Extensions;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{

    public class TownModel
    {
        public TownModel(String path)
        {
            Path = path;

            var arr = Path.SplitOnSlash();

            Region = arr[0];
            County = arr[1];
            Town = arr[2];
        }

        public String Path { get; set; }

        public String Region { get; set; }
        public String County { get; set; }
        public String Town { get; set; }

        public String FriendlyDescription
        {
            get { return Path.SplitOnSlash().JoinWithCommaReserve(); }
        }
    }
  
}
