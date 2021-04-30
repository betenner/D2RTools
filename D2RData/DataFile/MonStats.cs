using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// MonStats.txt controller.
    /// </summary>
    public class MonStats
    {
        private static MonStats _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static MonStats Instance
        {
            get
            {
                if (_instance == null) _instance = new MonStats();
                return _instance;
            }
        }

        private ListDict<string, Monster> _monsters = null;
        
        /// <summary>
        /// Unique monsters
        /// </summary>
        public ListDict<string, Monster> UniqueMonsters { get; private set; }

        /// <summary>
        /// Super uniques
        /// </summary>
        public ListDict<string, Monster> SuperUniques { get; private set; }

        /// <summary>
        /// Act bosses
        /// </summary>
        public ListDict<string, Monster> ActBosses { get; private set; }
        
        /// <summary>
        /// Gets specified Monster.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public Monster this[string id]
        {
            get
            {
                if (_monsters == null) return null;
                if (_monsters.TryGetValue(id, out Monster item)) return item;
                return null;
            }
        }

        /// <summary>
        /// Gets specified Monster.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns></returns>
        public Monster this[int index]
        {
            get
            {
                if (_monsters == null) return null;
                return _monsters.At(index);
            }
        }

        /// <summary>
        /// Gets total monster count.
        /// </summary>
        public int Count
        {
            get
            {
                return _monsters == null ? 0 : _monsters.Count;
            }
        }

        /// <summary>
        /// Parse data file MonStats.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseDataFile(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Parsing MonStats ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.MonStats];
            if (data == null)
            {
                LogHelper.Log(log, "Load MonStats.txt first!", LogLevel.Error);
                return;
            }

            _monsters = new ListDict<string, Monster>(data.RowCount);
            UniqueMonsters = new ListDict<string, Monster>();
            SuperUniques = new ListDict<string, Monster>();
            ActBosses = new ListDict<string, Monster>();
            for (int i = 0; i < data.RowCount; i++)
            {
                var id = data[i, "Id"];
                if (string.IsNullOrEmpty(id))
                {
                    LogHelper.Log(log, $"Null or empty id for MonStats row {i}, skipped.", LogLevel.Warning);
                    continue;
                }
                if (_monsters.ContainsKey(id))
                {
                    LogHelper.Log(log, $"Id [{id}] already exists, MonStats row {i} skipped.", LogLevel.Warning);
                    continue;
                }
                var enabled = DataHelper.ParseBool(data[i, "enabled"]);
                var isSpawn = DataHelper.ParseBool(data[i, "isSpawn"]);
                var isNpc = DataHelper.ParseBool(data[i, "npc"]);
                var isBoss = DataHelper.ParseBool(data[i, "boss"]);
                var isActBoss = DataHelper.ParseBool(data[i, "primeevil"]);
                var killable = DataHelper.ParseBool(data[i, "killable"]);
                var unique = DataHelper.ParseBool(data[i, "SetBoss"]);
                string tc = data[i, "TreasureClass1"];
                string tcC = data[i, "TreasureClass2"];
                string tcU = data[i, "TreasureClass3"];
                string tcN = data[i, "TreasureClass1(N)"];
                string tcCN = data[i, "TreasureClass2(N)"];
                string tcUN = data[i, "TreasureClass3(N)"];
                string tcH = data[i, "TreasureClass1(H)"];
                string tcCH = data[i, "TreasureClass2(H)"];
                string tcUH = data[i, "TreasureClass3(H)"];
                string tcQ = data[i, "TreasureClass4"];
                string tcQN = data[i, "TreasureClass4(N)"];
                string tcQH = data[i, "TreasureClass4(H)"];
                int level = DataHelper.ParseInt(data[i, "Level"]);
                int levelN = DataHelper.ParseInt(data[i, "Level(N)"]);
                int levelH = DataHelper.ParseInt(data[i, "Level(H)"]);

                // Skip disabled, unkillable, npc and non-spawnable monsters
                if (!enabled || isNpc || !killable || (!isSpawn && !isBoss && !isActBoss)) continue;

                // Skip no drop monsters
                if (string.IsNullOrEmpty(tc + tcC + tcU)) continue;

                // Skip no level monsters
                if (level + levelN + levelH == 0) continue;

                Monster monster = new Monster
                (
                    id,
                    data[i, "NameStr"],
                    unique,
                    isBoss,
                    isActBoss,
                    tc,
                    tcN,
                    tcH,
                    tcC,
                    tcCN,
                    tcCH,
                    tcU,
                    tcUN,
                    tcUH,
                    tcQ,
                    tcQN,
                    tcQH,
                    level,
                    levelN,
                    levelH
                );
                _monsters.Add(id, monster);
                if (unique) UniqueMonsters.Add(id, monster);
                if (DataFile.SuperUniques.Instance.GetSuperUniquesByClass(id) != null) SuperUniques.Add(id, monster);
                if (isActBoss) ActBosses.Add(id, monster);
            }
            LogHelper.Log(log, $"Parsed {_monsters.Count} monster(s) including: {UniqueMonsters.Count} unique monster(s), {SuperUniques.Count} super unique(s), {ActBosses.Count} act boss(es)");
        }
    }

    /// <summary>
    /// summary
    /// </summary>
    public class Monster
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public bool CanBeUnique { get; private set; }
        public bool IsBoss { get; private set; }
        public bool IsActBoss { get; private set; }
        public Dictionary<Difficulty, string> TreasureClassNormal { get; private set; }
        public Dictionary<Difficulty, string> TreasureClassChampion { get; private set; }
        public Dictionary<Difficulty, string> TreasureClassUnique { get; private set; }
        public Dictionary<Difficulty, string> TreasureClassQuest { get; private set; }
        public Dictionary<Difficulty, int> Level { get; private set; }

        private Monster() { }

        public Monster(string id, string name, bool unique, bool boss, bool actBoss,
            string tc, string tcN, string tcH, 
            string tcC, string tcCN, string tcCH,
            string tcU, string tcUN, string tcUH,
            string tcQ, string tcQN, string tcQH,
            int level, int levelN, int levelH)
        {
            Id = id;
            Name = name;
            CanBeUnique = unique;
            IsBoss = boss;
            IsActBoss = actBoss;
            TreasureClassNormal = new Dictionary<Difficulty, string>()
            {
                { Difficulty.Normal, tc },
                { Difficulty.Nightmare, tcN },
                { Difficulty.Hell, tcH },
            };
            TreasureClassChampion = new Dictionary<Difficulty, string>()
            {
                { Difficulty.Normal, tcC },
                { Difficulty.Nightmare, tcCN },
                { Difficulty.Hell, tcCH },
            };
            TreasureClassUnique = new Dictionary<Difficulty, string>()
            {
                { Difficulty.Normal, tcU },
                { Difficulty.Nightmare, tcUN },
                { Difficulty.Hell, tcUH },
            };
            TreasureClassQuest = new Dictionary<Difficulty, string>()
            {
                { Difficulty.Normal, tcQ },
                { Difficulty.Nightmare, tcQN },
                { Difficulty.Hell, tcQH },
            };
            Level = new Dictionary<Difficulty, int>()
            {
                { Difficulty.Normal, level },
                { Difficulty.Nightmare, levelN },
                { Difficulty.Hell, levelH },
            };
        }
    }
}
