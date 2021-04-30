using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// Unique item library.
    /// </summary>
    public class UniqueItems
    {
        private static UniqueItems _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static UniqueItems Instance
        {
            get
            {
                if (_instance == null) _instance = new UniqueItems();
                return _instance;
            }
        }

        private ListDict<string, List<UniqueItem>> _items = null;
        private Dictionary<string, List<UniqueItem>> _codeMap = null;

        /// <summary>
        /// Gets UniqueItem by name.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public List<UniqueItem> this[string name]
        {
            get
            {
                if (_items == null) return null;
                if (_items.TryGetValue(name, out var item)) return item;
                return null;
            }
        }

        /// <summary>
        /// Gets UniqueItem by index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns></returns>
        public List<UniqueItem> this[int index]
        {
            get
            {
                if (_items == null) return null;
                return _items.At(index);
            }
        }

        /// <summary>
        /// Gets total count of unique items.
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
        public List<UniqueItem> GetItemListByCode(string code)
        {
            if (_codeMap == null) return null;
            if (_codeMap.TryGetValue(code, out var list)) return list;
            return null;
        }

        /// <summary>
        /// Parse UniqueItems.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseDataFile(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Parsing UniqueItems ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.UniqueItems];
            if (data == null)
            {
                LogHelper.Log(log, "Load UniqueItems.txt first!", LogLevel.Error);
                return;
            }

            _items = new ListDict<string, List<UniqueItem>>(data.RowCount);
            _codeMap = new Dictionary<string, List<UniqueItem>>(ItemTypes.Instance.Count);
            for (int i = 0; i < data.RowCount; i++)
            {
                var name = data[i, "index"];
                if (string.IsNullOrEmpty(name))
                {
                    LogHelper.Log(log, $"Null or empty name for UniqueItems row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (data[i, "enabled"] != "1") continue;
                UniqueItem item = new UniqueItem
                (
                    i,
                    name,
                    data[i, "code"],
                    data[i, "*type"],
                    DataHelper.ParseInt(data[i, "rarity"]),
                    DataHelper.ParseInt(data[i, "lvl"]),
                    DataHelper.ParseInt(data[i, "lvl req"]),
                    DataHelper.ParseBool(data[i, "ladder"])
                );
                if (!_items.ContainsKey(name)) _items.Add(name, new List<UniqueItem>(1));
                _items[name].Add(item);
                if (!_codeMap.ContainsKey(item.Code)) _codeMap.Add(item.Code, new List<UniqueItem>(20));
                _codeMap[item.Code].Add(item);
            }
            LogHelper.Log(log, $"Parsed {Count} unique item(s).");
        }
    }

    /// <summary>
    /// Unique item class.
    /// </summary>
    public class UniqueItem
    {
        public int Index { get; private set; }
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string ItemName { get; private set; }
        public int Rarity { get; private set; }
        public int Level { get; private set; }
        public int LevelReq { get; private set; }
        public bool LadderOnly { get; private set; }

        public UniqueItem(int index, string name, string code, string itemName, int rarity, int level, int levelReq, bool ladderOnly)
        {
            Index = index;
            Name = name;
            Code = code;
            var item = Items.Instance[code];
            ItemName = item == null ? itemName : item.DisplayName;
            Rarity = rarity;
            Level = level;
            LevelReq = levelReq;
            LadderOnly = ladderOnly;
        }
    }
}
