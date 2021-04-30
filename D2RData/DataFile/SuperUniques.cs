using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// SuperUniques.txt controller.
    /// </summary>
    public class SuperUniques
    {
        private static SuperUniques _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static SuperUniques Instance
        {
            get
            {
                if (_instance == null) _instance = new SuperUniques();
                return _instance;
            }
        }

        private ListDict<string, SuperUnique> _superUniques = null;
        private Dictionary<string, List<SuperUnique>> _classMap = null;

        /// <summary>
        /// Gets SuperUnique list by class.
        /// </summary>
        /// <param name="cls">Monster class</param>
        /// <returns></returns>
        public List<SuperUnique> GetSuperUniquesByClass(string cls)
        {
            if (_classMap.TryGetValue(cls, out List<SuperUnique> list)) return list;
            return null;
        }
        
        /// <summary>
        /// Gets specified SuperUnique.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public SuperUnique this[string id]
        {
            get
            {
                if (_superUniques == null) return null;
                if (_superUniques.TryGetValue(id, out SuperUnique item)) return item;
                return null;
            }
        }

        /// <summary>
        /// Gets specified SuperUnique.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns></returns>
        public SuperUnique this[int index]
        {
            get
            {
                if (_superUniques == null) return null;
                return _superUniques.At(index);
            }
        }

        /// <summary>
        /// Gets total SuperUnique count.
        /// </summary>
        public int Count
        {
            get
            {
                return _superUniques == null ? 0 : _superUniques.Count;
            }
        }

        /// <summary>
        /// Parse data file SuperUniques.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseDataFile(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Parsing SuperUniques ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.SuperUniques];
            if (data == null)
            {
                LogHelper.Log(log, "Load SuperUniques.txt first!", LogLevel.Error);
                return;
            }

            _superUniques = new ListDict<string, SuperUnique>(data.RowCount);
            _classMap = new Dictionary<string, List<SuperUnique>>(data.RowCount);
            for (int i = 0; i < data.RowCount; i++)
            {
                var name = data[i, "Superunique"];
                if (string.IsNullOrEmpty(name))
                {
                    LogHelper.Log(log, $"Null or empty Superunique for SuperUniques row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_superUniques.ContainsKey(name))
                {
                    LogHelper.Log(log, $"Superunique [{name}] already exists, SuperUniques row {i} skipped.", LogLevel.Warning);
                    continue;
                }

                string displayName = data[i, "Name"];
                string cls = data[i, "Class"];
                string tc = data[i, "TC"];
                string tcN = data[i, "TC(N)"];
                string tcH = data[i, "TC(H)"];
                bool removed = DataHelper.ParseBool(data[i, "removed"]);

                // Skip removed ones
                if (removed) continue;

                // Skip no drop SuperUniques
                if (string.IsNullOrEmpty(tc + tcN + tcH)) continue;

                SuperUnique superUnique = new SuperUnique(name, displayName, cls, tc, tcN, tcH,
                    DataHelper.ParseInt(data[i, "Level"]),
                    DataHelper.ParseInt(data[i, "Level(N)"]),
                    DataHelper.ParseInt(data[i, "Level(H)"]),
                    data[i, "Area"]
                    );
                _superUniques.Add(name, superUnique);
                if (!_classMap.ContainsKey(cls)) _classMap.Add(cls, new List<SuperUnique>());
                _classMap[cls].Add(superUnique);
            }
            LogHelper.Log(log, $"Parsed {_superUniques.Count} SuperUnique(s)");
        }
    }

    /// <summary>
    /// summary
    /// </summary>
    public class SuperUnique
    {
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Class { get; private set; }
        public Dictionary<Difficulty, string> TreasureClasses { get; private set; }
        public Dictionary<Difficulty, int> Level { get; private set; }
        public string Area { get; private set; }

        private SuperUnique() { }

        public SuperUnique(string name, string displayName, string cls, string tc, string tcN, string tcH, int lv, int lvN, int lvH, string area)
        {
            Name = name;
            DisplayName = displayName;
            Class = cls;
            Area = area;
            TreasureClasses = new Dictionary<Difficulty, string>()
            {
                { Difficulty.Normal, tc },
                { Difficulty.Nightmare, tcN },
                { Difficulty.Hell, tcH },
            };
            Level = new Dictionary<Difficulty, int>()
            {
                { Difficulty.Normal, lv },
                { Difficulty.Nightmare, lvN },
                { Difficulty.Hell, lvH },
            };
        }
    }
}
