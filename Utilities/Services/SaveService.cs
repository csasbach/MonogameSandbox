using Microsoft.Xna.Framework;
using System;
using System.IO;
using Utilities.Abstractions;

namespace Utilities.Services
{
    public class SaveService<TSerializable, TSerialized> : ServiceBase, ISaveService<TSerializable, TSerialized>
    {
        private readonly ISerializer<TSerialized> _serializer;

        public string SaveDirectory { get; set; }

        public SaveService(Game game, ISerializer<TSerialized> serializer) : base(game, typeof(ISaveService<TSerializable, TSerialized>))
        {
            _serializer = serializer;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public bool TrySaveData(string path, TSerializable saveData)
        {
            using (var scope = Logger.BeginScope($"{nameof(SaveService<TSerializable, TSerialized>)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                try
                {
                    var fullPath = Path.Combine(SaveDirectory, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    if (_serializer.SerializedDataType == typeof(string))
                    {
                        File.WriteAllText(fullPath, ((ITextSerializer<TSerializable>)_serializer).Serialize(saveData));
                    }
                    if (_serializer.SerializedDataType == typeof(byte[]))
                    {
                        File.WriteAllBytes(fullPath, ((IBinarySerializer<TSerializable>)_serializer).Serialize(saveData));
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.LogError(scope, "{A8DEF2DA-CBD9-4194-A798-E8BB8B3B1E62}", "Failed to save data.", e);
                }
                return false;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public bool TryLoadData(string path, out TSerializable loadData)
        {
            using (var scope = Logger.BeginScope($"{nameof(SaveService<TSerializable, TSerialized>)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                loadData = default;
                try
                {
                    var fullPath = Path.Combine(SaveDirectory, path);
                    if (_serializer.SerializedDataType == typeof(string))
                    {
                        loadData = ((ITextSerializer<TSerializable>)_serializer).Deserialize(File.ReadAllText(fullPath));
                        return true;
                    }
                    if (_serializer.SerializedDataType == typeof(byte[]))
                    {
                        loadData = ((IBinarySerializer<TSerializable>)_serializer).Deserialize(File.ReadAllBytes(fullPath));
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(scope, "{A8DEF2DA-CBD9-4194-A798-E8BB8B3B1E62}", "Failed to load data.", e);
                }
                return false;
            }
        }
    }
}
