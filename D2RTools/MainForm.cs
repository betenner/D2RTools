using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using D2Data;
using D2Data.DataFile;

namespace D2Calc
{
    public partial class MainForm : Form
    {
        private const int MAX_PLAYERS = 8;

        private const string DATA_FOLDER = "data";

        // Log time format.
        private const string LOG_TIME_FORMAT = "{0:yyyy-MM-dd HH:mm:ss.fff}";

        // Log entry colors
        private static Dictionary<LogLevel, Color> _logColors = new Dictionary<LogLevel, Color>()
        {
            { LogLevel.Log, Color.Black },
            { LogLevel.Warning, Color.Brown },
            { LogLevel.Error, Color.Red },
        };

        private bool _dsOptionNoRefresh = false;
        private List<Monster> _monList = null;
        private List<Level> _areaList = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitData()
        {
            try
            {
                DataController.Init(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), DATA_FOLDER), LogCallback);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        private void LogCallback(string message, LogLevel level)
        {
            Log(message, level);
            Application.DoEvents();
        }

        // Log a message.
        private void Log(string message, LogLevel level = LogLevel.Log)
        {
            if (!_logColors.TryGetValue(level, out Color clr)) clr = Color.Black;
            ListViewItem lvi = new ListViewItem(string.Format(LOG_TIME_FORMAT, DateTime.Now))
            {
                ForeColor = clr
            };
            lvi.SubItems.Add(message);
            logConsole.Items.Add(lvi);
            logConsole.Items[^1].EnsureVisible();
        }

        private void LogWarning(string message)
        {
            Log(message, LogLevel.Warning);
        }

        private void LogError(string message)
        {
            Log(message, LogLevel.Error);
        }

        private void InitDropSimulationTab()
        {
            _dsOptionNoRefresh = true;

            DssoType.Items.Clear();
            foreach (MonsterType monsterType in Enum.GetValues(typeof(MonsterType)))
            {
                DssoType.Items.Add(monsterType.ToString());
            }
            DssoType.SelectedIndex = 0;

            DssoDifficulty.Items.Clear();
            foreach (Difficulty difficulty in Enum.GetValues(typeof(Difficulty)))
            {
                DssoDifficulty.Items.Add(difficulty.ToString());
            }

            _dsOptionNoRefresh = false;
            DssoDifficulty.SelectedIndex = 0;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            InitData();
            InitDropSimulationTab();
            MainTab.Enabled = true;
            RefreshDSAreas();
        }

        private void DssoType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDSMonsters();
        }

        private void DssoDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDSAreas();
        }

        private void DssoArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDSMonsters();
        }

        private void RefreshDSAreas()
        {
            if (_dsOptionNoRefresh) return;

            _dsOptionNoRefresh = true;
            DssoArea.Items.Clear();
            _areaList = new List<Level>();

            var diff = (Difficulty)DssoDifficulty.SelectedIndex;
            for (int i = 0; i < Levels.Instance.Count; i++)
            {
                var level = Levels.Instance[i];
                var alvl = level.MonsterLevel[diff];
                DssoArea.Items.Add($"{level.DisplayName} [Lv.{alvl}]");
                _areaList.Add(level);
            }

            _dsOptionNoRefresh = false;
            DssoArea.SelectedIndex = 0;
        }

        private void RefreshDSMonsters()
        {
            if (_dsOptionNoRefresh) return;

            DssoMonster.Items.Clear();
            if (DssoArea.SelectedIndex < 0 || DssoArea.SelectedIndex >= Levels.Instance.Count) return;

            var type = (MonsterType)DssoType.SelectedIndex;
            var level = Levels.Instance[DssoArea.SelectedIndex];
            var difficulty = (Difficulty)DssoDifficulty.SelectedIndex;
            var monsters = level.Monsters[difficulty];

            switch (type)
            {
                case MonsterType.ActBoss:
                    SetDssoMonOption(false);
                    DssoMonFirstKill.Enabled = true;
                    _monList = new List<Monster>();
                    foreach (var ab in MonStats.Instance.ActBosses.ToList())
                    {
                        DssoMonster.Items.Add(ab.Name);
                        _monList.Add(ab);
                    }
                    break;

                case MonsterType.SuperUnique:
                    SetDssoMonOption(false);
                    DssoMonFirstKill.Enabled = false;
                    for (int i = 0; i < SuperUniques.Instance.Count; i++)
                    {
                        var sumon = SuperUniques.Instance[i];
                        DssoMonster.Items.Add(sumon.DisplayName);
                    }
                    break;

                default:
                    SetDssoMonOption(true);
                    DssoMonFirstKill.Enabled = false;
                    _monList = new List<Monster>();
                    foreach (var mon in monsters)
                    {
                        var monster = MonStats.Instance[mon];
                        if (monster != null)
                        {
                            DssoMonster.Items.Add(monster.Name);
                            _monList.Add(monster);
                        }
                    }
                    break;
            }

            if (DssoMonster.Items.Count > 0)
            {
                DssoMonster.SelectedIndex = 0;
                DssoMonster.Enabled = true;
            }
            else
            {
                DssoMonster.Items.Add("(No available monster)");
                DssoMonster.SelectedIndex = 0;
                DssoMonster.Enabled = false;
            }
        }

        private void SetDssoMonOption(bool enabled)
        {
            DssoArea.Enabled = enabled;
            DssoMonNormal.Enabled = DssoMonChampion.Enabled = DssoMonUnique.Enabled = enabled;
        }

        private void DssoDropButton_Click(object sender, EventArgs e)
        {
            // Parameters
            Difficulty diff = (Difficulty)DssoDifficulty.SelectedIndex;
            int dropLevel;
            string tc;
            switch ((MonsterType)DssoType.SelectedIndex)
            {
                case MonsterType.ActBoss:
                    var mon = _monList[DssoMonster.SelectedIndex];
                    dropLevel = mon.Level[diff];
                    tc = DssoMonFirstKill.Checked ? mon.TreasureClassUnique[diff] : mon.TreasureClassNormal[diff];
                    break;

                case MonsterType.SuperUnique:
                    var su = SuperUniques.Instance[DssoMonster.SelectedIndex];
                    dropLevel = su.Level[diff];
                    tc = su.TreasureClasses[diff];
                    break;

                default:
                    dropLevel = _areaList[DssoArea.SelectedIndex].MonsterLevel[diff];
                    var m = _monList[DssoMonster.SelectedIndex];
                    if (DssoMonUnique.Checked)
                    {
                        tc = m.TreasureClassUnique[diff];
                    }
                    else if (DssoMonChampion.Checked)
                    {
                        tc = m.TreasureClassChampion[diff];
                    }
                    else
                    {
                        tc = m.TreasureClassNormal[diff];
                    }
                    break;
            }

            SimulateDrop(tc, dropLevel, (int)DssoMagicFind.Value, (int)DssoPartyPlayerCount.Value, (int)DssoTotalPlayerCount.Value, (int)DssoDropTimes.Value);
        }

        private void DssoNumericUpDown_Enter(object sender, EventArgs e)
        {
            (sender as NumericUpDown).Select(0, 10);
        }

        private void SimulateDrop(string tcName, int dropLevel, int mf, int partyPlayerCount, int totalPlayerCount, int times)
        {
            Dictionary<long, TreasureClass.DropResult> hashMap = new Dictionary<long, TreasureClass.DropResult>();
            Dictionary<long, int> countMap = new Dictionary<long, int>();

            MainTab.Enabled = false;
            DsProgress.Value = 0;
            DsProgress.Maximum = times;
            DsList.Items.Clear();
            Application.DoEvents();

            // Start rolling
            for (int i = 0; i < times; i++)
            {
                var rolls = TreasureClass.Instance.Roll(tcName, dropLevel, mf, partyPlayerCount, totalPlayerCount, LogCallback);
                foreach (var dr in rolls)
                {
                    var hash = dr.Uid;
                    if (!hashMap.ContainsKey(hash))
                    {
                        hashMap.Add(hash, dr);
                        countMap.Add(hash, 1);
                    }
                    else
                    {
                        countMap[hash]++;
                    }
                }
                DsProgress.Value = i + 1;
                DsProgress.Refresh();
            }

            // Sorting
            List<TreasureClass.DropResult> drList = new List<TreasureClass.DropResult>(hashMap.Count);
            foreach (var dr in hashMap.Values)
            {
                drList.Add(dr);
            }
            drList.Sort(DropResultComarer);

            // Showing results
            for (int i = drList.Count - 1; i >= 0; i--)
            {
                var dr = drList[i];
                var count = countMap[dr.Uid];
                ListViewItem lvi = new ListViewItem(count.ToString());
                lvi.SubItems.Add(dr.DropLevel.ToString());
                lvi.SubItems.Add(dr.BaseItem.IsMisc && dr.DropLevel == 0 ? "Misc" : dr.Quality.ToString());
                lvi.SubItems.Add($"{dr.Name}{dr.ParamText}");
                lvi.ForeColor = Utils.GetItemQualityColor(dr.Quality);
                DsList.Items.Add(lvi);
            }

            MainTab.Enabled = true;
        }

        private int DropResultComarer(TreasureClass.DropResult a, TreasureClass.DropResult b)
        {
            int c = b.BaseItem.IsMisc.CompareTo(a.BaseItem.IsMisc);
            if (c != 0) return c;
            c = a.Quality.CompareTo(b.Quality);
            if (c != 0) return c;
            if (!a.BaseItem.IsMisc || !b.BaseItem.IsMisc)
            {
                c = a.BaseItem.Level.CompareTo(b.BaseItem.Level);
                if (c != 0) return c;
                c = b.BaseItem.Rarity.CompareTo(a.BaseItem.Rarity);
                if (c != 0) return c;
            }
            return a.BaseItem.Index.CompareTo(b.BaseItem.Index);
        }

        private void DssoMonster_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = (MonsterType)DssoType.SelectedIndex;
            if (type == MonsterType.SuperUnique)
            {
                var su = SuperUniques.Instance[DssoMonster.SelectedIndex];
                if (su != null)
                {
                    var level = Levels.Instance[su.Area];
                    if (level != null)
                    {
                        _dsOptionNoRefresh = true;
                        DssoArea.SelectedIndex = DssoArea.FindString(level.DisplayName);
                        _dsOptionNoRefresh = false;
                    }                
                }
            }
        }
    }
}
