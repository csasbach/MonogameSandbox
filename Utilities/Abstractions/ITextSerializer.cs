using System;

namespace Utilities.Abstractions
{
    public interface ITextSerializer<TSerializable> : ISerializer<string>
    {
        string Serialize(TSerializable serializable);
        TSerializable Deserialize(string serialized);
    }

    public interface IBinarySerializer<TSerializable> : ISerializer<byte[]>
    {
        byte[] Serialize(TSerializable serializable);
        TSerializable Deserialize(byte[] serialized);
    }

    public interface ISerializer<TSerialized>
    {
        /// <summary>
        /// should return typeof(TSerialized)
        /// </summary>
        Type SerializedDataType { get; }
    }
}
