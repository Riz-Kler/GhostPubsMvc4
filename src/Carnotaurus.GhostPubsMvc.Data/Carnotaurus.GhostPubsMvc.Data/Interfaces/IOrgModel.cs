using System;

namespace Carnotaurus.GhostPubsMvc.Data.Interfaces
{
    public interface IOutputViewModel
    {
        String JumboTitle { get; set; }

        String Action { get; set; }
    }
}