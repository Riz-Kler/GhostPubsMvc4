using System;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;

namespace Carnotaurus.GhostPubsMvc.Data.Interfaces
{
    public interface IOutputViewModel
    {
        String JumboTitle { get; set; }

        PageTypeEnum Action { get; set; }
    }
}