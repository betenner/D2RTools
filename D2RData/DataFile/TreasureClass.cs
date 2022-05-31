using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace D2Data.DataFile
{
    /// <summary>
    /// TreasureClassEx.txt controller.
    /// </summary>
    public class TreasureClass
    {
        /// <summary>
        /// Drop result.
        /// </summary>
        public class DropResult
        {
            public int DropLevel { get; private set; }
            public Item BaseItem { get; private set; }
            public ItemQuality Quality { get; private set; }
            public SetItem SetItem { get; private set; }
            public UniqueItem UniqueItem { get; private set; }

            public string ParamText { get; private set; }

            public string Name
            {
                get
                {
                    if (UniqueItem != null) return $"{UniqueItem.Name} {UniqueItem.ItemName}";
                    if (SetItem != null) return $"{SetItem.Name} {SetItem.ItemName}";
                    return BaseItem.DisplayName;
                }
            }

            public DropResult(int dropLevel, Item baseItem, ItemQuality quality, SetItem setItem = null, UniqueItem uniqueItem = null, string paramText = null)
            {
                DropLevel = dropLevel;
                BaseItem = baseItem;
                Quality = quality;
                SetItem = setItem;
                UniqueItem = uniqueItem;
                ParamText = paramText;
            }

            /// <summary>
            /// Unique ID
            /// </summary>
            public long Uid
            {
                get
                {
                    // 3-bit quality + 7-bit drop level + 16-bit UniqueItem index + 16-bit SetItem index + 16-bit Item index
                    long qualityBits = ((long)Quality << 55) & 0x0380000000000000L;
                    long dropLevelBits = (long)DropLevel << 48 & 0x01ff000000000000L;
                    long uniqueIndexBits = UniqueItem == null ? 0L : ((long)UniqueItem.Index << 32 & 0xffff00000000L);
                    long setIndexBits = SetItem == null ? 0L : ((long)SetItem.Index << 16 & 0xffff0000L);
                    long itemIndexBits = BaseItem.Index & 0xffffL;
                    return qualityBits | dropLevelBits | uniqueIndexBits | setIndexBits | itemIndexBits;
                }
            }

            private int _count = 1;

            /// <summary>
            /// Gets or sets the count.
            /// </summary>
            public int Count
            {
                get => _count;
                set
                {
                    _count = Math.Max(1, value);
                }
            }

            public override string ToString()
            {
                return $"{(DropLevel > 0 ? "[" + DropLevel + "]" : string.Empty)}{(BaseItem.IsMisc && DropLevel == 0 ? string.Empty : "[" + Quality + "]")}{(UniqueItem != null ? UniqueItem.Name : (SetItem != null ? SetItem.Name : BaseItem.Name))}{ParamText}";
            }
        }

        /// <summary>
        /// Max drop count per TreasureClass.
        /// </summary>
        public const int TC_MAX_DROP = 6;

        private const int AUTO_GEN_TC_INTERVAL = 3;
        private const int AUTO_GEN_TC_MINLEVEL = 1;
        private const int AUTO_GEN_TC_MAXLEVEL = 85;
        private const int MUL_DIVISOR = 256;

        private static TreasureClass _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TreasureClass Instance
        {
            get
            {
                if (_instance == null) _instance = new TreasureClass();
                return _instance;
            }
        }

        private ListDict<string, TreasureClassItem> _items = null;
        private Dictionary<string, HashSet<string>> _tcItemMap = null;
        private Dictionary<string, HashSet<string>> _itemTcMap = null;
        private Dictionary<string, HashSet<string>> _uniqueMap = null;
        private Dictionary<string, HashSet<string>> _setMap = null;
        private Dictionary<int, SortedList<int, TreasureClassItem>> _groupMap = null;

        // Gets TC in TC group
        private TreasureClassItem GetTCInGroup(int group, int dropLevel)
        {
            if (_groupMap.TryGetValue(group, out SortedList<int, TreasureClassItem> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list.Values[i].Level == dropLevel) return list.Values[i];
                    else if (list.Values[i].Level > dropLevel) return list.Values[i - 1];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets TreasureClassItem by name.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public TreasureClassItem this[string name]
        {
            get
            {
                if (_items == null) return null;
                if (_items.TryGetValue(name, out TreasureClassItem item)) return item;
                return null;
            }
        }

        /// <summary>
        /// Gets TreasureClassItem by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TreasureClassItem this[int index]
        {
            get
            {
                if (_items == null) return null;
                return _items.At(index);
            }
        }

        /// <summary>
        /// Gets total count of treasure classes.
        /// </summary>
        public int Count
        {
            get 
            {
                return _items == null ? 0 : _items.Count;
            }
        }

        /// <summary>
        /// Generates cross-references
        /// </summary>
        /// <param name="log">Log callback</param>
        public void GenerateXRef(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Generating cross-references for treasure classes ...");
            _itemTcMap = new Dictionary<string, HashSet<string>>(Items.Instance.Count);
            _tcItemMap = new Dictionary<string, HashSet<string>>(Count);
            for (int i = 0; i < Count; i++)
            {
                HashSet<string> list = new HashSet<string>();
                SearchXRef(this[i], list);
            }
            LogHelper.Log(log, $"Generated {_itemTcMap.Count} Item-to-TC xref(s) and {_tcItemMap.Count} TC-to-Item xref(s).");
        }

        private void SearchXRef(TreasureClassItem tc, HashSet<string> tcList)
        {
            tcList.Add(tc.Name);
            foreach (var tcItem in tc.ItemProbs)
            {
                var nextTc = this[tcItem.Key];

                // Try item
                if (nextTc == null)
                {
                    var item = Items.Instance[tcItem.Key];
                    if (item != null)
                    {
                        if (!_itemTcMap.ContainsKey(item.Code)) _itemTcMap.Add(item.Code, new HashSet<string>());
                        _itemTcMap[item.Code].UnionWith(tcList);
                        foreach (var subtc in tcList)
                        {
                            if (!_tcItemMap.ContainsKey(subtc)) _tcItemMap.Add(subtc, new HashSet<string>());
                            _tcItemMap[subtc].Add(item.Code);
                        }
                    }
                }
                else
                {
                    SearchXRef(nextTc, tcList);
                }
            }
            tcList.Remove(tc.Name);
        }

        /// <summary>
        /// Generates treasure classes from ItemTypes.
        /// </summary>
        /// <param name="log">Log callback</param>
        public void GenerateItemTypeTreasureClasses(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Generating auto treasure classes from ItemTypes ...");
            var list = ItemTypes.Instance.GenTCList;
            if (list == null || list.Count == 0)
            {
                LogHelper.Log(log, "No type to generate treasure class", LogLevel.Warning);
                return;
            }
            _items ??= new ListDict<string, TreasureClassItem>();
            foreach (var type in list)
            {
                for (int i = AUTO_GEN_TC_MINLEVEL; i <= AUTO_GEN_TC_MAXLEVEL; i += AUTO_GEN_TC_INTERVAL) 
                {
                    var tc = type + (i + AUTO_GEN_TC_INTERVAL - 1);
                    if (_items.ContainsKey(tc))
                    {
                        LogHelper.Log(log, $"Auto TreasureClass [{tc}] already exists, skipped.", LogLevel.Warning);
                        continue;
                    }
                    var tcItem = new TreasureClassItem(tc, i, i + AUTO_GEN_TC_INTERVAL - 1);
                    int count = 0;
                    for (int j = 0; j < AUTO_GEN_TC_INTERVAL; j++)
                    {
                        var pool = ItemTypes.Instance.GetSpawnablePool(type);
                        if (pool == null) continue;
                        if (pool.TryGetValue(j + i, out var set))
                        {
                            foreach (var item in set)
                            {
                                tcItem.AddItem(item.Code, item.Rarity);
                                count++;
                            }
                        }
                    }
                    if (count > 0)
                    {
                        _items.Add(tc, tcItem);
                        LogHelper.Log(log, $"Auto TreasureClass [{tc}] generated, contains {count} item(s).");
                    }
                }
            }
        }

        /// <summary>
        /// Parse data file TreasureClassEx.txt
        /// </summary>
        /// <param name="logCallback">Log callback</param>
        public void ParseDataFile(Action<string, LogLevel> logCallback = null)
        {
            LogHelper.Log(logCallback, "Parsing TreasureClassEx.txt ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(logCallback, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.TreasureClassEx];
            if (data == null)
            {
                LogHelper.Log(logCallback, "Load TreasureClassEx.txt first!", LogLevel.Error);
                return;
            }

            _items ??= new ListDict<string, TreasureClassItem>(data.RowCount);
            _groupMap ??= new Dictionary<int, SortedList<int, TreasureClassItem>>();
            for (int i = 0; i < data.RowCount; i++)
            {
                var name = data[i, "Treasure Class"];
                if (string.IsNullOrEmpty(name))
                {
                    LogHelper.Log(logCallback, $"Null or empty name for TreasureClassEx row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_items.ContainsKey(name))
                {
                    LogHelper.Log(logCallback, $"Name {name} already exists, TreasureClassEx row {i} skipped.", LogLevel.Warning);
                    continue;
                }
                TreasureClassItem item = new TreasureClassItem
                (
                    name,
                    DataHelper.ParseInt(data[i, "group"]),
                    DataHelper.ParseInt(data[i, "level"]),
                    DataHelper.ParseInt(data[i, "Picks"]),
                    DataHelper.ParseInt(data[i, "Unique"]),
                    DataHelper.ParseInt(data[i, "Set"]),
                    DataHelper.ParseInt(data[i, "Rare"]),
                    DataHelper.ParseInt(data[i, "Magic"]),
                    DataHelper.ParseInt(data[i, "NoDrop"]),
                    data[i, "Item1"], DataHelper.ParseInt(data[i, "Prob1"]),
                    data[i, "Item2"], DataHelper.ParseInt(data[i, "Prob2"]),
                    data[i, "Item3"], DataHelper.ParseInt(data[i, "Prob3"]),
                    data[i, "Item4"], DataHelper.ParseInt(data[i, "Prob4"]),
                    data[i, "Item5"], DataHelper.ParseInt(data[i, "Prob5"]),
                    data[i, "Item6"], DataHelper.ParseInt(data[i, "Prob6"]),
                    data[i, "Item7"], DataHelper.ParseInt(data[i, "Prob7"]),
                    data[i, "Item8"], DataHelper.ParseInt(data[i, "Prob8"]),
                    data[i, "Item9"], DataHelper.ParseInt(data[i, "Prob9"]),
                    data[i, "Item10"], DataHelper.ParseInt(data[i, "Prob10"])
                );
                _items.Add(name, item);
                if (item.Group != 0)
                {
                    if (!_groupMap.ContainsKey(item.Group)) _groupMap.Add(item.Group, new SortedList<int, TreasureClassItem>());
                    if (!_groupMap[item.Group].ContainsKey(item.Level)) _groupMap[item.Group].Add(item.Level, item);
                    else _groupMap[item.Group][item.Level] = item;
                }
            }
        }

        /// <summary>
        /// Rolls a speficied treasure class.
        /// </summary>
        /// <param name="tcName">Treausre class name</param>
        /// <param name="dropLevel">Drop level (area level)</param>
        /// <param name="mf">MF</param>
        /// <param name="totalPlayerCount">Total player count in game</param>
        /// <param name="partyPlayerCount">Total player count in party</param>
        /// <param name="log">Log callback</param>
        /// <returns></returns>
        public List<DropResult> Roll(string tcName, int dropLevel,
            int mf = 0, int partyPlayerCount = 1, int totalPlayerCount = 1,
            Action<string, LogLevel> log = null)
        {
            if (string.IsNullOrEmpty(tcName)) return null;
            List<DropResult> result = new List<DropResult>(TC_MAX_DROP);
            InnerRoll(tcName, result, dropLevel, mf, partyPlayerCount, totalPlayerCount, 0, 0, 0, 0, log);
            return result;
        }

        private void InnerRoll(string tcName, List<DropResult> result, int dropLevel, int mf = 0, 
            int partyPlayerCount = 1, int totalPlayerCount = 1,
            int uniqueBonus = 0, int setBonus = 0, int rareBonus = 0, int magicBonus = 0,
            Action<string, LogLevel> log = null)
        {
            if (string.IsNullOrEmpty(tcName) || result == null) return;

            // Max drop reached
            if (result.Count >= TC_MAX_DROP) return;

            // Get param
            int commaPos = tcName.IndexOf(',');
            string prmStr = null;
            string prmText = null;
            if (commaPos >= 0)
            {
                prmStr = tcName[(commaPos + 1)..];
                tcName = tcName.Substring(0, commaPos);
                prmText = GetParamText(prmStr);
            }

            // Try to find treasure class
            var tc = this[tcName];
            if (tc == null)
            {
                // Try item directly
                var dr = GetDropResult(tcName, dropLevel, prmText, mf, uniqueBonus, setBonus, rareBonus, magicBonus, log);
                if (dr != null) result.Add(dr);
                return;
            }
            else if (tc.Picks == 0)
            {
                LogHelper.Log(log, $"Treasure class [{tcName}] has 0 pick!", LogLevel.Warning);
                return;
            }

            // Find TC in group if possible
            if (tc.Group != 0)
            {
                var newTc = GetTCInGroup(tc.Group, dropLevel);
                if (newTc != null) tc = newTc;
            }

            // Guaranteed drop
            if (tc.Picks < 0)
            {
                for (int i = 0; i < -tc.Picks; i++)
                {
                    if (tc.ItemProbs.Count > i)
                    {
                        var item = tc.ItemProbs[i];
                        for (int j = 0; j < item.Value; j++)
                        {
                            InnerRoll(item.Key, result, dropLevel, mf, partyPlayerCount, totalPlayerCount,
                                Math.Max(tc.UniqueBonus, uniqueBonus),
                                Math.Max(tc.SetBonus, setBonus),
                                Math.Max(tc.RareBonus, rareBonus),
                                Math.Max(tc.MagicBonus, magicBonus),
                                log
                            );
                        }
                    }
                }
            }

            // Ratio drop
            else
            {
                // Auto TC
                if (tc.IsAutoTC)
                {
                    // Auto TC doesn't have 'no drop'
                    var code = tc.WeightList.GetRandomElement();
                    var dr = GetDropResult(code, dropLevel, prmText,
                        mf, uniqueBonus, setBonus, rareBonus, magicBonus, log);
                    if (dr != null) result.Add(dr);
                }

                // Normal TC
                else
                {
                    List<string> nextTcList = new List<string>(TC_MAX_DROP);
                    int noDrop = tc.NoDrop;
                    if (noDrop == 0)
                    {
                        for (int i = 0; i < tc.Picks; i++)
                        {
                            nextTcList.Add(tc.WeightList.GetRandomElement());
                        }
                    }
                    else
                    {
                        noDrop = GetAdjustedNoDropValue(noDrop, (int)tc.WeightList.TotalWeight,
                            GetEssentialPlayerCount(partyPlayerCount, totalPlayerCount));

                        // Adjust no drop
                        tc.WeightList.Add(null, (uint)noDrop);
                        for (int i = 0; i < tc.Picks; i++)
                        {
                            nextTcList.Add(tc.WeightList.GetRandomElement());
                        }
                        tc.WeightList.Remove(tc.WeightList.Count - 1);
                    }
                    foreach (var tn in nextTcList)
                    {
                        InnerRoll(tn, result, dropLevel, mf, partyPlayerCount, totalPlayerCount,
                            Math.Max(tc.UniqueBonus, uniqueBonus),
                            Math.Max(tc.SetBonus, setBonus),
                            Math.Max(tc.RareBonus, rareBonus),
                            Math.Max(tc.MagicBonus, magicBonus),
                            log
                            );
                    }
                }
            }
        }

        // Get drop result
        private DropResult GetDropResult(string code, int dropLevel, string prm = null,
            int mf = 0, int uniqueBonus = 0, int setBonus = 0, int rareBonus = 0, int magicBonus = 0,
            Action<string, LogLevel> log = null)
        {
            // Get Item
            var item = Items.Instance[code];
            if (item == null)
            {
                LogHelper.Log(log, $"Item [{code}] not found!", LogLevel.Warning);
                return null;
            }

            // Determine what to drop...

            // Special case
            if (item.Rarity == 0) return new DropResult(dropLevel, item, item.IsMisc ? ItemQuality.LowQuality : ItemQuality.Normal, null, null, prm);
            if (item.Level == 0) return new DropResult(0, item, item.IsMisc ? ItemQuality.LowQuality : ItemQuality.Normal, null, null, prm);

            // Try unique
            double dropValue = new Random().NextDouble();
            WeightList<UniqueItem> uniqueWeights = new WeightList<UniqueItem>();
            var uniqueList = UniqueItems.Instance.GetItemListByCode(code);
            if (uniqueList != null && uniqueList.Count > 0)
            {
                foreach (var uitem in uniqueList)
                {
                    if (uitem.Level <= dropLevel) uniqueWeights.Add(uitem, (uint)uitem.Rarity);
                }
                if (uniqueWeights.Count > 0)
                {
                    var dropUItem = uniqueWeights.GetRandomElement();
                    if (dropUItem != null)
                    {
                        var uniqueDrop = FormulaHelper.CalcFinalDropRate(ItemQuality.Unique, dropLevel, dropUItem.Level, uniqueBonus, mf, item.IsUber, item.IsClassSpecific);
                        if (dropValue <= uniqueDrop)
                        {
                            return new DropResult(dropLevel, item, ItemQuality.Unique, null, dropUItem, prm);
                        }
                    }
                }
            }
            // Roll failed, drop 3x Dur rare
            else if (!item.IsMisc)
            {
                return new DropResult(dropLevel, item, ItemQuality.Rare, null, null, " [3x Dur]");
            }

            // Try rare (rare check comes before set!)
            dropValue = new Random().NextDouble();
            if ((item.Type1 == null || item.Type1.CanBeRare) && (item.Type2 == null || item.Type2.CanBeRare))
            {
                var rareDrop = FormulaHelper.CalcFinalDropRate(ItemQuality.Rare, dropLevel, item.Level, rareBonus, mf, item.IsUber, item.IsClassSpecific);
                if (dropValue <= rareDrop)
                {
                    return new DropResult(dropLevel, item, ItemQuality.Rare, null, null, prm);
                }
            }

            // Try set
            dropValue = new Random().NextDouble();
            WeightList<SetItem> setWeights = new WeightList<SetItem>();
            var setList = SetItems.Instance.GetItemListByCode(code);
            if (setList != null && setList.Count > 0)
            {
                foreach (var sitem in setList)
                {
                    if (sitem.Level <= dropLevel) setWeights.Add(sitem, (uint)sitem.Rarity);
                }
                if (setWeights.Count > 0)
                {
                    var dropSItem = setWeights.GetRandomElement();
                    if (dropSItem != null)
                    {
                        var setDrop = FormulaHelper.CalcFinalDropRate(ItemQuality.Set, dropLevel, dropSItem.Level,
                            setBonus, mf, item.IsUber, item.IsClassSpecific);
                        if (dropValue <= setDrop)
                        {
                            return new DropResult(dropLevel, item, ItemQuality.Set, dropSItem, null, prm);
                        }
                    }
                }
            }
            // Roll failed, drop 2x Dur magic
            else if (!item.IsMisc)
            {
                return new DropResult(dropLevel, item, ItemQuality.Magic, null, null, " [2x Dur]");
            }

            // Try magic
            dropValue = new Random().NextDouble(); 
            if ((item.Type1 == null || item.Type1.CanBeMagic) && (item.Type2 == null || item.Type2.CanBeMagic))
            {
                var magicDrop = FormulaHelper.CalcFinalDropRate(ItemQuality.Magic, dropLevel, item.Level, magicBonus, mf, item.IsUber, item.IsClassSpecific);
                if (dropValue <= magicDrop)
                {
                    return new DropResult(dropLevel, item, ItemQuality.Magic, null, null, prm);
                }
            }


            // Try superior, normal & low quality
            if ((item.Type1 == null || item.Type1.CanBeNormal) && (item.Type2 == null || item.Type2.CanBeNormal))
            {
                // Misc item
                if (item.IsMisc) return new DropResult(0, item, ItemQuality.LowQuality, null, null, prm);

                // Superior
                dropValue = new Random().NextDouble();
                var hiDrop = FormulaHelper.CalcFinalDropRate(ItemQuality.Superior, dropLevel, item.Level, 0, 0, item.IsUber, item.IsClassSpecific);
                if (dropValue <= hiDrop)
                {
                    return new DropResult(dropLevel, item, ItemQuality.Superior, null, null, prm);
                }

                // Normal
                dropValue = new Random().NextDouble();
                var normalDrop = FormulaHelper.CalcFinalDropRate(ItemQuality.Normal, dropLevel, item.Level, 0, 0, item.IsUber, item.IsClassSpecific);
                if (dropValue <= normalDrop)
                {
                    return new DropResult(dropLevel, item, ItemQuality.Normal, null, null, prm);
                }

                // Low quality
                return new DropResult(dropLevel, item, ItemQuality.LowQuality, null, null, prm);
            }

            return null;
        }

        // Gets essential player count
        private int GetEssentialPlayerCount(int partyPlayerCount, int totalPlayerCount)
        {
            int otherPlayerCount = Math.Max(0, totalPlayerCount - partyPlayerCount);
            return partyPlayerCount + otherPlayerCount / 2;
        }

        // Gets adjusted no drop value from player count
        private int GetAdjustedNoDropValue(int noDrop, int otherTotal, int essentialPlayerCount)
        {
            if (essentialPlayerCount == 1) return noDrop;
            double oldNoDropRate = (double)noDrop / (noDrop + otherTotal);
            double adjustFactor = Math.Pow(oldNoDropRate, essentialPlayerCount);
            return (int)(otherTotal * adjustFactor / (1d - adjustFactor));
        }
        
        private string GetParamText(string prmStr)
        {
            if (prmStr.StartsWith("mul="))
            {
                if (int.TryParse(prmStr[4..], out int amount))
                {
                    return $" x{amount / 256}";
                }
            }
            return $" {prmStr}";
        }
    }

    /// <summary>
    /// Item for ItemTypes.txt
    /// </summary>
    public class TreasureClassItem
    {
        public string Name { get; private set; }
        public int Group { get; private set; }
        public int Level { get; private set; }
        public int Picks { get; private set; }
        public int UniqueBonus { get; private set; }
        public int SetBonus { get; private set; }
        public int RareBonus { get; private set; }
        public int MagicBonus { get; private set; }
        public int NoDrop { get; private set; }
        public List<KeyValuePair<string, int>> ItemProbs { get; private set; }
        public int TotalProb { get; private set; }
        public WeightList<string> WeightList { get; private set; }
        public bool IsAutoTC { get; private set; }
        public int AutoTCMinLevel { get; private set; }
        public int AutoTCMaxLevel { get; private set; }

        public TreasureClassItem(string name, int autoTCMinLevel, int autoTCMaxLevel)
        {
            Name = name;
            Picks = 1;
            ItemProbs = new List<KeyValuePair<string, int>>(10);
            WeightList = new WeightList<string>();
            AutoTCMinLevel = autoTCMinLevel;
            AutoTCMaxLevel = autoTCMaxLevel;
            IsAutoTC = true;
        }

        public TreasureClassItem(string name, int group, int level, int picks,
            int uniqueBonus, int setBonus, int rareBonus, int magicBonus, int noDrop, 
            string item1, int prob1,
            string item2, int prob2,
            string item3, int prob3,
            string item4, int prob4,
            string item5, int prob5,
            string item6, int prob6,
            string item7, int prob7,
            string item8, int prob8,
            string item9, int prob9,
            string item10, int prob10)
        {
            Name = name;
            Picks = picks;
            Group = group;
            Level = level;
            UniqueBonus = uniqueBonus;
            SetBonus = setBonus;
            RareBonus = rareBonus;
            MagicBonus = magicBonus;
            NoDrop = noDrop;
            ItemProbs = new List<KeyValuePair<string, int>>(10);
            WeightList = new WeightList<string>();
            AddItem(item1, prob1);
            AddItem(item2, prob2);
            AddItem(item3, prob3);
            AddItem(item4, prob4);
            AddItem(item5, prob5);
            AddItem(item6, prob6);
            AddItem(item7, prob7);
            AddItem(item8, prob8);
            AddItem(item9, prob9);
            AddItem(item10, prob10);
        }

        /// <summary>
        /// Adds an item and its prob.
        /// </summary>
        /// <param name="item">Item code</param>
        /// <param name="prob">Item prob</param>
        public void AddItem(string item, int prob)
        {
            if (!string.IsNullOrEmpty(item) && prob > 0)
            {
                ItemProbs.Add(new KeyValuePair<string, int>(item, prob));
                TotalProb += prob;
                WeightList.Add(item, (uint)prob);
            }
        }
    }
}
