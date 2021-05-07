using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data.DataFile
{
    /// <summary>
    /// Experience.txt controller.
    /// </summary>
    public class Experience
    {
        private const int EXP_RATIO_DIVISOR = 1024;
        private static Experience _instance = null;

        /// <summary>
        /// Singleton
        /// </summary>
        public static Experience Instance
        {
            get
            {
                if (_instance == null) _instance = new Experience();
                return _instance;
            }
        }

        private Dictionary<PlayerClass, List<long>> _expDict;
        private Dictionary<PlayerClass, int> _maxLevelDict;
        private Dictionary<PlayerClass, List<int>> _expRatioDict;

        /// <summary>
        /// Gets max level.
        /// </summary>
        /// <param name="pc">Player class</param>
        /// <returns></returns>
        public int GetMaxLevel(PlayerClass pc)
        {
            if (_maxLevelDict.TryGetValue(pc, out int lv)) return lv;
            return 0;
        }

        /// <summary>
        /// Gets level-up experience.
        /// </summary>
        /// <param name="pc">Player class</param>
        /// <param name="level">Level</param>
        /// <returns></returns>
        public long GetExp(PlayerClass pc, int level)
        {
            var maxLv = GetMaxLevel(pc);
            if (level < 1 || level > maxLv) return -1L;
            if (_expDict.TryGetValue(pc, out List<long> expList))
            {
                if (expList.Count >= level) return expList[level - 1];
            }
            return -1L;
        }

        /// <summary>
        /// Gets experience ratio.
        /// </summary>
        /// <param name="pc">Player class</param>
        /// <param name="level">Level</param>
        /// <returns></returns>
        public float GetExpRatio(PlayerClass pc, int level)
        {
            var maxLv = GetMaxLevel(pc);
            if (level < 1 || level > maxLv) return 0f;
            if (_expRatioDict.TryGetValue(pc, out List<int> ratioList))
            {
                if (ratioList.Count >= level) return (float)ratioList[level - 1] / EXP_RATIO_DIVISOR;
            }
            return 0f;
        }

        /// <summary>
        /// Parse data file Experience.txt
        /// </summary>
        /// <param name="log">Log callback</param>
        public void ParseDataFile(Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Parsing Experience ...");
            if (DataController.Instance == null)
            {
                LogHelper.Log(log, "Initialize DataController first!", LogLevel.Error);
                return;
            }
            var data = DataController.Instance[DataFileEnum.Experience];
            if (data == null)
            {
                LogHelper.Log(log, "Load Experience.txt first!", LogLevel.Error);
                return;
            }

            _maxLevelDict = new Dictionary<PlayerClass, int>();
            _expDict = new Dictionary<PlayerClass, List<long>>();
            _expRatioDict = new Dictionary<PlayerClass, List<int>>();
            foreach (PlayerClass pc in Enum.GetValues(typeof(PlayerClass)))
            {
                var maxLv = DataHelper.ParseInt(data["MaxLvl", pc.ToString()]);
                if (maxLv == 0)
                {
                    LogHelper.Log(log, $"No MaxLvl for class {pc}!", LogLevel.Error);
                    continue;
                }
                _maxLevelDict.Add(pc, maxLv);
                _expDict.Add(pc, new List<long>(maxLv));
                _expRatioDict.Add(pc, new List<int>(maxLv));
                for (int lv = 1; lv <= maxLv; lv++)
                {
                    var exp = DataHelper.ParseLong(data[lv.ToString(), pc.ToString()]);
                    _expDict[pc].Add(exp);
                    _expRatioDict[pc].Add(DataHelper.ParseInt(data[lv.ToString(), "ExpRatio"]));
                }
            }
        }
    }
}
