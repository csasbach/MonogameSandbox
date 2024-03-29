﻿using Microsoft.Xna.Framework;
using System;
using System.IO;
using Utilities.Abstractions;

namespace Utilities.Services
{
    public class SaveService<TSerializable, TSerializer> : ServiceBase, ISaveService<TSerializable, TSerializer> where TSerializer : ISerializer
    {
        private readonly ISerializer _serializer;

        public string SaveDirectory { get; set; }

        public SaveService(Game game) : base(game, typeof(ISaveService<TSerializable, TSerializer>))
        {
            _serializer = Activator.CreateInstance<TSerializer>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public bool TrySaveData(string path, TSerializable saveData)
        {
            using (var scope = Logger.BeginScope($"{nameof(SaveService<TSerializable, TSerializer>)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                try
                {
                    var fullPath = Path.Combine(SaveDirectory, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    File.WriteAllBytes(fullPath, _serializer.Serialize(saveData));
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
            using (var scope = Logger.BeginScope($"{nameof(SaveService<TSerializable, TSerializer>)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                loadData = default;
                try
                {
                    var fullPath = Path.Combine(SaveDirectory, path);
                    loadData = _serializer.Deserialize<TSerializable>(File.ReadAllBytes(fullPath));
                    return true;
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
