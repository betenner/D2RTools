using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// Levels.txt controller.
    /// </summary>
    public class Levels
    {
        private static Levels _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static Levels Instance
        {
            get
            {
                if (_instance == null) _instance = new Levels();
                return _instance;
            }
        }

        private ListDict<string, Level> _levels = null;
        
        /// <summary>
        /// Gets specified Level.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public Level this[string id]
        {
            get
            {
                if (_levels == null) return null;
                if (_levels.TryGetValue(id, out Level item)) return item;
                return null;
            }
        }

        /// <summary>
        /// Gets specified Level.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns></returns>
        public Level this[int index]
        {
            get
            {
                if (_levels == null) return null;
                return _levels.At(index);
            }
        }

        /// <summary>
        /// Gets total Level count.
        /// </summary>
        public int Count
        {
            get
            {
                return _levels == null ? 0 : _levels.Count;
            }
        }

        /// <summary>
        /// Parse data file Levels.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseDataFile(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Parsing Levels ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.Levels];
            if (data == null)
            {
                LogHelper.Log(log, "Load Levels.txt first!", LogLevel.Error);
                return;
            }

            _levels = new ListDict<string, Level>(data.RowCount);
            for (int i = 0; i < data.RowCount; i++)
            {
                var name = data[i, "Name"];
                if (string.IsNullOrEmpty(name))
                {
                    LogHelper.Log(log, $"Null or empty name for Levels row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_levels.ContainsKey(name))
                {
                    LogHelper.Log(log, $"Name [{name}] already exists, Levels row {i} skipped.", LogLevel.Warning);
                    continue;
                }

                // Skip no level levels
                int monLevel = DataHelper.ParseInt(data[i, "MonLvl1Ex"]);
                int monLevelN = DataHelper.ParseInt(data[i, "MonLvl2Ex"]);
                int monLevelH = DataHelper.ParseInt(data[i, "MonLvl3Ex"]);
                if (monLevel == 0 || monLevelN == 0 || monLevelH == 0) continue;

                var mons = DataHelper.ParseNonEmptyHashSet(
                    new string[]
                    {
                        data[i, "mon1"],
                        data[i, "mon2"],
                        data[i, "mon3"],
                        data[i, "mon4"],
                        data[i, "mon5"],
                        data[i, "mon6"],
                        data[i, "mon7"],
                        data[i, "mon8"],
                        data[i, "mon9"],
                        data[i, "mon10"],
                    });
                var monsN = DataHelper.ParseNonEmptyHashSet(new string[]
                    {
                        data[i, "nmon1"],
                        data[i, "nmon2"],
                        data[i, "nmon3"],
                        data[i, "nmon4"],
                        data[i, "nmon5"],
                        data[i, "nmon6"],
                        data[i, "nmon7"],
                        data[i, "nmon8"],
                        data[i, "nmon9"],
                        data[i, "nmon10"],
                    });
                var monsH = DataHelper.ParseNonEmptyHashSet(new string[]
                    {
                        data[i, "umon1"],
                        data[i, "umon2"],
                        data[i, "umon3"],
                        data[i, "umon4"],
                        data[i, "umon5"],
                        data[i, "umon6"],
                        data[i, "umon7"],
                        data[i, "umon8"],
                        data[i, "umon9"],
                        data[i, "umon10"],
                    });

                // Skip no monster levels
                bool skip = true;
                foreach (var mon in mons)
                {
                    if (MonStats.Instance[mon] != null)
                    {
                        skip = false;
                        break;
                    }
                }
                if (skip) continue;
                foreach (var mon in monsN)
                {
                    if (MonStats.Instance[mon] != null)
                    {
                        skip = false;
                        break;
                    }
                }
                if (skip) continue;
                foreach (var mon in monsH)
                {
                    if (MonStats.Instance[mon] != null)
                    {
                        skip = false;
                        break;
                    }
                }
                if (skip) continue;

                Level Level = new Level
                (
                    name,
                    data[i, "LevelName"],
                    monLevel, monLevelN, monLevelH,
                    mons,
                    monsN,
                    monsH
                );
                _levels.Add(name, Level);
            }
            LogHelper.Log(log, $"Parsed {Count} levels.");
        }
    }

    /// <summary>
    /// summary
    /// </summary>
    public class Level
    {
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public Dictionary<Difficulty, int> MonsterLevel { get; private set; }
        public Dictionary<Difficulty, HashSet<string>> Monsters { get; private set; }

        private Level() { }

        public Level(string name, string displayName, int monLevel, int monLevelN, int monLevelH,
            HashSet<string> monsters, HashSet<string> monstersN, HashSet<string> monstersH)
        {
            Name = name;
            DisplayName = displayName;
            MonsterLevel = new Dictionary<Difficulty, int>()
            {
                { Difficulty.Normal, monLevel },
                { Difficulty.Nightmare, monLevelN },
                { Difficulty.Hell, monLevelH },
            };
            Monsters = new Dictionary<Difficulty, HashSet<string>>()
            {
                { Difficulty.Normal, monsters },
                { Difficulty.Nightmare, monstersN },
                { Difficulty.Hell, monstersH },
            };
        }
    }
}
