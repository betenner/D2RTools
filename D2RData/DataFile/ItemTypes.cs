using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// ItemTypes.txt controller.
    /// </summary>
    public class ItemTypes
    {
        private static ItemTypes _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static ItemTypes Instance
        {
            get
            {
                if (_instance == null) _instance = new ItemTypes();
                return _instance;
            }
        }

        private ListDict<string, ItemTypeItem> _items = null;
        private Dictionary<string, Dictionary<int, HashSet<Item>>> _spawnableMap = null;
        
        /// <summary>
        /// Auto generate treasure class types list.
        /// </summary>
        public HashSet<string> GenTCList { get; private set; }

        /// <summary>
        /// Gets specified ItemTypeItem.
        /// </summary>
        /// <param name="code">Code</param>
        /// <returns></returns>
        public ItemTypeItem this[string code]
        {
            get
            {
                if (_items == null) return null;
                if (_items.TryGetValue(code, out ItemTypeItem item)) return item;
                return null;
            }
        }

        /// <summary>
        /// Gets specified ItemTypeItem.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns></returns>
        public ItemTypeItem this[int index]
        {
            get
            {
                if (_items == null) return null;
                return _items.At(index);
            }
        }

        /// <summary>
        /// Gets total item count.
        /// </summary>
        public int Count
        {
            get
            {
                return _items == null ? 0 : _items.Count;
            }
        }

        /// <summary>
        /// Gets item rarity from item type
        /// </summary>
        /// <param name="type">Item type</param>
        /// <returns></returns>
        public int GetRarityFromItemType(string type)
        {
            var itemType = Instance[type];
            if (itemType == null) return 0;
            return itemType.Rarity;
        }

        /// <summary>
        /// Adds an item to spawnable pool.
        /// </summary>
        /// <param name="type">Item type</param>
        /// <param name="code">Item code</param>
        /// <param name="log">Log callback</param>
        public void AddToSpawnablePool(string type, Item item, Action<string, LogLevel> log = null)
        {
            if (_items == null)
            {
                LogHelper.Log(log, "Parse item data files first!", LogLevel.Error);
                return;
            }
            _spawnableMap ??= new Dictionary<string, Dictionary<int, HashSet<Item>>>(_items.Count);
            if (!_spawnableMap.ContainsKey(type)) _spawnableMap.Add(type, new Dictionary<int, HashSet<Item>>());
            var subDict = _spawnableMap[type];
            if (!subDict.ContainsKey(item.Level)) subDict.Add(item.Level, new HashSet<Item>());
            subDict[item.Level].Add(item);
        }
        
        /// <summary>
        /// Gets a spawnable pool.
        /// </summary>
        /// <param name="type">Item type</param>
        /// <returns></returns>
        public Dictionary<int, HashSet<Item>> GetSpawnablePool(string type)
        {
            if (_spawnableMap == null) return null;
            if (_spawnableMap.TryGetValue(type, out var pool)) return pool;
            return null;
        }

        /// <summary>
        /// Parse data file ItemTypes.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseDataFile(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Parsing ItemTypes ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.ItemTypes];
            if (data == null)
            {
                LogHelper.Log(log, "Load ItemTypes.txt first!", LogLevel.Error);
                return;
            }

            _items = new ListDict<string, ItemTypeItem>(data.RowCount);
            GenTCList = new HashSet<string>();
            for (int i = 0; i < data.RowCount; i++)
            {
                var code = data[i, "Code"];
                if (string.IsNullOrEmpty(code))
                {
                    LogHelper.Log(log, $"Null or empty code for ItemType row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_items.ContainsKey(code))
                {
                    LogHelper.Log(log, $"Code [{code}] already exists, ItemType row {i} skipped.", LogLevel.Warning);
                    continue;
                }
                ItemTypeItem item = new ItemTypeItem
                (
                    code,
                    data[i, "ItemType"],
                    data[i, "Equiv1"],
                    data[i, "Equiv2"],
                    data[i, "Repair"],
                    data[i, "Body"],
                    data[i, "BodyLoc1"],
                    data[i, "BodyLoc2"],
                    data[i, "Shoots"],
                    data[i, "Quiver"],
                    data[i, "Throwable"],
                    data[i, "Reload"],
                    data[i, "ReEquip"],
                    data[i, "AutoStack"],
                    data[i, "Rare"],
                    data[i, "Magic"],
                    data[i, "Normal"],
                    data[i, "Charm"],
                    data[i, "Gem"],
                    data[i, "Beltable"],
                    data[i, "MaxSock1"],
                    data[i, "MaxSock25"],
                    data[i, "MaxSock40"],
                    data[i, "TreasureClass"],
                    data[i, "Rarity"],
                    data[i, "StaffMods"],
                    data[i, "Class"],
                    data[i, "CostFormula"]
                );
                _items.Add(code, item);
                if (item.GenerateTreasureClass) GenTCList.Add(code);
            }
        }
    }

    /// <summary>
    /// Item for ItemTypes.txt
    /// </summary>
    public class ItemTypeItem
    {
        public string ItemType { get; private set; }
        public string Code { get; private set; }
        public HashSet<string> EquivCodes { get; private set; }
        public bool HasDurability { get; private set; }
        public bool CanBeEquipped { get; private set; }
        public HashSet<string> EquipSlots { get; private set; }
        public string AmmoUsed { get; private set; }
        public string AmmoFor { get; private set; }
        public bool Throwable { get; private set; }
        public bool Reload { get; private set; }
        public bool ReEquip { get; private set; }
        public bool AutoStack { get; private set; }
        public bool Rare { get; private set; }
        public bool Magic { get; private set; }
        public bool NormalOnly { get; private set; }
        public bool IsCharm { get; private set; }
        public bool IsGem { get; private set; }
        public bool Beltable { get; private set; }
        public int MaxSocket1to24 { get; private set; }
        public int MaxSocket25to39 { get; private set; }
        public int MaxSocket40 { get; private set; }
        public int Rarity { get; private set; }
        public bool GenerateTreasureClass { get; private set; }
        public int TreasureClassRarity { get; private set; }
        public string ClassMod { get; private set; }
        public string ClassOnly { get; private set; }
        public bool CostFormula { get; private set; }

        public bool CanBeRare
        {
            get
            {
                return Rare && !NormalOnly;
            }
        }

        public bool CanBeMagic
        {
            get
            {
                return !NormalOnly;
            }
        }

        public bool CanBeNormal
        {
            get
            {
                return !Magic;
            }
        }

        private ItemTypeItem() { }

        public ItemTypeItem(string code, string itemType, string equivCode1, string equivCode2, string hasDurability,
            string canBeEquipped, string equipSlot1, string equipSlot2, string ammoUsed, string ammoFor,
            string throwable, string reload, string reEquip, string autoStack, string rareOnly, string magicOnly,
            string normalOnly, string isCharm, string isGem, string beltable, string maxSocket1_24, string maxSocket25_39,
            string maxSocket40, string genTC, string tcRarity, string classMod, string classOnly, string costFormula)
        {
            Code = code;
            ItemType = itemType;
            EquivCodes = new HashSet<string>();
            if (!string.IsNullOrEmpty(equivCode1)) EquivCodes.Add(equivCode1);
            if (!string.IsNullOrEmpty(equivCode2)) EquivCodes.Add(equivCode2);
            HasDurability = DataHelper.ParseBool(hasDurability);
            CanBeEquipped = DataHelper.ParseBool(canBeEquipped);
            EquipSlots = new HashSet<string>();
            if (!string.IsNullOrEmpty(equipSlot1)) EquipSlots.Add(equipSlot1);
            if (!string.IsNullOrEmpty(equipSlot2)) EquipSlots.Add(equipSlot2);
            AmmoUsed = ammoUsed;
            AmmoFor = ammoFor;
            Throwable = DataHelper.ParseBool(throwable);
            Reload = DataHelper.ParseBool(reload);
            ReEquip = DataHelper.ParseBool(reEquip);
            AutoStack = DataHelper.ParseBool(autoStack);
            Rare = DataHelper.ParseBool(rareOnly);
            Magic = DataHelper.ParseBool(magicOnly);
            NormalOnly = DataHelper.ParseBool(normalOnly);
            IsCharm = DataHelper.ParseBool(isCharm);
            IsGem = DataHelper.ParseBool(isGem);
            Beltable = DataHelper.ParseBool(beltable);
            MaxSocket1to24 = DataHelper.ParseInt(maxSocket1_24);
            MaxSocket25to39 = DataHelper.ParseInt(maxSocket25_39);
            MaxSocket40 = DataHelper.ParseInt(maxSocket40);
            GenerateTreasureClass = DataHelper.ParseBool(genTC);
            TreasureClassRarity = DataHelper.ParseInt(tcRarity);
            ClassMod = classMod;
            ClassOnly = classOnly;
            CostFormula = DataHelper.ParseBool(costFormula);
            Rarity = DataHelper.ParseInt(tcRarity);
        }
    }
}
