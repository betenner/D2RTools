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

        /// <summary>
        /// Initialize data controller.
        /// </summary>
        /// <param name="dataFolder">Folder of data files</param>
        /// <param name="log">Log callback</param>
        public static void Init(string dataFolder, Action<string, LogLevel> log = null)
        {
            Instance = new DataController();
            Instance.InitDataFiles(dataFolder, log);
        }

        // Initialize all data files.
        private void InitDataFiles(string dataFolder, Action<string, LogLevel> log = null)
        {
            LogHelper.Log(log, "Begin loading data files...");
            _dataFiles = new Dictionary<DataFileEnum, TxtDataSheet>();
            foreach (DataFileEnum val in Enum.GetValues(typeof(DataFileEnum)))
            {
                string filename = DataFileUtils.GetDataFilename(val);
                LogHelper.Log(log, $"Loading {filename} ...");
                _dataFiles.Add(val, new TxtDataSheet(Path.Combine(dataFolder, filename), true, false, true, true));
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

            GenerateTCXRef(log);
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
                TxtDataSheet txt;
                if (_dataFiles.TryGetValue(dataFile, out txt)) return txt;
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
    }
}
