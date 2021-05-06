
namespace D2Calc
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainTab = new System.Windows.Forms.TabControl();
            this.DropSimTab = new System.Windows.Forms.TabPage();
            this.DsList = new System.Windows.Forms.ListView();
            this.DschCount = new System.Windows.Forms.ColumnHeader();
            this.DschIlvl = new System.Windows.Forms.ColumnHeader();
            this.DschQuality = new System.Windows.Forms.ColumnHeader();
            this.DschItem = new System.Windows.Forms.ColumnHeader();
            this.DssoGroup = new System.Windows.Forms.GroupBox();
            this.DsProgress = new System.Windows.Forms.ProgressBar();
            this.DssoPartyPlayerCount = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.DssoTotalPlayerCount = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.DssoMagicFind = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.DssoDropTimes = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.DssoDropButton = new System.Windows.Forms.Button();
            this.DssoMonFirstKill = new System.Windows.Forms.CheckBox();
            this.DssoMonUnique = new System.Windows.Forms.RadioButton();
            this.DssoMonChampion = new System.Windows.Forms.RadioButton();
            this.DssoMonNormal = new System.Windows.Forms.RadioButton();
            this.DssoMonster = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DssoArea = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DssoDifficulty = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DssoType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveEditorTab = new System.Windows.Forms.TabPage();
            this.SfCharacter = new System.Windows.Forms.GroupBox();
            this.SfSaveFileBrowse = new System.Windows.Forms.Button();
            this.SfSaveFile = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.logConsole = new System.Windows.Forms.ListView();
            this.chTime = new System.Windows.Forms.ColumnHeader();
            this.chMessage = new System.Windows.Forms.ColumnHeader();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SfSave = new System.Windows.Forms.Button();
            this.MainTab.SuspendLayout();
            this.DropSimTab.SuspendLayout();
            this.DssoGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DssoPartyPlayerCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DssoTotalPlayerCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DssoMagicFind)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DssoDropTimes)).BeginInit();
            this.SaveEditorTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTab
            // 
            this.MainTab.Controls.Add(this.DropSimTab);
            this.MainTab.Controls.Add(this.SaveEditorTab);
            this.MainTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTab.Enabled = false;
            this.MainTab.Location = new System.Drawing.Point(0, 0);
            this.MainTab.Name = "MainTab";
            this.MainTab.SelectedIndex = 0;
            this.MainTab.Size = new System.Drawing.Size(1054, 673);
            this.MainTab.TabIndex = 0;
            // 
            // DropSimTab
            // 
            this.DropSimTab.Controls.Add(this.DsList);
            this.DropSimTab.Controls.Add(this.DssoGroup);
            this.DropSimTab.Location = new System.Drawing.Point(4, 26);
            this.DropSimTab.Name = "DropSimTab";
            this.DropSimTab.Padding = new System.Windows.Forms.Padding(3);
            this.DropSimTab.Size = new System.Drawing.Size(1046, 643);
            this.DropSimTab.TabIndex = 0;
            this.DropSimTab.Text = "Drop Simulation";
            this.DropSimTab.UseVisualStyleBackColor = true;
            // 
            // DsList
            // 
            this.DsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DsList.BackColor = System.Drawing.Color.Black;
            this.DsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DschCount,
            this.DschIlvl,
            this.DschQuality,
            this.DschItem});
            this.DsList.FullRowSelect = true;
            this.DsList.HideSelection = false;
            this.DsList.Location = new System.Drawing.Point(383, 10);
            this.DsList.MultiSelect = false;
            this.DsList.Name = "DsList";
            this.DsList.Size = new System.Drawing.Size(660, 466);
            this.DsList.TabIndex = 21;
            this.DsList.UseCompatibleStateImageBehavior = false;
            this.DsList.View = System.Windows.Forms.View.Details;
            this.DsList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.DsList_ColumnClick);
            // 
            // DschCount
            // 
            this.DschCount.Text = "Count";
            // 
            // DschIlvl
            // 
            this.DschIlvl.Text = "ilvl";
            // 
            // DschQuality
            // 
            this.DschQuality.Text = "Quality";
            this.DschQuality.Width = 80;
            // 
            // DschItem
            // 
            this.DschItem.Text = "Item";
            this.DschItem.Width = 300;
            // 
            // DssoGroup
            // 
            this.DssoGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.DssoGroup.Controls.Add(this.DsProgress);
            this.DssoGroup.Controls.Add(this.DssoPartyPlayerCount);
            this.DssoGroup.Controls.Add(this.label8);
            this.DssoGroup.Controls.Add(this.DssoTotalPlayerCount);
            this.DssoGroup.Controls.Add(this.label7);
            this.DssoGroup.Controls.Add(this.DssoMagicFind);
            this.DssoGroup.Controls.Add(this.label6);
            this.DssoGroup.Controls.Add(this.DssoDropTimes);
            this.DssoGroup.Controls.Add(this.label5);
            this.DssoGroup.Controls.Add(this.DssoDropButton);
            this.DssoGroup.Controls.Add(this.DssoMonFirstKill);
            this.DssoGroup.Controls.Add(this.DssoMonUnique);
            this.DssoGroup.Controls.Add(this.DssoMonChampion);
            this.DssoGroup.Controls.Add(this.DssoMonNormal);
            this.DssoGroup.Controls.Add(this.DssoMonster);
            this.DssoGroup.Controls.Add(this.label3);
            this.DssoGroup.Controls.Add(this.DssoArea);
            this.DssoGroup.Controls.Add(this.label2);
            this.DssoGroup.Controls.Add(this.DssoDifficulty);
            this.DssoGroup.Controls.Add(this.label4);
            this.DssoGroup.Controls.Add(this.DssoType);
            this.DssoGroup.Controls.Add(this.label1);
            this.DssoGroup.Location = new System.Drawing.Point(8, 3);
            this.DssoGroup.Name = "DssoGroup";
            this.DssoGroup.Size = new System.Drawing.Size(369, 473);
            this.DssoGroup.TabIndex = 0;
            this.DssoGroup.TabStop = false;
            this.DssoGroup.Text = "Source && Options";
            // 
            // DsProgress
            // 
            this.DsProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DsProgress.Location = new System.Drawing.Point(6, 444);
            this.DsProgress.Name = "DsProgress";
            this.DsProgress.Size = new System.Drawing.Size(357, 23);
            this.DsProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.DsProgress.TabIndex = 22;
            // 
            // DssoPartyPlayerCount
            // 
            this.DssoPartyPlayerCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DssoPartyPlayerCount.Location = new System.Drawing.Point(169, 326);
            this.DssoPartyPlayerCount.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.DssoPartyPlayerCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DssoPartyPlayerCount.Name = "DssoPartyPlayerCount";
            this.DssoPartyPlayerCount.Size = new System.Drawing.Size(96, 23);
            this.DssoPartyPlayerCount.TabIndex = 13;
            this.DssoPartyPlayerCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DssoPartyPlayerCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DssoPartyPlayerCount.Enter += new System.EventHandler(this.DssoNumericUpDown_Enter);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 328);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 17);
            this.label8.TabIndex = 12;
            this.label8.Text = "&Party Player Count:";
            // 
            // DssoTotalPlayerCount
            // 
            this.DssoTotalPlayerCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DssoTotalPlayerCount.Location = new System.Drawing.Point(169, 355);
            this.DssoTotalPlayerCount.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.DssoTotalPlayerCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DssoTotalPlayerCount.Name = "DssoTotalPlayerCount";
            this.DssoTotalPlayerCount.Size = new System.Drawing.Size(96, 23);
            this.DssoTotalPlayerCount.TabIndex = 15;
            this.DssoTotalPlayerCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DssoTotalPlayerCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DssoTotalPlayerCount.Enter += new System.EventHandler(this.DssoNumericUpDown_Enter);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 357);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "T&otal Player Count:";
            // 
            // DssoMagicFind
            // 
            this.DssoMagicFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DssoMagicFind.Location = new System.Drawing.Point(169, 384);
            this.DssoMagicFind.Maximum = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.DssoMagicFind.Name = "DssoMagicFind";
            this.DssoMagicFind.Size = new System.Drawing.Size(96, 23);
            this.DssoMagicFind.TabIndex = 17;
            this.DssoMagicFind.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DssoMagicFind.Enter += new System.EventHandler(this.DssoNumericUpDown_Enter);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 386);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 17);
            this.label6.TabIndex = 16;
            this.label6.Text = "&Magic Find: (%)";
            // 
            // DssoDropTimes
            // 
            this.DssoDropTimes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DssoDropTimes.Location = new System.Drawing.Point(169, 413);
            this.DssoDropTimes.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.DssoDropTimes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DssoDropTimes.Name = "DssoDropTimes";
            this.DssoDropTimes.Size = new System.Drawing.Size(96, 23);
            this.DssoDropTimes.TabIndex = 19;
            this.DssoDropTimes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DssoDropTimes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DssoDropTimes.Enter += new System.EventHandler(this.DssoNumericUpDown_Enter);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 415);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 17);
            this.label5.TabIndex = 18;
            this.label5.Text = "T&ime(s):";
            // 
            // DssoDropButton
            // 
            this.DssoDropButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DssoDropButton.Location = new System.Drawing.Point(271, 413);
            this.DssoDropButton.Name = "DssoDropButton";
            this.DssoDropButton.Size = new System.Drawing.Size(92, 25);
            this.DssoDropButton.TabIndex = 20;
            this.DssoDropButton.Text = "&Drop!";
            this.DssoDropButton.UseVisualStyleBackColor = true;
            this.DssoDropButton.Click += new System.EventHandler(this.DssoDropButton_Click);
            // 
            // DssoMonFirstKill
            // 
            this.DssoMonFirstKill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DssoMonFirstKill.AutoSize = true;
            this.DssoMonFirstKill.Location = new System.Drawing.Point(291, 146);
            this.DssoMonFirstKill.Name = "DssoMonFirstKill";
            this.DssoMonFirstKill.Size = new System.Drawing.Size(72, 21);
            this.DssoMonFirstKill.TabIndex = 11;
            this.DssoMonFirstKill.Text = "First Kill";
            this.DssoMonFirstKill.UseVisualStyleBackColor = true;
            // 
            // DssoMonUnique
            // 
            this.DssoMonUnique.AutoSize = true;
            this.DssoMonUnique.Location = new System.Drawing.Point(174, 146);
            this.DssoMonUnique.Name = "DssoMonUnique";
            this.DssoMonUnique.Size = new System.Drawing.Size(67, 21);
            this.DssoMonUnique.TabIndex = 10;
            this.DssoMonUnique.Text = "Unique";
            this.DssoMonUnique.UseVisualStyleBackColor = true;
            // 
            // DssoMonChampion
            // 
            this.DssoMonChampion.AutoSize = true;
            this.DssoMonChampion.Location = new System.Drawing.Point(83, 146);
            this.DssoMonChampion.Name = "DssoMonChampion";
            this.DssoMonChampion.Size = new System.Drawing.Size(85, 21);
            this.DssoMonChampion.TabIndex = 9;
            this.DssoMonChampion.Text = "Champion";
            this.DssoMonChampion.UseVisualStyleBackColor = true;
            // 
            // DssoMonNormal
            // 
            this.DssoMonNormal.AutoSize = true;
            this.DssoMonNormal.Checked = true;
            this.DssoMonNormal.Location = new System.Drawing.Point(7, 146);
            this.DssoMonNormal.Name = "DssoMonNormal";
            this.DssoMonNormal.Size = new System.Drawing.Size(70, 21);
            this.DssoMonNormal.TabIndex = 8;
            this.DssoMonNormal.TabStop = true;
            this.DssoMonNormal.Text = "Normal";
            this.DssoMonNormal.UseVisualStyleBackColor = true;
            // 
            // DssoMonster
            // 
            this.DssoMonster.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DssoMonster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DssoMonster.FormattingEnabled = true;
            this.DssoMonster.Location = new System.Drawing.Point(100, 115);
            this.DssoMonster.Name = "DssoMonster";
            this.DssoMonster.Size = new System.Drawing.Size(263, 25);
            this.DssoMonster.TabIndex = 7;
            this.DssoMonster.SelectedIndexChanged += new System.EventHandler(this.DssoMonster_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "&Monster:";
            // 
            // DssoArea
            // 
            this.DssoArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DssoArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DssoArea.FormattingEnabled = true;
            this.DssoArea.Location = new System.Drawing.Point(100, 84);
            this.DssoArea.Name = "DssoArea";
            this.DssoArea.Size = new System.Drawing.Size(263, 25);
            this.DssoArea.TabIndex = 5;
            this.DssoArea.SelectedIndexChanged += new System.EventHandler(this.DssoArea_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "&Area:";
            // 
            // DssoDifficulty
            // 
            this.DssoDifficulty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DssoDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DssoDifficulty.FormattingEnabled = true;
            this.DssoDifficulty.Location = new System.Drawing.Point(100, 53);
            this.DssoDifficulty.Name = "DssoDifficulty";
            this.DssoDifficulty.Size = new System.Drawing.Size(263, 25);
            this.DssoDifficulty.TabIndex = 3;
            this.DssoDifficulty.SelectedIndexChanged += new System.EventHandler(this.DssoDifficulty_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "&Difficulty:";
            // 
            // DssoType
            // 
            this.DssoType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DssoType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DssoType.FormattingEnabled = true;
            this.DssoType.Location = new System.Drawing.Point(100, 22);
            this.DssoType.Name = "DssoType";
            this.DssoType.Size = new System.Drawing.Size(263, 25);
            this.DssoType.TabIndex = 1;
            this.DssoType.SelectedIndexChanged += new System.EventHandler(this.DssoType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Type:";
            // 
            // SaveEditorTab
            // 
            this.SaveEditorTab.Controls.Add(this.SfSave);
            this.SaveEditorTab.Controls.Add(this.SfCharacter);
            this.SaveEditorTab.Controls.Add(this.SfSaveFileBrowse);
            this.SaveEditorTab.Controls.Add(this.SfSaveFile);
            this.SaveEditorTab.Controls.Add(this.label9);
            this.SaveEditorTab.Location = new System.Drawing.Point(4, 26);
            this.SaveEditorTab.Name = "SaveEditorTab";
            this.SaveEditorTab.Size = new System.Drawing.Size(1046, 643);
            this.SaveEditorTab.TabIndex = 1;
            this.SaveEditorTab.Text = "Save Editor";
            this.SaveEditorTab.UseVisualStyleBackColor = true;
            // 
            // SfCharacter
            // 
            this.SfCharacter.Enabled = false;
            this.SfCharacter.Location = new System.Drawing.Point(8, 38);
            this.SfCharacter.Name = "SfCharacter";
            this.SfCharacter.Size = new System.Drawing.Size(1030, 230);
            this.SfCharacter.TabIndex = 3;
            this.SfCharacter.TabStop = false;
            this.SfCharacter.Text = "Character";
            // 
            // SfSaveFileBrowse
            // 
            this.SfSaveFileBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SfSaveFileBrowse.Location = new System.Drawing.Point(958, 6);
            this.SfSaveFileBrowse.Name = "SfSaveFileBrowse";
            this.SfSaveFileBrowse.Size = new System.Drawing.Size(85, 28);
            this.SfSaveFileBrowse.TabIndex = 2;
            this.SfSaveFileBrowse.Text = "&Browse...";
            this.SfSaveFileBrowse.UseVisualStyleBackColor = true;
            this.SfSaveFileBrowse.Click += new System.EventHandler(this.SfSaveFileBrowse_Click);
            // 
            // SfSaveFile
            // 
            this.SfSaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SfSaveFile.Location = new System.Drawing.Point(105, 9);
            this.SfSaveFile.Name = "SfSaveFile";
            this.SfSaveFile.ReadOnly = true;
            this.SfSaveFile.Size = new System.Drawing.Size(847, 23);
            this.SfSaveFile.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 17);
            this.label9.TabIndex = 0;
            this.label9.Text = "&Save File:";
            // 
            // logConsole
            // 
            this.logConsole.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTime,
            this.chMessage});
            this.logConsole.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logConsole.FullRowSelect = true;
            this.logConsole.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.logConsole.HideSelection = false;
            this.logConsole.Location = new System.Drawing.Point(0, 511);
            this.logConsole.Name = "logConsole";
            this.logConsole.Size = new System.Drawing.Size(1054, 162);
            this.logConsole.TabIndex = 22;
            this.logConsole.UseCompatibleStateImageBehavior = false;
            this.logConsole.View = System.Windows.Forms.View.Details;
            // 
            // chTime
            // 
            this.chTime.Text = "Time";
            this.chTime.Width = 180;
            // 
            // chMessage
            // 
            this.chMessage.Text = "Message";
            this.chMessage.Width = 600;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 508);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1054, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // SfSave
            // 
            this.SfSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SfSave.Location = new System.Drawing.Point(953, 448);
            this.SfSave.Name = "SfSave";
            this.SfSave.Size = new System.Drawing.Size(85, 28);
            this.SfSave.TabIndex = 0;
            this.SfSave.Text = "S&ave";
            this.SfSave.UseVisualStyleBackColor = true;
            this.SfSave.Click += new System.EventHandler(this.SfSave_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 673);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.logConsole);
            this.Controls.Add(this.MainTab);
            this.MinimumSize = new System.Drawing.Size(956, 560);
            this.Name = "MainForm";
            this.Text = "D2R Tools";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.MainTab.ResumeLayout(false);
            this.DropSimTab.ResumeLayout(false);
            this.DssoGroup.ResumeLayout(false);
            this.DssoGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DssoPartyPlayerCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DssoTotalPlayerCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DssoMagicFind)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DssoDropTimes)).EndInit();
            this.SaveEditorTab.ResumeLayout(false);
            this.SaveEditorTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTab;
        private System.Windows.Forms.TabPage DropSimTab;
        private System.Windows.Forms.ListView logConsole;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ColumnHeader chTime;
        private System.Windows.Forms.ColumnHeader chMessage;
        private System.Windows.Forms.GroupBox DssoGroup;
        private System.Windows.Forms.ComboBox DssoType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox DssoMonster;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox DssoArea;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox DssoDifficulty;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton DssoMonUnique;
        private System.Windows.Forms.RadioButton DssoMonChampion;
        private System.Windows.Forms.RadioButton DssoMonNormal;
        private System.Windows.Forms.CheckBox DssoMonFirstKill;
        private System.Windows.Forms.Button DssoDropButton;
        private System.Windows.Forms.NumericUpDown DssoDropTimes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown DssoPartyPlayerCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown DssoTotalPlayerCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown DssoMagicFind;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView DsList;
        private System.Windows.Forms.ColumnHeader DschCount;
        private System.Windows.Forms.ColumnHeader DschIlvl;
        private System.Windows.Forms.ColumnHeader DschQuality;
        private System.Windows.Forms.ColumnHeader DschItem;
        private System.Windows.Forms.ProgressBar DsProgress;
        private System.Windows.Forms.TabPage SaveEditorTab;
        private System.Windows.Forms.TextBox SfSaveFile;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox SfCharacter;
        private System.Windows.Forms.Button SfSaveFileBrowse;
        private System.Windows.Forms.Button SfSave;
    }
}

