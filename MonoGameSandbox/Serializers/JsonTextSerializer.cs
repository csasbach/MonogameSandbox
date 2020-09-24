using System;
using System.Text.Json;
using Utilities.Abstractions;

namespace MonoGameSandbox.Serializers
{
    /// <summary>
    /// Used to create human readable data files.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class JsonTextSerializer<TData> : ITextSerializer<TData>
    {
        public Type SerializedDataType => typeof(string);

        public TData Deserialize(string serialized)
        {
            return JsonSerializer.Deserialize<TData>(serialized);
        }

        public string Serialize(TData serializable)
        {
            return JsonSerializer.Serialize(serializable);
        }
    }
}
