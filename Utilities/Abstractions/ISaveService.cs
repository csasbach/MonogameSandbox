namespace Utilities.Abstractions
{
    public interface ISaveService<TSerializable, TSerializer> where TSerializer : ISerializer
    {
        bool TrySaveData(string path, TSerializable saveData);
        bool TryLoadData(string path, out TSerializable loadData);
    }
}
