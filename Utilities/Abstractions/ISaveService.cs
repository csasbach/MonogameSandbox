namespace Utilities.Abstractions
{
    public interface ISaveService<TSerializable, TSerialized>
    {
        bool TrySaveData(string path, TSerializable saveData);
        bool TryLoadData(string path, out TSerializable loadData);
    }
}
