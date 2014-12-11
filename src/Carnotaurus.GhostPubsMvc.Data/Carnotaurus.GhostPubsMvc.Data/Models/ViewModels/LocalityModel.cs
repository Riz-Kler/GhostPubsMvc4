using System;
using Carnotaurus.GhostPubsMvc.Common.Extensions;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class LocalityModel
    {
        public LocalityModel(String path)
        {
            Path = path;

            var arr = Path.SplitOnSlash();

            Region = arr[0];
            County = arr[1];
            Locality = arr[2];
        }

        public String Path { get; set; }

        public String Region { get; set; }
        public String County { get; set; }
        public String Locality { get; set; }

        public String FriendlyDescription
        {
            get { return Path.SplitOnSlash().JoinWithCommaReserve(); }
        }
    }
}