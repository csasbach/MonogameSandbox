using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities.Abstractions;

namespace MonoGameSandbox.Serializers
{
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
