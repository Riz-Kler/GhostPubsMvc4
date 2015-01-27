using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class GenericExtensions
    {
        public static T DeepClone<T>(this T obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(stream, obj);

                stream.Position = 0;

                var result = (T)formatter.Deserialize(stream);

                return result;
            }
        }
    }
}