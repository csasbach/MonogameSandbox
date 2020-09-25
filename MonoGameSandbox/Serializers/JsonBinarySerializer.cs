using System;
using System.Text;
using System.Text.Json;
using Utilities.Abstractions;

namespace MonoGameSandbox.Serializers
{
    /// <summary>
    /// Used to obfuscate data files so that they are not human readable.
    /// </summary>
    public class JsonBinarySerializer : ISerializer
    {
        public Type SerializedDataType => typeof(byte[]);

        public TData Deserialize<TData>(byte[] serialized)
        {
            for (int i = 0; i < serialized.Length; i++)
            {
                serialized[i] ^= 255;
            }
            var json = Encoding.UTF8.GetString(serialized);
            return JsonSerializer.Deserialize<TData>(json);
        }

        public byte[] Serialize<TData>(TData serializable)
        {
            var json = JsonSerializer.Serialize(serializable);
            var bytes = Encoding.UTF8.GetBytes(json);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= 255;
            }
            return bytes;
        }
    }
}
