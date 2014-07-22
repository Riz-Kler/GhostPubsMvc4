using System;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions.InjectionExtensions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcludeFromDiRegistrationAttribute : Attribute
    {
    }
}