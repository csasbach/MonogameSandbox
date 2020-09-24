using System.Runtime.Serialization;

namespace Utilities.Abstractions
{
    public interface ISaveService<TSerializable> : ISaveService
    {
        bool TrySaveData(string path, TSerializable saveData);
        bool TryLoadData(string path, out TSerializable loadData);
    }

    public interface ISaveService { }
}
