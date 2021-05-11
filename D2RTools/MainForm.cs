using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using D2Data;
using D2Data.DataFile;
using D2S = D2SaveFile;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace D2RTools
{
    public partial class MainForm : Form
    {
        private const string REG_SAVED_GAMES_KEY = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders";
        private const string REG_SAVED_GAMES_VALUE = "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}";
        private const string D2R_SAVED_GAMES_FOLDER = "Diablo II Resurrected Tech Alpha";
        private static readonly Regex REG_CHAR_NAME = new Regex("^[^-_]?[a-zA-Z]+[-_]?[a-zA-Z]+[^-_]?$", RegexOptions.Compiled);
        private const string SE_TITLE_CHANGED = "Save Editor *";
        private const string SE_TITLE_UNCHANGED = "Save Editor";
        private const float IN_GAME_VALUE_DIVISOR = 256f;

        private const int MAX_PLAYERS = 8;

        private const string DATA_FOLDER = "data";

        // Log time format.
        private const string LOG_TIME_FORMAT = "{0:yyyy-MM-dd HH:mm:ss.fff}";

        // Log entry colors
        private static readonly Dictionary<LogLevel, Color> _logColors = new Dictionary<LogLevel, Color>()
        {
            { LogLevel.Log, Color.Black },
            { LogLevel.Warning, Color.Brown },
            { LogLevel.Error, Color.Red },
        };

        private bool _dsOptionNoRefresh = false;
        private List<Monster> _monList = null;
        private List<Level> _areaList = null;
        private List<TreasureClass.DropResult> _drList = null;
        private Dictionary<long, TreasureClass.DropResult> _drResults = null;

        private D2S.D2SaveFile _save = null;

        private bool _dsInit = false;

        private bool _seChanged = false;
        private PlayerClass _sePc;
        private long _sePrevExp, _seNextExp;
        private bool _seRefreshing;

        private enum DropResultSortingColumn
        {
            Default,
            Count,
            Quality,
            Item,
        }

        private Dictionary<DropResultSortingColumn, bool?> _drSortingColumnDesc =
            new Dictionary<DropResultSortingColumn, bool?>()
        {
            { DropResultSortingColumn.Count, null },
            { DropResultSortingColumn.Quality, null },
            { DropResultSortingColumn.Item, null },
        };
        private Comparison<TreasureClass.DropResult> _drSortingComparison;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitDataExp()
        {
            try
            {
                DataController.InitExp(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), DATA_FOLDER), LogCallback);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        private void InitDataFiles()
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
            _drResults ??= new Dictionary<long, TreasureClass.DropResult>(TreasureClass.TC_MAX_DROP * times);
            _drResults.Clear();
            _drResults.EnsureCapacity(TreasureClass.TC_MAX_DROP * times);

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
                    if (!_drResults.ContainsKey(hash))
                    {
                        _drResults.Add(hash, dr);
                    }
                    else
                    {
                        _drResults[hash].Count++;
                    }
                }
                DsProgress.Value = i + 1;
                DsProgress.Refresh();
            }

            // Show result
            ShowDropResults();

            MainTab.Enabled = true;
        }

        private void ShowDropResults()
        {
            if (_drResults == null || _drResults.Count == 0) return;

            var comparison = _drSortingComparison == null ? DropResultDefaultComparison : _drSortingComparison;

            _drList = new List<TreasureClass.DropResult>(_drResults.Values);
            _drList.Sort(comparison);

            DsList.Items.Clear();
            _drList.FindAll(x => SeDrFilter(x)).ForEach((dr) =>
            {
                ListViewItem lvi = new ListViewItem(dr.Count.ToString());
                lvi.SubItems.Add(dr.DropLevel.ToString());
                lvi.SubItems.Add(dr.BaseItem.IsMisc && dr.DropLevel == 0 ? "Misc" : dr.Quality.ToString());
                lvi.SubItems.Add($"{dr.Name}{dr.ParamText}");
                lvi.ForeColor = Utils.GetItemQualityColor(dr.Quality);
                DsList.Items.Add(lvi);
            });
        }

        private bool SeDrFilter(TreasureClass.DropResult dr)
        {
            if (dr.DropLevel == 0) return SeDrFilterMisc.Checked;

            switch (dr.Quality)
            {
                case ItemQuality.Unique:
                    return SeDrFilterUnique.Checked;

                case ItemQuality.Set:
                    return SeDrFilterSet.Checked;

                case ItemQuality.Rare:
                    return SeDrFilterRare.Checked;

                case ItemQuality.Magic:
                    return SeDrFilterMagic.Checked;

                case ItemQuality.Superior:
                    return SeDrFilterSuperior.Checked;

                case ItemQuality.Normal:
                    return SeDrFilterNormal.Checked;

                case ItemQuality.LowQuality:
                    return SeDrFilterLowQuality.Checked;
            }

            return true;
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

        #region Drop Result Comparisons

        private int DropResultDefaultComparison(TreasureClass.DropResult a, TreasureClass.DropResult b)
        {
            int c = b.Quality.CompareTo(a.Quality);
            if (c != 0) return c;
            c = a.BaseItem.IsMisc.CompareTo(b.BaseItem.IsMisc);
            if (c != 0) return c;
            if (!a.BaseItem.IsMisc || !b.BaseItem.IsMisc)
            {
                c = b.BaseItem.Level.CompareTo(a.BaseItem.Level);
                if (c != 0) return c;
                c = a.BaseItem.Rarity.CompareTo(b.BaseItem.Rarity);
                if (c != 0) return c;
            }
            return b.BaseItem.Index.CompareTo(a.BaseItem.Index);
        }

        private int DropResultCountComparisonDesc(TreasureClass.DropResult a, TreasureClass.DropResult b)
        {
            int c = b.Count.CompareTo(a.Count);
            if (c != 0) return c;
            return DropResultDefaultComparison(a, b);
        }

        private int DropResultCountComparisonAsc(TreasureClass.DropResult a, TreasureClass.DropResult b)
        {
            return -DropResultCountComparisonDesc(a, b);
        }

        private int DropResultQualityComparisonDesc(TreasureClass.DropResult a, TreasureClass.DropResult b)
        {
            int c = a.Quality.CompareTo(b.Quality);
            if (c != 0) return c;
            return DropResultDefaultComparison(a, b);
        }

        private int DropResultQualityComparisonAsc(TreasureClass.DropResult a, TreasureClass.DropResult b)
        {
            return -DropResultQualityComparisonDesc(a, b);
        }

        private int DropResultItemComparisonAsc(TreasureClass.DropResult a, TreasureClass.DropResult b)
        {
            int c = a.Name.CompareTo(b.Name);
            if (c != 0) return c;
            return DropResultDefaultComparison(a, b);
        }

        private int DropResultItemComparisonDesc(TreasureClass.DropResult a, TreasureClass.DropResult b)
        {
            return -DropResultItemComparisonAsc(a, b);
        }

        #endregion

        private void DsList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var col = e.Column switch
            {
                0 => DropResultSortingColumn.Count,
                2 => DropResultSortingColumn.Quality,
                3 => DropResultSortingColumn.Item,
                _ => DropResultSortingColumn.Default,
            };
            Comparison<TreasureClass.DropResult> comparison;

            if (col == DropResultSortingColumn.Default)
            {
                comparison = DropResultDefaultComparison;
            }
            else
            {
                var desc = _drSortingColumnDesc[col];
                if (desc.HasValue)
                {
                    if (desc.Value) desc = false;
                    else desc = null;
                }
                else desc = true;
                _drSortingColumnDesc[col] = desc;
                if (!desc.HasValue) col = DropResultSortingColumn.Default;

                switch (col)
                {
                    case DropResultSortingColumn.Count:
                        if (desc.Value) comparison = DropResultCountComparisonDesc;
                        else comparison = DropResultCountComparisonAsc;
                        break;

                    case DropResultSortingColumn.Item:
                        if (desc.Value) comparison = DropResultItemComparisonDesc;
                        else comparison = DropResultItemComparisonAsc;
                        break;

                    case DropResultSortingColumn.Quality:
                        if (desc.Value) comparison = DropResultQualityComparisonDesc;
                        else comparison = DropResultQualityComparisonAsc;
                        break;

                    default:
                        comparison = DropResultDefaultComparison;
                        break;
                }
            }

            _drSortingComparison = comparison;

            ShowDropResults();
        }

        private void SeSaveFileBrowse_Click(object sender, EventArgs e)
        {
            string folder;
            if (!string.IsNullOrEmpty(SeSaveFile.Text)) folder = Path.GetDirectoryName(SeSaveFile.Text);
            else folder = SeTryGetSavedGamesFolder();
            if (string.IsNullOrEmpty(folder)) folder = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            else
            {
                var path = Path.Combine(folder, D2R_SAVED_GAMES_FOLDER);
                if (Directory.Exists(path)) folder = path;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose a D2R save file";
            ofd.Filter = "D2R Save File (*.d2s)|*.d2s|All Files (*.*)|*.*";
            ofd.DefaultExt = ".d2s";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.InitialDirectory = folder;
            if (ofd.ShowDialog() == DialogResult.Cancel) return;
            SeSaveFile.Text = ofd.FileName;
            SeOpenSaveFile();
        }

        private string SeTryGetSavedGamesFolder()
        {
            var value = Registry.GetValue(REG_SAVED_GAMES_KEY, REG_SAVED_GAMES_VALUE, null);
            if (value != null) return value.ToString();
            return null;
        }

        private bool SeSetChanged(bool changed, bool force = false)
        {
            if (_save == null) return false;
            if (force) _seChanged = changed;
            else _seChanged |= changed;
            if (_seChanged && SaveEditorTab.Text != SE_TITLE_CHANGED) SaveEditorTab.Text = SE_TITLE_CHANGED;
            else if (!_seChanged && SaveEditorTab.Text != SE_TITLE_UNCHANGED) SaveEditorTab.Text = SE_TITLE_UNCHANGED;
            SeSave.Enabled = _seChanged;
            return changed;
        }

        private void SeOpenSaveFile()
        {
            InitDataExp();

            string file = SeSaveFile.Text;
            if (string.IsNullOrEmpty(file) || !File.Exists(file)) return;
            _save = new D2S.D2SaveFile(file)
            {
                SkipDataAfterStats = true
            };
            var result = _save.Load();
            if (result != D2S.FileValidity.Valid)
            {
                MessageBox.Show($"Error open save file!\n\n{result}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SeChar.Enabled = true;
            SeSaveFix.Enabled = true;
            SeRefreshCharacter();
            SeSetChanged(false, true);
        }

        private void SeRefreshCharacter()
        {
            if (_save == null) return;

            _seRefreshing = true;

            // Name
            SeCharName.Text = _save.CharacterData.CharacterName;
            SeCharName.ReadOnly = false;
            SeToggleNameEditMode();

            // Class
            SeCharClass.Text = _save.CharacterData.HeroClass.ToString();
            _sePc = Enum.Parse<PlayerClass>(SeCharClass.Text);

            // Level & Exp
            SeCharLevel.Value = _save.CharacterData.Level;
            SeCharExp.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Experience);

            // Stats
            SeCharStrength.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Strength);
            SeCharDexterity.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Dexterity);
            SeCharVitality.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Vitality);
            SeCharEnergy.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Energy);
            SeCharLife.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Life);
            SeCharMaxLife.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.MaxLife);
            SeCharMana.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Mana);
            SeCharMaxMana.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.MaxMana);
            SeCharStamina.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Stamina);
            SeCharMaxStamina.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.MaxStamina);
            SeCharGold.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.Gold);
            SeCharStashGold.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.StashGold);
            SeCharStatsLeft.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.StatsLeft);
            SeCharSkillLeft.Value = _save.Statistics.GetStatistic(D2S.CharacterStatistic.SkillsLeft);

            _seRefreshing = false;
        }

        private void SeUniformExpFromLevel()
        {
            int level = (int)SeCharLevel.Value;
            SeSetChanged(level != _save.CharacterData.Level);
            long exp = (long)SeCharExp.Value;
            _seNextExp = Experience.Instance.GetExp(_sePc, level);
            _sePrevExp = level <= 1 ? 0L : Experience.Instance.GetExp(_sePc, level - 1);
            if (exp < _sePrevExp) exp = _sePrevExp;
            else if (exp >= _seNextExp) exp = _seNextExp - 1L;
            SeCharExp.Value = exp;
            SeUniformCharExpBar();
        }

        private void SeUniformLevelFromExp()
        {
            long exp = (long)SeCharExp.Value;
            SeSetChanged(exp != _save.Statistics.GetStatistic(D2S.CharacterStatistic.Experience));
            int lv;
            for (lv = 1; lv <= Experience.Instance.GetMaxLevel(_sePc); lv++)
            {
                if (Experience.Instance.GetExp(_sePc, lv) > exp) break;
            }
            SeCharLevel.Value = lv;
            SeUniformCharExpBar();
        }

        private void SeSave_Click(object sender, EventArgs e)
        {
            if (_save == null) return;

            SeDoSave();
        }

        private void SeCharName_TextChanged(object sender, EventArgs e)
        {
            SeCheckCharName();
        }

        private void SeCheckCharName()
        {
            SeCharNameEdit.Enabled = REG_CHAR_NAME.IsMatch(SeCharName.Text);
        }

        private void SeCharNameEdit_Click(object sender, EventArgs e)
        {
            if (_save == null) return;
            SeToggleNameEditMode();
        }

        private void SeCharLevel_ValueChanged(object sender, EventArgs e)
        {
            SeUniformExpFromLevel();
        }

        private void SeCharExp_ValueChanged(object sender, EventArgs e)
        {
            SeUniformLevelFromExp();
        }

        private void SeUniformCharExpBar()
        {
            long exp = (long)SeCharExp.Value;
            float ratio = (float)(exp - _sePrevExp) / (_seNextExp - _sePrevExp);
            SeCharExpBar.Value = (int)(SeCharExpBar.Maximum * ratio);
        }

        private void SeCharExpBar_Scroll(object sender, ScrollEventArgs e)
        {
            float ratio = (float)SeCharExpBar.Value / SeCharExpBar.Maximum;
            long exp = (long)(ratio * (_seNextExp - _sePrevExp) + _sePrevExp);
            SeCharExp.Value = exp;
        }

        private void SeToggleNameEditMode()
        {
            if (SeCharName.ReadOnly)
            {
                SeCharNameEdit.Text = "Save";
                SeCharName.ReadOnly = false;
                SeCharName.Focus();
                SeCheckCharName();
            }
            else
            {
                SeCharNameEdit.Text = "Edit";
                SeCharName.ReadOnly = true;
                SeSetChanged(_save.CharacterData.CharacterName != SeCharName.Text);
            }
        }

        private void SeCharStrength_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.Strength, sender);
        }

        private void SeSetValue(D2S.CharacterStatistic stat, object sender)
        {
            if (_seRefreshing) return;
            if (!(sender is NumericUpDown nud)) return;
            SeSetChanged(nud.Value != _save.Statistics.GetStatistic(stat));
        }

        private void SeCharDexterity_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.Dexterity, sender);
        }

        private void SeCharVitality_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.Vitality, sender);
        }

        private void SeCharEnergy_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.Energy, sender);
        }

        private void SeCharLife_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.Life, sender);
            SeSetInGameValue(sender, SeCharLifeValue);
        }

        private void SeCharNumericUpDown_Enter(object sender, EventArgs e)
        {
            (sender as NumericUpDown).Select(0, 10);
        }

        private void SeCharMaxLife_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.MaxLife, sender);
            SeSetInGameValue(sender, SeCharMaxLifeValue);
        }

        private void SeCharMana_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.MaxLife, sender);
            SeSetInGameValue(sender, SeCharManaValue);
        }

        private void SeCharMaxMana_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.MaxMana, sender);
            SeSetInGameValue(sender, SeCharMaxManaValue);
        }

        private void SeCharStamina_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.Stamina, sender);
            SeSetInGameValue(sender, SeCharStaminaValue);
        }

        private void SeCharMaxStamina_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.MaxStamina, sender);
            SeSetInGameValue(sender, SeCharMaxStaminaValue);
        }

        private void SeCharGold_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.Gold, sender);
        }

        private void SeCharStashGold_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.StashGold, sender);
        }

        private void SeCharStatsLeft_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.StatsLeft, sender);
        }

        private void SeCharSkillLeft_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(D2S.CharacterStatistic.SkillsLeft, sender);
        }

        private void MainTab_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == DropSimTab)
            {
                InitDataFiles();
                if (!_dsInit)
                {
                    InitDropSimulationTab();
                    RefreshDSAreas();
                    _dsInit = true;
                }
            }
        }

        private void SeSetInGameValue(object sender, Label output)
        {
            if (!(sender is NumericUpDown nud)) return;
            output.Text = ((float)nud.Value / IN_GAME_VALUE_DIVISOR).ToString("#0.##");
        }

        private void SeSaveFix_Click(object sender, EventArgs e)
        {
            if (_save == null) return;

            if (!SeCheckUnsaved()) return;

            try
            {
                SeMakeBackup(SeSaveFile.Text);
                SaveFix.Fix(SeSaveFile.Text);
                MessageBox.Show("Save fixed for D2R!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                LogError($"Error occurred while fixing save for D2R: {ex.Message}");
            }
        }

        private void SeMakeBackup(string filepath)
        {
            try
            {
                var data = File.ReadAllBytes(filepath);
                var path = Path.GetDirectoryName(filepath);
                var fn = Path.GetFileName(filepath);
                string key = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var backupFile = Path.Combine(path, fn + "." + key + ".bak");
                if (File.Exists(backupFile)) File.Delete(backupFile);
                using FileStream fs = new FileStream(backupFile, FileMode.Create);
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            catch (Exception ex)
            {
                LogError($"Error occurred while making backup file: {ex.Message}");
            }
        }

        private bool SeCheckUnsaved()
        {
            if (_save == null) return true;
            if (_seChanged)
            {
                var dr = MessageBox.Show("There are unsaved change(s) to the current save file. Do you want to save now?", "Warning",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3);
                switch (dr)
                {
                    case DialogResult.Cancel: return false;
                    case DialogResult.Yes:
                        SeDoSave();
                        SeSetChanged(false, true);
                        break;
                }
            }
            return true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SeCheckUnsaved()) e.Cancel = true;
        }

        private void SeDoSave()
        {
            if (_save == null) return;

            SeMakeBackup(SeSaveFile.Text);

            // Level & Exp
            SeSaveStat(D2S.CharacterStatistic.Level, SeCharLevel);
            SeSaveStat(D2S.CharacterStatistic.Experience, SeCharExp);

            // Stats
            SeSaveStat(D2S.CharacterStatistic.Strength, SeCharStrength);
            SeSaveStat(D2S.CharacterStatistic.Dexterity, SeCharDexterity);
            SeSaveStat(D2S.CharacterStatistic.Vitality, SeCharVitality);
            SeSaveStat(D2S.CharacterStatistic.Energy, SeCharEnergy);
            SeSaveStat(D2S.CharacterStatistic.Life, SeCharLife);
            SeSaveStat(D2S.CharacterStatistic.MaxLife, SeCharMaxLife);
            SeSaveStat(D2S.CharacterStatistic.Mana, SeCharMana);
            SeSaveStat(D2S.CharacterStatistic.MaxMana, SeCharMaxMana);
            SeSaveStat(D2S.CharacterStatistic.Stamina, SeCharStamina);
            SeSaveStat(D2S.CharacterStatistic.MaxStamina, SeCharMaxStamina);
            SeSaveStat(D2S.CharacterStatistic.Gold, SeCharGold);
            SeSaveStat(D2S.CharacterStatistic.StashGold, SeCharStashGold);
            SeSaveStat(D2S.CharacterStatistic.StatsLeft, SeCharStatsLeft);
            SeSaveStat(D2S.CharacterStatistic.SkillsLeft, SeCharSkillLeft);

            // Save
            _save.Save();
            SeSetChanged(false, true);
        }

        private void SeUndoneActBossQuests_Click(object sender, EventArgs e)
        {
            if (_save == null) return;
            foreach (D2S.Difficulty diff in Enum.GetValues(typeof(D2S.Difficulty)))
            {
                _save.QuestData.ChangeQuest(diff, D2S.Act.Act1, D2S.Quest.Quest6, false);
                _save.QuestData.ChangeQuest(diff, D2S.Act.Act3, D2S.Quest.Quest6, false);
                _save.QuestData.ChangeQuest(diff, D2S.Act.Act4, D2S.Quest.Quest2, false);
                _save.QuestData.ChangeQuest(diff, D2S.Act.Act5, D2S.Quest.Quest6, false);
            }
            SeDoSave();
        }

        private void SeDrFilter_CheckedChanged(object sender, EventArgs e)
        {
            ShowDropResults();
        }

        private void SeSaveStat(D2S.CharacterStatistic stat, NumericUpDown nud)
        {
            Helper.Assert(nud.Value != _save.Statistics.GetStatistic(stat), () =>
            {
                _save.Statistics.SetStatistic(stat, (uint)nud.Value);
            });
        }
    }
}
