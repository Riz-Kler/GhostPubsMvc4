using System;
using Carnotaurus.GhostPubsMvc.Common.Extensions;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class GeoPathModel
    {
        public GeoPathModel(String path)
        {
            Path = path;

            var arr = Path.SplitOnSlash();

            var itemCount = arr.Count;

            ParentOfRightmost = arr[itemCount - 2];
            Rightmost = arr[itemCount - 1];
        }

        public String Path { get; set; }

        public String ParentOfRightmost { get; set; }

        public String Rightmost { get; set; }

        public String FriendlyDescription
        {
            get { return Path.SplitOnSlash().JoinWithCommaReserve(); }
        }
    }
}