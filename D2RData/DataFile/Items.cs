using D2Data.DataFile;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// Items library.
    /// </summary>
    public class Items
    {
        private static Items _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static Items Instance
        {
            get
            {
                if (_instance == null) _instance = new Items();
                return _instance;
            }
        }

        private ListDict<string, Item> _items = null;

        /// <summary>
        /// Gets item by code.
        /// </summary>
        /// <param name="code">Item code.</param>
        /// <returns></returns>
        public Item this[string code]
        {
            get
            {
                if (_items == null) return null;
                if (_items.TryGetValue(code, out Item item)) return item;
                return null;
            }
        }

        /// <summary>
        /// Gets item by index.
        /// </summary>
        /// <param name="index">Item index.</param>
        /// <returns></returns>
        public Item this[int index]
        {
            get
            {
                if (_items == null) return null;
                return _items.At(index);
            }
        }

        /// <summary>
        /// Gets total count of items.
        /// </summary>
        public int Count
        {
            get
            {
                return _items == null ? 0 : _items.Count;
            }
        }

        private int _index = 0;

        public void ResetIndex()
        {
            _index = 0;
        }

        /// <summary>
        /// Parse Weapons.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseWeapons(Action<string, LogLevel> log)
        {
            LogHelper.Log(log, "Parsing Weapons ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.Weapons];
            if (data == null)
            {
                LogHelper.Log(log, "Load Weapons.txt first!", LogLevel.Error);
                return;
            }

            _items ??= new ListDict<string, Item>(data.RowCount * 3);

            for (int i = 0; i < data.RowCount; i++)
            {
                var code = data[i, "code"];
                if (string.IsNullOrEmpty(code))
                {
                    LogHelper.Log(log, $"Null or empty code for Weapons row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_items.ContainsKey(code))
                {
                    LogHelper.Log(log, $"Code [{code}] already exists, Weapons row {i} skipped.", LogLevel.Warning);
                    continue;
                }
                var item = new Item
                (
                    _index++,
                    code,
                    data[i, "name"],
                    data[i, "name"],
                    data[i, "type"],
                    "weap",
                    data[i, "ubercode"],
                    data[i, "ultracode"],
                    DataHelper.ParseInt(data[i, "level"]),
                    DataHelper.ParseInt(data[i, "levelreq"]),
                    ItemTypes.Instance.GetRarityFromItemType(data[i, "type"]),
                    data[i, "spawnable"] == "1",
                    false
                );
                _items.Add(code, item);
                if (item.Spawnable)
                {
                    if (item.Type1 != null) ItemTypes.Instance.AddToSpawnablePool(item.Type1.Code, item);
                    if (item.Type2 != null) ItemTypes.Instance.AddToSpawnablePool(item.Type2.Code, item);
                }
            }
        }

        /// <summary>
        /// Parse Armors.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseArmors(Action<string, LogLevel> log)
        {
            LogHelper.Log(log, "Parsing Armor ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.Armor];
            if (data == null)
            {
                LogHelper.Log(log, "Load Armor.txt first!", LogLevel.Error);
                return;
            }

            _items ??= new ListDict<string, Item>(data.RowCount * 3);

            for (int i = 0; i < data.RowCount; i++)
            {
                var code = data[i, "code"];
                if (string.IsNullOrEmpty(code))
                {
                    LogHelper.Log(log, $"Null or empty code for Armor row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_items.ContainsKey(code))
                {
                    LogHelper.Log(log, $"Code [{code}] already exists, Armor row {i} skipped.", LogLevel.Warning);
                    continue;
                }
                var item = new Item
                (
                    _index++,
                    code,
                    data[i, "name"],
                    data[i, "name"],
                    data[i, "type"],
                    "armo",
                    data[i, "ubercode"],
                    data[i, "ultracode"],
                    DataHelper.ParseInt(data[i, "level"]),
                    DataHelper.ParseInt(data[i, "levelreq"]),
                    ItemTypes.Instance.GetRarityFromItemType(data[i, "type"]),
                    data[i, "spawnable"] == "1",
                    false
                );
                _items.Add(code, item);
                if (item.Spawnable)
                {
                    if (item.Type1 != null) ItemTypes.Instance.AddToSpawnablePool(item.Type1.Code, item);
                    if (item.Type2 != null) ItemTypes.Instance.AddToSpawnablePool(item.Type2.Code, item);
                }
            }
        }

        /// <summary>
        /// Parse Misc.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseMisc(Action<string, LogLevel> log)
        {
            LogHelper.Log(log, "Parsing Misc ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.Misc];
            if (data == null)
            {
                LogHelper.Log(log, "Load Misc.txt first!", LogLevel.Error);
                return;
            }

            _items ??= new ListDict<string, Item>(data.RowCount * 3);

            for (int i = 0; i < data.RowCount; i++)
            {
                var code = data[i, "code"];
                if (string.IsNullOrEmpty(code))
                {
                    LogHelper.Log(log, $"Null or empty code for Misc row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_items.ContainsKey(code))
                {
                    LogHelper.Log(log, $"Code {code} already exists, Misc row {i} skipped.", LogLevel.Warning);
                    continue;
                }
                var item = new Item
                (
                    _index++,
                    code,
                    data[i, "name"],
                    data[i, "*name"],
                    data[i, "type"],
                    "misc",
                    null,
                    null,
                    DataHelper.ParseInt(data[i, "level"]),
                    DataHelper.ParseInt(data[i, "levelreq"]),
                    DataHelper.ParseInt(data[i, "rarity"]),
                    data[i, "spawnable"] == "1",
                    true
                );
                _items.Add(code, item);
                if (item.Spawnable)
                {
                    if (item.Type1 != null) ItemTypes.Instance.AddToSpawnablePool(item.Type1.Code, item);
                    if (item.Type2 != null) ItemTypes.Instance.AddToSpawnablePool(item.Type2.Code, item);
                }
            }
        }
    }

    /// <summary>
    /// Item class.
    /// </summary>
    public class Item
    {
        public int Index { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public ItemTypeItem Type1 { get; private set; }
        public ItemTypeItem Type2 { get; private set; }
        public string ExcCode { get; private set; }
        public string EliCode { get; private set; }
        public bool IsUber
        {
            get
            {
                return Code == ExcCode || Code == EliCode;
            }
        }
        public int Level { get; private set; }
        public int LevelReq { get; private set; }
        public int Rarity { get; private set; }
        public bool Spawnable { get; private set; }
        public bool IsClassSpecific
        {
            get
            {
                return Type1 != null && !string.IsNullOrEmpty(Type1.ClassOnly) 
                    || Type2 != null && !string.IsNullOrEmpty(Type2.ClassOnly);
            }
        }
        public bool IsMisc { get; private set; }

        public Item(int index, string code, string name, string displayName, string type1, string type2, string excCode, string eliCode,
            int level, int levelReq, int rarity, bool spawnable, bool misc)
        {
            Index = index;
            Code = code;
            Name = name;
            DisplayName = displayName;
            if (!string.IsNullOrEmpty(type1))
            {
                Type1 = ItemTypes.Instance[type1];
            }
            if (!string.IsNullOrEmpty(type2))
            {
                Type2 = ItemTypes.Instance[type2];
            }
            ExcCode = excCode;
            EliCode = eliCode;
            Level = level;
            LevelReq = levelReq;
            Rarity = rarity;
            Spawnable = spawnable;
            IsMisc = misc;
        }
    }

    /// <summary>
    /// Special item (unique or set) class.
    /// </summary>
    public class SpecialItem
    {
        public string Name { get; private set; }
        public string Set { get; private set; }
        public string Code { get; private set; }
        public string ItemName { get; private set; }
        public int Rarity { get; private set; }
        public int Level { get; private set; }
        public int LevelReq { get; private set; }

        public SpecialItem(string name, string set, string code, string itemName, int rarity, int level, int levelReq)
        {
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
