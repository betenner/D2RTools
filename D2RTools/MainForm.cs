using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using D2Data;
using D2Data.DataFile;
//using D2S = D2SaveFile;
using D2S = D2SLib.Model.Save;
using D2SLib;
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

        private D2S.D2S _save = null;

        private bool _dsInit = false;

        private bool _seChanged = false;
        private CharClass _seCcls;
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
            try
            {
                _save = Core.ReadD2S(File.ReadAllBytes(file));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error open save file!\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SeTab.Enabled = true;
            SeSaveFix.Enabled = true;
            SeRefresh();
            SeSetChanged(false, true);
        }

        private int TryGetAttr(D2S.D2S save, CharAttr? attr, int defaultValue = 0)
        {
            if (save == null || attr == null) return defaultValue;
            var str = Helper.CharAttr2Str(attr.Value);
            if (save.Attributes.Stats.TryGetValue(str, out int value)) return value;
            return defaultValue;
        }

        private void SetAttr(D2S.D2S save, CharAttr attr, int value)
        {
            if (save == null) return;
            var str = Helper.CharAttr2Str(attr);
            if (save.Attributes.Stats.ContainsKey(str)) save.Attributes.Stats[str] = value;
            else save.Attributes.Stats.Add(str, value);
        }

        private void SeRefreshChar()
        {
            // Name
            SeCharName.Text = _save.Name;
            SeCharName.ReadOnly = false;
            SeToggleNameEditMode();

            // Class
            SeCharClass.Text = ((CharClass)_save.ClassId).ToString();
            _seCcls = Enum.Parse<CharClass>(SeCharClass.Text);

            // Level & Exp
            SeCharLevel.Value = _save.Level;
            SeUniformExpFromLevel();
            SeCharExp.Value = (uint)TryGetAttr(_save, CharAttr.Experience);

            // Stats
            SeCharStrength.Value = TryGetAttr(_save, CharAttr.Strength);
            SeCharDexterity.Value = TryGetAttr(_save, CharAttr.Dexterity);
            SeCharVitality.Value = TryGetAttr(_save, CharAttr.Vitality);
            SeCharEnergy.Value = TryGetAttr(_save, CharAttr.Energy);
            SeCharLife.Value = TryGetAttr(_save, CharAttr.Life);
            SeCharMaxLife.Value = TryGetAttr(_save, CharAttr.MaxLife);
            SeCharMana.Value = TryGetAttr(_save, CharAttr.Mana);
            SeCharMaxMana.Value = TryGetAttr(_save, CharAttr.MaxMana);
            SeCharStamina.Value = TryGetAttr(_save, CharAttr.Stamina);
            SeCharMaxStamina.Value = TryGetAttr(_save, CharAttr.MaxStamina);
            SeCharGold.Value = TryGetAttr(_save, CharAttr.Gold);
            SeCharStashGold.Value = TryGetAttr(_save, CharAttr.StashGold);
            SeCharStatsLeft.Value = TryGetAttr(_save, CharAttr.StatsLeft);
            SeCharSkillLeft.Value = TryGetAttr(_save, CharAttr.SkillLeft);
        }

        private void SeGenProgressionActs(Difficulty difficulty)
        {
            var selected = SeProgressionAct.SelectedText;
            SeProgressionAct.Items.Clear();
            if (difficulty == Difficulty.Normal) SeProgressionAct.Items.Add("(None)");
            SeProgressionAct.Items.Add("Act I");
            SeProgressionAct.Items.Add("Act II");
            SeProgressionAct.Items.Add("Act III");
            SeProgressionAct.Items.Add("Act IV");
            SeProgressionAct.Items.Add("Act V");
            int index = SeProgressionAct.FindString(selected);
            SeProgressionAct.SelectedIndex = Math.Max(0, index);
        }

        private void SeRefreshProgression()
        {
            _seRefreshing = true;

            int v = Math.Max(0, _save.Progression - 1);
            int diff = v / 5;
            int act = v % 5;
            SeProgressionDifficulty.SelectedIndex = diff;
            SeProgressionAct.SelectedIndex = act + (diff == 0 ? 1 : 0);

            _seRefreshing = false;
        }

        private void SeRefreshQuest()
        {
            _seRefreshing = true;

            // Quest
            D2S.QuestsDifficulty quests = null;
            if (SeQuestDifficulty.SelectedIndex < 0) SeQuestDifficulty.SelectedIndex = 0;
            switch (SeQuestDifficulty.SelectedIndex)
            {
                case 0:
                    quests = _save.Quests.Normal;
                    break;

                case 1:
                    quests = _save.Quests.Nightmare;
                    break;

                case 2:
                    quests = _save.Quests.Hell;
                    break;
            }
            if (quests != null)
            {
                // Act I
                SeQuestA1Q1.Checked = SeQuestGetComplete(quests.ActI.DenOfEvil);
                SeQuestA1Q2.Checked = SeQuestGetComplete(quests.ActI.SistersBurialGrounds);
                SeQuestA1Q3.Checked = SeQuestGetComplete(quests.ActI.TheSearchForCain);
                SeQuestA1Q4.Checked = SeQuestGetComplete(quests.ActI.TheForgottenTower);
                SeQuestA1Q5.Checked = SeQuestGetComplete(quests.ActI.ToolsOfTheTrade);
                SeQuestA1Q6.Checked = SeQuestGetComplete(quests.ActI.SistersToTheSlaughter);

                // Act II
                SeQuestA2Q1.Checked = SeQuestGetComplete(quests.ActII.RadamentsLair);
                SeQuestA2Q2.Checked = SeQuestGetComplete(quests.ActII.TheHoradricStaff);
                SeQuestA2Q3.Checked = SeQuestGetComplete(quests.ActII.TaintedSun);
                SeQuestA2Q4.Checked = SeQuestGetComplete(quests.ActII.ArcaneSanctuary);
                SeQuestA2Q5.Checked = SeQuestGetComplete(quests.ActII.TheSummoner);
                SeQuestA2Q6.Checked = SeQuestGetComplete(quests.ActII.TheSevenTombs);

                // Act III
                SeQuestA3Q1.Checked = SeQuestGetComplete(quests.ActIII.TheGoldenBird);
                SeQuestA3Q2.Checked = SeQuestGetComplete(quests.ActIII.BladeOfTheOldReligion);
                SeQuestA3Q3.Checked = SeQuestGetComplete(quests.ActIII.KhalimsWill);
                SeQuestA3Q4.Checked = SeQuestGetComplete(quests.ActIII.LamEsensTome);
                SeQuestA3Q5.Checked = SeQuestGetComplete(quests.ActIII.TheBlackenedTemple);
                SeQuestA3Q6.Checked = SeQuestGetComplete(quests.ActIII.TheGuardian);

                // Act IV
                SeQuestA4Q1.Checked = SeQuestGetComplete(quests.ActIV.TheFallenAngel);
                SeQuestA4Q2.Checked = SeQuestGetComplete(quests.ActIV.Hellforge);
                SeQuestA4Q3.Checked = SeQuestGetComplete(quests.ActIV.TerrorsEnd);

                // Act V
                SeQuestA5Q1.Checked = SeQuestGetComplete(quests.ActV.SiegeOnHarrogath);
                SeQuestA5Q2.Checked = SeQuestGetComplete(quests.ActV.RescueOnMountArreat);
                SeQuestA5Q3.Checked = SeQuestGetComplete(quests.ActV.PrisonOfIce);
                SeQuestA5Q4.Checked = SeQuestGetComplete(quests.ActV.BetrayalOfHarrogath);
                SeQuestA5Q5.Checked = SeQuestGetComplete(quests.ActV.RiteOfPassage);
                SeQuestA5Q6.Checked = SeQuestGetComplete(quests.ActV.EveOfDestruction);
            }

            _seRefreshing = false;
        }

        private void SeRefresh()
        {
            if (_save == null) return;

            SeRefreshChar();
            SeRefreshQuest();
            SeRefreshProgression();
        }

        private bool SeQuestGetComplete(D2S.Quest quest)
        {
            return quest.CompletedBefore || quest.CompletedNow || quest.RewardGranted;
        }

        private void SeQuestSetComplete(D2S.Quest quest, bool complete)
        {
            quest.RewardGranted = complete;
            quest.CompletedBefore = complete;
            quest.CompletedNow = false;
            quest.Started = !complete;
        }

        private void SeUpdateQuests()
        {
            if (_seRefreshing) return;

            D2S.QuestsDifficulty quests = null;
            if (SeQuestDifficulty.SelectedIndex < 0) SeQuestDifficulty.SelectedIndex = 0;
            switch (SeQuestDifficulty.SelectedIndex)
            {
                case 0:
                    quests = _save.Quests.Normal;
                    break;

                case 1:
                    quests = _save.Quests.Nightmare;
                    break;

                case 2:
                    quests = _save.Quests.Hell;
                    break;
            }
            if (quests != null)
            {
                // Act I
                SeQuestSetComplete(quests.ActI.DenOfEvil, SeQuestA1Q1.Checked);
                SeQuestSetComplete(quests.ActI.SistersBurialGrounds, SeQuestA1Q2.Checked);
                SeQuestSetComplete(quests.ActI.TheSearchForCain, SeQuestA1Q3.Checked);
                SeQuestSetComplete(quests.ActI.TheForgottenTower, SeQuestA1Q4.Checked);
                SeQuestSetComplete(quests.ActI.ToolsOfTheTrade, SeQuestA1Q5.Checked);
                SeQuestSetComplete(quests.ActI.SistersToTheSlaughter, SeQuestA1Q6.Checked);

                // Act II
                SeQuestSetComplete(quests.ActII.RadamentsLair, SeQuestA2Q1.Checked);
                SeQuestSetComplete(quests.ActII.TheHoradricStaff, SeQuestA2Q2.Checked);
                SeQuestSetComplete(quests.ActII.TaintedSun, SeQuestA2Q3.Checked);
                SeQuestSetComplete(quests.ActII.ArcaneSanctuary, SeQuestA2Q4.Checked);
                SeQuestSetComplete(quests.ActII.TheSummoner, SeQuestA2Q5.Checked);
                SeQuestSetComplete(quests.ActII.TheSevenTombs, SeQuestA2Q6.Checked);

                // Act III
                SeQuestSetComplete(quests.ActIII.TheGoldenBird, SeQuestA3Q1.Checked);
                SeQuestSetComplete(quests.ActIII.BladeOfTheOldReligion, SeQuestA3Q2.Checked);
                SeQuestSetComplete(quests.ActIII.KhalimsWill, SeQuestA3Q3.Checked);
                SeQuestSetComplete(quests.ActIII.LamEsensTome, SeQuestA3Q4.Checked);
                SeQuestSetComplete(quests.ActIII.TheBlackenedTemple, SeQuestA3Q5.Checked);
                SeQuestSetComplete(quests.ActIII.TheGuardian, SeQuestA3Q6.Checked);

                // Act IV
                SeQuestSetComplete(quests.ActIV.TheFallenAngel, SeQuestA4Q1.Checked);
                SeQuestSetComplete(quests.ActIV.Hellforge, SeQuestA4Q2.Checked);
                SeQuestSetComplete(quests.ActIV.TerrorsEnd, SeQuestA4Q3.Checked);

                // Act V
                SeQuestSetComplete(quests.ActV.SiegeOnHarrogath, SeQuestA5Q1.Checked);
                SeQuestSetComplete(quests.ActV.RescueOnMountArreat, SeQuestA5Q2.Checked);
                SeQuestSetComplete(quests.ActV.PrisonOfIce, SeQuestA5Q3.Checked);
                SeQuestSetComplete(quests.ActV.BetrayalOfHarrogath, SeQuestA5Q4.Checked);
                SeQuestSetComplete(quests.ActV.RiteOfPassage, SeQuestA5Q5.Checked);
                SeQuestSetComplete(quests.ActV.EveOfDestruction, SeQuestA5Q6.Checked);
            }
        }

        private void SeUniformExpFromLevel()
        {
            int level = (int)SeCharLevel.Value;
            SeSetChanged(level != _save.Level);
            long exp = (long)SeCharExp.Value;
            _seNextExp = Experience.Instance.GetExp(_seCcls, level);
            _sePrevExp = level <= 1 ? 0L : Experience.Instance.GetExp(_seCcls, level - 1);
            if (exp < _sePrevExp) exp = _sePrevExp;
            else if (exp >= _seNextExp) exp = _seNextExp - 1L;
            SeCharExp.Value = exp;
            SeUniformCharExpBar();
        }

        private void SeUniformLevelFromExp()
        {
            long exp = (long)SeCharExp.Value;
            SeSetChanged(exp != (uint)TryGetAttr(_save, CharAttr.Experience));
            int lv;
            for (lv = 1; lv <= Experience.Instance.GetMaxLevel(_seCcls); lv++)
            {
                if (Experience.Instance.GetExp(_seCcls, lv) > exp) break;
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
                SeSetChanged(_save.Name != SeCharName.Text);
            }
        }

        private void SeCharStrength_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.Strength, sender);
        }

        private void SeSetValue(CharAttr attr, object sender)
        {
            if (_seRefreshing) return;
            if (!(sender is NumericUpDown nud)) return;
            SeSetChanged(nud.Value != TryGetAttr(_save, attr));
        }

        private void SeCharDexterity_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.Dexterity, sender);
        }

        private void SeCharVitality_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.Vitality, sender);
        }

        private void SeCharEnergy_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.Energy, sender);
        }

        private void SeCharLife_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.Life, sender);
        }

        private void SeCharNumericUpDown_Enter(object sender, EventArgs e)
        {
            (sender as NumericUpDown).Select(0, 10);
        }

        private void SeCharMaxLife_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.MaxLife, sender);
        }

        private void SeCharMana_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.MaxLife, sender);
        }

        private void SeCharMaxMana_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.MaxMana, sender);
        }

        private void SeCharStamina_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.Stamina, sender);
        }

        private void SeCharMaxStamina_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.MaxStamina, sender);
        }

        private void SeCharGold_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.Gold, sender);
        }

        private void SeCharStashGold_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.StashGold, sender);
        }

        private void SeCharStatsLeft_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.StatsLeft, sender);
        }

        private void SeCharSkillLeft_ValueChanged(object sender, EventArgs e)
        {
            SeSetValue(CharAttr.SkillLeft, sender);
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
            SeSaveStat(CharAttr.Level, SeCharLevel);
            SeSaveStat(CharAttr.Experience, SeCharExp);

            // Stats
            SeSaveStat(CharAttr.Strength, SeCharStrength);
            SeSaveStat(CharAttr.Dexterity, SeCharDexterity);
            SeSaveStat(CharAttr.Vitality, SeCharVitality);
            SeSaveStat(CharAttr.Energy, SeCharEnergy);
            SeSaveStat(CharAttr.Life, SeCharLife);
            SeSaveStat(CharAttr.MaxLife, SeCharMaxLife);
            SeSaveStat(CharAttr.Mana, SeCharMana);
            SeSaveStat(CharAttr.MaxMana, SeCharMaxMana);
            SeSaveStat(CharAttr.Stamina, SeCharStamina);
            SeSaveStat(CharAttr.MaxStamina, SeCharMaxStamina);
            SeSaveStat(CharAttr.Gold, SeCharGold);
            SeSaveStat(CharAttr.StashGold, SeCharStashGold);
            SeSaveStat(CharAttr.StatsLeft, SeCharStatsLeft);
            SeSaveStat(CharAttr.SkillLeft, SeCharSkillLeft);

            // Save
            File.WriteAllBytes(SeSaveFile.Text, Core.WriteD2S(_save));
            SeSetChanged(false, true);
        }

        private void SeDrFilter_CheckedChanged(object sender, EventArgs e)
        {
            ShowDropResults();
        }

        private void SeQuestDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            SeRefreshQuest();
        }

        private void SeProgressionDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            SeGenProgressionActs((Difficulty)SeProgressionDifficulty.SelectedIndex);
        }

        private void SeQuest_CheckedChanged(object sender, EventArgs e)
        {
            SeUpdateQuests();
            SeSetChanged(true, true);
        }

        private void SeProgressionAct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_seRefreshing) return;
            int diff = SeProgressionDifficulty.SelectedIndex;
            int act = SeProgressionAct.SelectedIndex;
            _save.Progression = (byte)(diff * 5 + (diff == 0 ? act : act + 1));
            SeSetChanged(true, true);
            System.Diagnostics.Debug.WriteLine(_save.Progression);
        }

        private void SeSaveStat(CharAttr stat, NumericUpDown nud)
        {
            Helper.Assert(nud.Value != TryGetAttr(_save, stat), () =>
            {
                SetAttr(_save, stat, (int)nud.Value);
            });
        }
    }
}
