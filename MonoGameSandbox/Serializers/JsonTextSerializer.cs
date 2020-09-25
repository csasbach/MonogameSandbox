using System;
using System.Text;
using System.Text.Json;
using Utilities.Abstractions;

namespace MonoGameSandbox.Serializers
{
    /// <summary>
    /// Used to create human readable data files.
    /// </summary>
    public class JsonTextSerializer : ISerializer
    {
        public Type SerializedDataType => typeof(string);

        public TData Deserialize<TData>(byte[] serialized)
        {
            return JsonSerializer.Deserialize<TData>(Encoding.UTF8.GetString(serialized));
        }

        public byte[] Serialize<TData>(TData serializable)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(serializable));
        }
    }
}
