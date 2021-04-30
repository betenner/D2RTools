using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// Set item library.
    /// </summary>
    public class SetItems
    {
        private static SetItems _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static SetItems Instance
        {
            get
            {
                if (_instance == null) _instance = new SetItems();
                return _instance;
            }
        }

        private ListDict<string, SetItem> _items = null;
        private Dictionary<string, List<SetItem>> _codeMap = null;

        /// <summary>
        /// Gets SetItem by name.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public SetItem this[string name]
        {
            get
            {
                if (_items == null) return null;
                if (_items.TryGetValue(name, out var item)) return item;
                return null;
            }
        }

        /// <summary>
        /// Gets SetItem by index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns></returns>
        public SetItem this[int index]
        {
            get
            {
                if (_items == null) return null;
                return _items.At(index);
            }
        }

        /// <summary>
        /// Gets total count of set items.
        /// </summary>
        public int Count
        {
            get
            {
                return _items == null ? 0 : _items.Count;
            }
        }

        /// <summary>
        /// Gets item list by code.
        /// </summary>
        /// <param name="code">Code</param>
        /// <returns></returns>
        public List<SetItem> GetItemListByCode(string code)
        {
            if (_codeMap == null) return null;
            if (_codeMap.TryGetValue(code, out var list)) return list;
            return null;
        }

        /// <summary>
        /// Parse SetItems.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseDataFile(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Parsing SetItems ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.SetItems];
            if (data == null)
            {
                LogHelper.Log(log, "Load SetItems.txt first!", LogLevel.Error);
                return;
            }

            _items = new ListDict<string, SetItem>(data.RowCount);
            _codeMap = new Dictionary<string, List<SetItem>>(ItemTypes.Instance.Count);
            for (int i = 0; i < data.RowCount; i++)
            {
                var name = data[i, "index"];
                if (string.IsNullOrEmpty(name))
                {
                    LogHelper.Log(log, $"Null or empty name for SetItems row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_items.ContainsKey(name))
                {
                    LogHelper.Log(log, $"Name [{name}] already exists, SetItems row {i} skipped.", LogLevel.Warning);
                    continue;
                }
                SetItem item = new SetItem
                (
                    i,
                    name,
                    data[i, "set"],
                    data[i, "item"],
                    data[i, "*item"],
                    DataHelper.ParseInt(data[i, "rarity"]),
                    DataHelper.ParseInt(data[i, "lvl"]),
                    DataHelper.ParseInt(data[i, "lvl req"])
                );
                _items.Add(name, item);
                if (!_codeMap.ContainsKey(item.Code)) _codeMap.Add(item.Code, new List<SetItem>(20));
                _codeMap[item.Code].Add(item);
            }
            LogHelper.Log(log, $"Parsed {Count} set item(s).");
        }
    }

    /// <summary>
    /// Set item class.
    /// </summary>
    public class SetItem
    {
        public int Index { get; private set; }
        public string Name { get; private set; }
        public string Set { get; private set; }
        public string Code { get; private set; }
        public string ItemName { get; private set; }
        public int Rarity { get; private set; }
        public int Level { get; private set; }
        public int LevelReq { get; private set; }

        public SetItem(int index, string name, string set, string code, string itemName, int rarity, int level, int levelReq)
        {
            Index = index;
            Name = name;
            Set = set;
            Code = code;
            ItemName = itemName;
            Rarity = rarity;
            Level = level;
            LevelReq = levelReq;
        }
    }
}
