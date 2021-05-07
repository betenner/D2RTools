using D2Data.DataFile;
using System;
using System.Collections.Generic;
using System.IO;

namespace D2Data
{
    /// <summary>
    /// Data controller for all game data.
    /// </summary>
    public class DataController
    {
        /// <summary>
        /// Singleton of the data controller.<br />
        /// Call <c>Init</c> before use this.
        /// </summary>
        public static DataController Instance { get; private set; }

        private DataController() { }

        private Dictionary<DataFileEnum, TxtDataSheet> _dataFiles = null;
        private bool _expInit = false;
        private bool _dataInit = false;

        /// <summary>
        /// Initialize data controller.
        /// </summary>
        /// <param name="dataFolder">Folder of data files</param>
        /// <param name="log">Log callback</param>
        public static void Init(string dataFolder, Action<string, LogLevel> log = null)
        {
            Instance ??= new DataController();
            Instance.InitDataFiles(dataFolder, log);
        }

        /// <summary>
        /// Initialize experience only.
        /// </summary>
        /// <param name="dataFolder">Folder of data files</param>
        /// <param name="log">Log callback</param>
        public static void InitExp(string dataFolder, Action<string, LogLevel> log = null)
        {
            Instance ??= new DataController();
            Instance.InitExpFile(dataFolder, log);
        }

        // Initialize experience only.
        private void InitExpFile(string dataFolder, Action<string, LogLevel> log = null)
        {
            if (_expInit) return;
            LogHelper.Log(log, "Begin loading experience file...");
            _dataFiles ??= new Dictionary<DataFileEnum, TxtDataSheet>();
            string filename = DataFileUtils.GetDataFilename(DataFileEnum.Experience);
            LogHelper.Log(log, $"Loading {filename} ...");
            _dataFiles.Add(DataFileEnum.Experience, new TxtDataSheet(Path.Combine(dataFolder, filename), true, true, true, true));
            LogHelper.Log(log, $"{filename} loaded.");
            ParseExperience(log);
            _expInit = true;
        }

        // Initialize all data files.
        private void InitDataFiles(string dataFolder, Action<string, LogLevel> log = null)
        {
            if (_dataInit) return;
            LogHelper.Log(log, "Begin loading data files...");
            _dataFiles ??= new Dictionary<DataFileEnum, TxtDataSheet>();
            foreach (DataFileEnum val in Enum.GetValues(typeof(DataFileEnum)))
            {
                if (val == DataFileEnum.Experience && _expInit) continue;
                string filename = DataFileUtils.GetDataFilename(val);
                LogHelper.Log(log, $"Loading {filename} ...");
                if (val == DataFileEnum.Experience) _dataFiles.Add(val, new TxtDataSheet(Path.Combine(dataFolder, filename), true, true, true, true));
                else _dataFiles.Add(val, new TxtDataSheet(Path.Combine(dataFolder, filename), true, false, true, true));
                LogHelper.Log(log, $"{filename} loaded.");
            }

            ParseItemTypes(log);
            ParseWeapons(log);
            ParseArmor(log);
            ParseMisc(log);
            ParseTreasureClasses(log);
            ParseSetItems(log);
            ParseUniqueItems(log);
            ParseSuperUniques(log);
            ParseMonStats(log);
            ParseLevels(log);
            if (!_expInit) ParseExperience(log);
            _expInit = true;
            _dataInit = true;

            //GenerateTCXRef(log);
        }

        /// <summary>
        /// Gets specified data file.
        /// </summary>
        /// <param name="dataFile">Data file</param>
        /// <returns></returns>
        public TxtDataSheet this[DataFileEnum dataFile]
        {
            get
            {
                if (_dataFiles == null) return null;
                if (_dataFiles.TryGetValue(dataFile, out TxtDataSheet txt)) return txt;
                return null;
            }
        }

        private void ParseItemTypes(Action<string, LogLevel> log = null)
        {
            ItemTypes.Instance.ParseDataFile(log);
        }

        private void ParseTreasureClasses(Action<string, LogLevel> log = null)
        {
            TreasureClass.Instance.ParseDataFile(log);
            TreasureClass.Instance.GenerateItemTypeTreasureClasses(log);
        }

        private void ParseWeapons(Action<string, LogLevel> log = null)
        {
            Items.Instance.ParseWeapons(log);
        }

        private void ParseArmor(Action<string, LogLevel> log = null)
        {
            Items.Instance.ParseArmors(log);
        }

        private void ParseMisc(Action<string, LogLevel> log = null)
        {
            Items.Instance.ParseMisc(log);
        }

        private void ParseSetItems(Action<string, LogLevel> log = null)
        {
            SetItems.Instance.ParseDataFile(log);
        }

        private void ParseUniqueItems(Action<string, LogLevel> log = null)
        {
            UniqueItems.Instance.ParseDataFile(log);
        }

        private void ParseMonStats(Action<string, LogLevel> log = null)
        {
            MonStats.Instance.ParseDataFile(log);
        }

        private void ParseLevels(Action<string, LogLevel> log = null)
        {
            Levels.Instance.ParseDataFile(log);
        }

        private void GenerateTCXRef(Action<string, LogLevel> log = null)
        {
            TreasureClass.Instance.GenerateXRef(log);
        }

        private void ParseSuperUniques(Action<string, LogLevel> log = null)
        {
            SuperUniques.Instance.ParseDataFile(log);
        }

        private void ParseExperience(Action<string, LogLevel> log = null)
        {
            Experience.Instance.ParseDataFile(log);
        }
    }
}
