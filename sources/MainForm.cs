using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRadarBuddy
{
    public partial class MainForm : Form
    {
        private GameData gameData = new GameData();
        private OverlayForm overlay = new OverlayForm();
        private ListViewColumnSorter actorListSorter = new ListViewColumnSorter();
        private GameData.ActorItem selectedActor = null;
        private ActorFilterPreset activePreset = null;
        private bool ignorePresetSelection = false;

        public MainForm()
        {
            InitializeComponent();

            string orgTitle = Text;
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            string versionTitle = " [v" + version.Major + "]";
            Text = orgTitle + versionTitle;

            PlayerSettings.Get().Load();
            UpdatePresetList();
            RunUpdateCheck();

            checkFilterNearby.Checked = PlayerSettings.Get().FilterNearbyActors;
            splitContainer2.Panel1Collapsed = true;
            listViewActors.ListViewItemSorter = actorListSorter;

            GameData_OnScannerStateChanged(GameData.ScannerState.MissingProcess);
            gameData.OnScannerStateChanged += GameData_OnScannerStateChanged;
            gameData.OnActorListChanged += GameData_OnActorListChanged;

            overlay.gameData = gameData;
            overlay.Initialize();
            overlay.Show();
        }

        private void GameData_OnActorListChanged()
        {
            if (!checkBoxPauseScan.Checked)
            {
                RefreshActorList();
            }
        }

        private void RefreshActorList()
        { 
            listViewActors.SuspendLayout();
            listViewActors.BeginUpdate();

            // remove missing
            for (int Idx = listViewActors.Items.Count - 1; Idx >= 0; Idx--)
            {
                GameData.ActorItem tagActor = (GameData.ActorItem)listViewActors.Items[Idx].Tag;
                bool shouldRemove = !gameData.listActors.Contains(tagActor);
                if (checkFilterNearby.Checked && !tagActor.OverlaySettings.IsMatchingFilters)
                {
                    shouldRemove = true;
                }

                if (shouldRemove)
                {
                    listViewActors.Items.RemoveAt(Idx);
                }
                else
                {
                    listViewActors.Items[Idx].BackColor = tagActor.IsHidden ? Color.LightGray : SystemColors.Window;
                }
            }

            // add new
            if (listViewActors.Items.Count < gameData.listActors.Count)
            {
                List<GameData.ActorItem> knownActors = new List<GameData.ActorItem>();
                for (int Idx = 0; Idx < listViewActors.Items.Count; Idx++)
                {
                    GameData.ActorItem tagActor = (GameData.ActorItem)listViewActors.Items[Idx].Tag;
                    knownActors.Add(tagActor);
                }

                foreach (GameData.ActorItem actor in gameData.listActors)
                {
                    if (!knownActors.Contains(actor))
                    {
                        if (activePreset != null)
                        {
                            activePreset.Apply(actor);
                        }
                        else
                        {
                            actor.OverlaySettings.SetDefault(actor.ShowName);
                        }

                        bool canAdd = !checkFilterNearby.Checked || actor.OverlaySettings.IsMatchingFilters;
                        if (canAdd)
                        {
                            ListViewItem lvi = new ListViewItem(actor.ShowName);
                            lvi.Tag = actor;
                            lvi.SubItems.Add(actor.ShowType);
                            lvi.SubItems.Add(actor.ShowId);
                            lvi.SubItems.Add(actor.ShowDistance);
                            lvi.BackColor = actor.IsHidden ? Color.LightGray : SystemColors.Window;

                            listViewActors.Items.Add(lvi);
                        }
                    }
                }
            }

            listViewActors.ResumeLayout();
            listViewActors.Sort();
            listViewActors.EndUpdate();
        }

        private void UpdateShownDistance()
        {
            for (int Idx = 0; Idx < listViewActors.Items.Count; Idx++)
            {
                GameData.ActorItem tagActor = (GameData.ActorItem)listViewActors.Items[Idx].Tag;
                if (tagActor != null)
                {
                    listViewActors.Items[Idx].SubItems[3].Text = tagActor.ShowDistance;
                }
            }

            if (actorListSorter.SortColumn == columnHeaderDistance.Index)
            {
                listViewActors.Sort();
            }
        }

        private void UpdateOverlaySettings()
        {
            if (activePreset != null)
            {
                foreach (GameData.ActorItem actor in gameData.listActors)
                {
                    activePreset.Apply(actor);
                }
            }
            else
            {
                foreach (GameData.ActorItem actor in gameData.listActors)
                {
                    actor.OverlaySettings.SetDefault(actor.ShowName);
                }
            }
        }

        private void GameData_OnScannerStateChanged(GameData.ScannerState newState)
        {
            switch (newState)
            {
                case GameData.ScannerState.Ready:
                    panelScanState.BackColor = Color.FromArgb(0xff, 0x95, 0xfa, 0x87);
                    labelScanState.Text = "Status: Ready";
                    break;

                case GameData.ScannerState.MissingProcess:
                    panelScanState.BackColor = Color.FromArgb(0xff, 0xfa, 0x87, 0x95);
                    labelScanState.Text = "Status: Can't find game process!";
                    break;

                case GameData.ScannerState.MissingMemPaths:
                    panelScanState.BackColor = Color.FromArgb(0xff, 0xfa, 0x87, 0x95);
                    labelScanState.Text = "Status: Can't find data in memory!";
                    break;
            }

            overlay.SetScanActive(newState == GameData.ScannerState.Ready);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            overlay.Close();
            PlayerSettings.Get().Save();
        }

        private void listViewActors_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == actorListSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (actorListSorter.Order == SortOrder.Ascending)
                {
                    actorListSorter.Order = SortOrder.Descending;
                }
                else
                {
                    actorListSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                actorListSorter.SortColumn = e.Column;
                actorListSorter.Order = SortOrder.Ascending;
            }

            actorListSorter.Mode = (e.Column == columnHeaderDistance.Index) ? ListSortMode.Number : ListSortMode.String;

            // Perform the sort with these new sort options.
            listViewActors.Sort();
        }

        private void timerScan_Tick(object sender, EventArgs e)
        {
            gameData.Tick();
            overlay.Tick();
            UpdateShownDistance();
        }

        private void listViewActors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedActor != null)
            {
                selectedActor.OverlaySettings.IsHighlighted = false;
                selectedActor = null;
            }

            if (listViewActors.SelectedItems.Count == 1)
            {
                selectedActor = (GameData.ActorItem)listViewActors.SelectedItems[0].Tag;
                selectedActor.OverlaySettings.IsHighlighted = true;

                labelSelectedName.Text = selectedActor.Name;
                labelSelectedPos.Text = selectedActor.ShowPos + ", dist: " + selectedActor.ShowDistance;
                labelSelectedNpcID.Text = "0x" + selectedActor.NpcId.ToString("X");
                labelSelectedIDActorA.Text = "0x" + selectedActor.ActorIdA.ToString("X");
                labelSelectedIDActorB.Text = "0x" + selectedActor.ActorIdB.ToString("X");
                labelSelectedFlags.Text = "0x" + selectedActor.Flags.ToString("X2");
                labelSelectedType.Text = selectedActor.Type.ToString();
                labelSelectedSubType.Text = selectedActor.SubType.ToString();
                labelSelectedFlagsHidden.Text = selectedActor.IsHidden ? "(hidden)" : "";
            }
            else
            {
                labelSelectedName.Text = "";
                labelSelectedPos.Text = "";
                labelSelectedNpcID.Text = "";
                labelSelectedIDActorA.Text = "";
                labelSelectedIDActorB.Text = "";
                labelSelectedFlags.Text = "";
                labelSelectedType.Text = "";
                labelSelectedSubType.Text = "";
                labelSelectedFlagsHidden.Text = "";
            }
        }

        private void checkBoxPauseScan_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxPauseScan.Checked = !checkBoxPauseScan.Checked;
            if (!checkBoxPauseScan.Checked)
            {
                GameData_OnActorListChanged();
            }
        }

        private void checkFilterNearby_CheckedChanged(object sender, EventArgs e)
        {
            PlayerSettings.Get().FilterNearbyActors = checkFilterNearby.Checked;
            RefreshActorList();
        }

        private void RunUpdateCheck()
        {
            Task updateProgramTask = new Task(() => {
                bool bFoundUpdate = GithubUpdater.FindAndDownloadUpdates(out string statusMsg);

                Invoke((MethodInvoker)delegate
                {
                    Logger.WriteLine("Version check: " + statusMsg);
                    labelUpdateNotify.Visible = bFoundUpdate;
                    labelUpdateNotify.BringToFront();
                });
            });

            Task updatePresetsTask = new Task(() => {
                List<string> onlinePresets = GithubUpdaterPresets.FindAndDownloadPresets(out string statusMsg);

                Invoke((MethodInvoker)delegate
                {
                    Logger.WriteLine("Preset sync: " + statusMsg);
                    MergeOnlinePresets(onlinePresets);

                    buttonPresetOptions.Text = "Manage";
                    buttonPresetOptions.Enabled = true;
                });
            });

            buttonPresetOptions.Text = "Syncing...";
            buttonPresetOptions.Enabled = false;

            updateProgramTask.Start();
            updatePresetsTask.Start();
        }

        private void labelUpdateNotify_Click(object sender, EventArgs e)
        {
            labelUpdateNotify.Hide();
        }

        #region Preset: list

        private void UpdatePresetList()
        {
            comboBoxPreset.Items.Clear();
            listViewPresetManage.Items.Clear();

            PlayerSettings settings = PlayerSettings.Get();
            foreach (ActorFilterPreset preset in settings.Presets)
            {
                comboBoxPreset.Items.Add(preset);

                ListViewItem lvi = new ListViewItem(preset.Name);
                lvi.Tag = preset;
                listViewPresetManage.Items.Add(lvi);
            }

            if (comboBoxPreset.Items.Count > 0)
            {
                comboBoxPreset.SelectedIndex = 0;
            }
        }

        private void MergeOnlinePresets(List<string> onlinePresets)
        {
            PlayerSettings settings = PlayerSettings.Get();
            bool hasChanges = false;

            for (int importIdx = 0; importIdx < onlinePresets.Count; importIdx++)
            {
                ActorFilterPreset testPreset = new ActorFilterPreset();
                bool isValid = false;
                try
                {
                    JsonParser.ObjectValue rootOb = JsonParser.ParseJson(onlinePresets[importIdx]);
                    isValid = testPreset.LoadFromJson(rootOb);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Failed to merge synced preset #" + importIdx + ": " + ex);
                }

                if (isValid)
                {
                    ActorFilterPreset existingPreset = settings.Presets.Find(x => x.Name == testPreset.Name);
                    if (existingPreset != null && existingPreset.version >= testPreset.version)
                    {
                        Logger.WriteLine("Ignoring online preset #" + importIdx + ": " + testPreset.Name + " (v" + testPreset.version + ") => local version: " + existingPreset.version);
                        continue;
                    }

                    if (existingPreset != null)
                    {
                        settings.Presets.Remove(existingPreset);
                    }

                    Logger.WriteLine("Using online preset #" + importIdx + ": " + testPreset.Name + " (v" + testPreset.version + ")");
                    settings.Presets.Add(testPreset);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                UpdatePresetList();
            }
        }

        private void RefreshPresetDropdown()
        {
            int selected = comboBoxPreset.SelectedIndex;
            ignorePresetSelection = true;

            comboBoxPreset.Items.Clear();

            PlayerSettings settings = PlayerSettings.Get();
            foreach (ActorFilterPreset preset in settings.Presets)
            {
                comboBoxPreset.Items.Add(preset);
            }

            comboBoxPreset.SelectedIndex = selected;
            ignorePresetSelection = false;
        }

        private void comboBoxPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ignorePresetSelection)
            {
                activePreset = comboBoxPreset.SelectedItem as ActorFilterPreset;
                UpdatePresetFilters();
            }
        }

        private void buttonPresetOptions_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel1Collapsed = !splitContainer2.Panel1Collapsed;
            buttonPresetOptions.Text = splitContainer2.Panel1Collapsed ? "Manage..." : "Hide";
        }

        private void buttonAddPreset_Click(object sender, EventArgs e)
        {
            PlayerSettings settings = PlayerSettings.Get();

            string UniqueName = "";
            for (int Idx = 1; Idx < 10000; Idx++)
            {
                UniqueName = "Preset#" + Idx;
                bool alreadyExists = settings.Presets.Find(x => (x.Name == UniqueName)) != null;
                if (!alreadyExists)
                {
                    break;
                }
            }

            ActorFilterPreset preset = new ActorFilterPreset
            {
                Name = UniqueName,
                ShowOnlyMatching = false
            };
            settings.Presets.Add(preset);

            ListViewItem lvi = new ListViewItem(preset.Name);
            lvi.Tag = preset;
            listViewPresetManage.Items.Add(lvi);

            int itemIdx = comboBoxPreset.Items.Add(preset);
            comboBoxPreset.SelectedIndex = itemIdx;
        }

        private void buttonImportPreset_Click(object sender, EventArgs e)
        {
            ActorFilterPreset loadedPreset = null;

            string importText = Clipboard.GetText();
            if (!string.IsNullOrEmpty(importText) && importText.Length > 2 && importText[0] == '{')
            {
                ActorFilterPreset preset = new ActorFilterPreset();
                try
                {
                    JsonParser.ObjectValue jsonOb = JsonParser.ParseJson(importText);
                    bool hasLoaded = preset.LoadFromJson(jsonOb);
                    if (hasLoaded)
                    {
                        loadedPreset = preset;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Import failed text:'" + importText + "', exception:" + ex);
                }
            }

            if (loadedPreset != null)
            {
                PlayerSettings settings = PlayerSettings.Get();
                ActorFilterPreset existingPreset = settings.Presets.Find(x => (x.Name == loadedPreset.Name));
                bool bCanAdd = false;

                if (existingPreset != null)
                {
                    DialogResult dr = MessageBox.Show("Preset '" + existingPreset.Name + "' already exists, do you want to merge it?", Text, MessageBoxButtons.YesNoCancel);
                    if (dr == DialogResult.Yes)
                    {
                        // merge
                        existingPreset.Filters.AddRange(loadedPreset.Filters);                      
                    }
                    else if (dr == DialogResult.No)
                    {
                        // keep separate, create new name

                        string UseName = existingPreset.Name;
                        int sepIdx = UseName.LastIndexOf('#');
                        if (sepIdx > 0)
                        {
                            UseName = UseName.Substring(0, sepIdx);
                        }

                        for (int Idx = 2; Idx < 10000; Idx++)
                        {
                            string UniqueName = UseName + "#" + Idx;
                            bool alreadyExists = settings.Presets.Find(x => (x.Name == UniqueName)) != null;
                            if (!alreadyExists)
                            {
                                loadedPreset.Name = UniqueName;
                                bCanAdd = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    bCanAdd = true;
                }

                if (bCanAdd)
                {
                    settings.Presets.Add(loadedPreset);

                    ListViewItem lvi = new ListViewItem(loadedPreset.Name);
                    lvi.Tag = loadedPreset;
                    listViewPresetManage.Items.Add(lvi);

                    int itemIdx = comboBoxPreset.Items.Add(loadedPreset);
                    comboBoxPreset.SelectedIndex = itemIdx;
                }
            }
        }

        private void listViewPresetManage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                toolStripMenuItemPresetDelete_Click(null, null);
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                toolStripMenuItemPresetExport_Click(null, null);
            }
            else if (e.KeyCode == Keys.V && e.Control)
            {
                buttonImportPreset_Click(null, null);
            }
        }

        private void toolStripMenuItemPresetExport_Click(object sender, EventArgs e)
        {
            if (listViewPresetManage.SelectedItems.Count == 1)
            {
                ActorFilterPreset preset = (ActorFilterPreset)listViewPresetManage.SelectedItems[0].Tag;

                JsonWriter writer = new JsonWriter();
                preset.SaveToJson(writer);

                Clipboard.SetText(writer.ToString());
            }
        }

        private void toolStripMenuItemPresetDelete_Click(object sender, EventArgs e)
        {
            if (listViewPresetManage.SelectedItems.Count == 1)
            {
                ActorFilterPreset preset = (ActorFilterPreset)listViewPresetManage.SelectedItems[0].Tag;

                PlayerSettings settings = PlayerSettings.Get();
                settings.Presets.Remove(preset);

                if (settings.Presets.Count == 0)
                {
                    settings.CreateDefaultPreset();
                }

                UpdatePresetList();
            }
        }

        private void contextMenuStripManagePresets_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = (listViewPresetManage.SelectedItems.Count == 0);

            if (listViewPresetManage.SelectedItems.Count > 0)
            {
                toolStripPresetListTextBox.Text = listViewPresetManage.SelectedItems[0].Text;
            }
        }

        private void listViewPresetManage_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Item >= 0 && !string.IsNullOrEmpty(e.Label))
            {
                PlayerSettings settings = PlayerSettings.Get();

                bool bAlreadyExisting = settings.Presets.Find(x => x.Name.Equals(e.Label, StringComparison.InvariantCultureIgnoreCase)) != null;
                if (!bAlreadyExisting)
                {
                    ActorFilterPreset preset = (ActorFilterPreset)listViewPresetManage.Items[e.Item].Tag;
                    preset.Name = e.Label;

                    RefreshPresetDropdown();
                }
                else
                {
                    e.CancelEdit = true;
                }
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        private void toolStripPresetListTextBox_TextChanged(object sender, EventArgs e)
        {
            if (listViewPresetManage.SelectedIndices.Count == 1)
            {
                LabelEditEventArgs changeArgs = new LabelEditEventArgs(listViewPresetManage.SelectedIndices[0], toolStripPresetListTextBox.Text);
                listViewPresetManage_AfterLabelEdit(null, changeArgs);

                if (!changeArgs.CancelEdit)
                {
                    listViewPresetManage.SelectedItems[0].Text = changeArgs.Label;
                }
            }
        }

        private void listViewPresetManage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPresetManage.SelectedIndices.Count == 1)
            {
                comboBoxPreset.SelectedIndex = listViewPresetManage.SelectedIndices[0];
            }
        }

        #endregion

        #region Preset: edit

        private void UpdatePresetFilters()
        {
            listViewPresetEdit.Items.Clear();

            if (activePreset != null)
            {
                foreach (ActorFilter filter in activePreset.Filters)
                {
                    ListViewItem lvi = new ListViewItem(filter.Description);
                    lvi.Tag = filter;
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    UpdateFilterItem(lvi);

                    listViewPresetEdit.Items.Add(lvi);
                }

                checkBoxShowOnlyMatching.Enabled = true;
                checkBoxShowOnlyMatching.Checked = activePreset.ShowOnlyMatching;
            }
            else
            {
                checkBoxShowOnlyMatching.Enabled = false;
            }

            labelFilterHint.Visible = (activePreset != null) && (activePreset.Filters.Count == 0);
            UpdateOverlaySettings();

            if (checkFilterNearby.Checked)
            {
                RefreshActorList();
            }
        }

        private void listViewActors_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && listViewActors.SelectedItems.Count == 1)
            {
                GameData.ActorItem tagActor = (GameData.ActorItem)listViewActors.SelectedItems[0].Tag;
                DoDragDrop(tagActor, DragDropEffects.Move);
            }
        }

        private void listViewPresetEdit_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(GameData.ActorItem)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void listViewPresetEdit_DragDrop(object sender, DragEventArgs e)
        {
            GameData.ActorItem actor = (GameData.ActorItem)e.Data.GetData(typeof(GameData.ActorItem));
            Logger.WriteLine("Filter edit, drop: " + actor);

            ActorFilter filterOb = new ActorFilter();
            filterOb.Description = actor.ShowName;
            filterOb.Pen = Pens.Gray;
            filterOb.Mode = GameData.OverlaySettings.DisplayMode.WhenClose;
            filterOb.MatchType = actor.Type;
            filterOb.MatchNpcId = actor.NpcId;
            filterOb.UseMatchType = true;
            filterOb.UseMatchNpcId = (actor.Type != MemoryLayout.ActorType.Player);

            CreateFilterItem(filterOb);
        }

        private void CreateFilterItem(ActorFilter filterOb)
        { 
            if (activePreset != null)
            {
                if (!filterOb.UseMatchNpcId)
                {
                    filterOb.Description = "";
                }

                activePreset.Filters.Add(filterOb);

                ListViewItem lvi = new ListViewItem(filterOb.Description);
                lvi.Tag = filterOb;
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                UpdateFilterItem(lvi);
                UpdateOverlaySettings();

                listViewPresetEdit.Items.Add(lvi);
            }

            labelFilterHint.Visible = (activePreset != null) && (activePreset.Filters.Count == 0);
        }

        private void UpdateFilterItem(ListViewItem lvi)
        {
            ActorFilter filterOb = (ActorFilter)lvi.Tag;
            lvi.SubItems[0].Text = filterOb.UseMatchNpcId ? filterOb.Description : "(multiple)";
            lvi.SubItems[1].Text = filterOb.MatchType.ToString();
            lvi.SubItems[2].Text = filterOb.UseMatchNpcId ? "set" : "";
            // 3: color

            switch (filterOb.Mode)
            {
                case GameData.OverlaySettings.DisplayMode.WhenClose: lvi.SubItems[4].Text = "Close"; break;
                case GameData.OverlaySettings.DisplayMode.WhenLookingAt: lvi.SubItems[4].Text = "Look at"; break;
                case GameData.OverlaySettings.DisplayMode.WhenCloseAndLookingAt: lvi.SubItems[4].Text = "Close & Look at"; break;
                default: lvi.SubItems[4].Text = filterOb.Mode.ToString(); break;
            }
        }

        private void checkBoxShowOnlyMatching_CheckedChanged(object sender, EventArgs e)
        {
            if (activePreset != null)
            {
                activePreset.ShowOnlyMatching = checkBoxShowOnlyMatching.Checked;
                UpdateOverlaySettings();
            }
        }

        private void listViewPresetEdit_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listViewPresetEdit_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // empty, handle in subitem draw
        }

        private void listViewPresetEdit_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex != 3)
            {
                e.DrawDefault = true;
                return;
            }

            bool isSelected = ((e.ItemState & ListViewItemStates.Selected) == ListViewItemStates.Selected);
            if (isSelected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }

            ActorFilter filterOb = (ActorFilter)e.Item.Tag;
            Rectangle rowBounds = e.SubItem.Bounds;
            int boxSize = rowBounds.Height - 4;
            Rectangle boxBounds = new Rectangle(rowBounds.Left + ((rowBounds.Width - boxSize) / 2), rowBounds.Top + 2, boxSize, boxSize);

            e.Graphics.FillRectangle(filterOb.Pen.Brush, boxBounds);
            e.Graphics.DrawRectangle(Pens.Black, boxBounds);
        }

        private void listViewPresetEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (activePreset != null && listViewPresetEdit.SelectedItems.Count == 1)
                {
                    ActorFilter filterTag = (ActorFilter)listViewPresetEdit.SelectedItems[0].Tag;
                    activePreset.Filters.Remove(filterTag);

                    UpdatePresetFilters();
                }
            }
        }

        private void contextMenuStripFilters_Opening(object sender, CancelEventArgs e)
        {
            bool bHasFilter = (activePreset != null) && (listViewPresetEdit.SelectedItems.Count == 1);
            foreach (ToolStripItem item in contextMenuStripFilters.Items)
            {
                item.Visible = bHasFilter;
            }

            toolStripMenuItemAddFilter.Visible = !bHasFilter;
        }

        private void contextMenuStripFilters_Opened(object sender, EventArgs e)
        {
            bool bHasFilter = (activePreset != null) && (listViewPresetEdit.SelectedItems.Count == 1);
            if (bHasFilter)
            {
                ActorFilter filterTag = (ActorFilter)listViewPresetEdit.SelectedItems[0].Tag;
                contextMenuStripFilters.Tag = filterTag;

                toolStripMenuItemOverrideName.Checked = filterTag.HasDescriptionOverride;
                toolStripMenuItemMatchType.Checked = filterTag.UseMatchType;
                toolStripMenuItemMatchId.Checked = filterTag.UseMatchNpcId;
                toolStripTextBoxNameOverrride.Text = string.IsNullOrEmpty(filterTag.Description) ? "??" : filterTag.Description;
                toolStripTextBoxMatchId.Text = filterTag.MatchNpcId.ToString();

                toolStripComboBoxLabelMode.Items.Clear();
                foreach (GameData.OverlaySettings.DisplayMode v in Enum.GetValues(typeof(GameData.OverlaySettings.DisplayMode)))
                {
                    toolStripComboBoxLabelMode.Items.Add(v);
                }
                toolStripComboBoxLabelMode.SelectedItem = filterTag.Mode;

                toolStripComboBoxMatchType.Items.Clear();
                foreach (MemoryLayout.ActorType v in Enum.GetValues(typeof(MemoryLayout.ActorType)))
                {
                    toolStripComboBoxMatchType.Items.Add(v);
                }
                toolStripComboBoxMatchType.SelectedItem = filterTag.MatchType;
            }
        }

        private void contextMenuStripFilters_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            ActorFilter filterTag = (ActorFilter)contextMenuStripFilters.Tag;
            if (filterTag != null)
            {
                e.Cancel = (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked);
                if (!e.Cancel)
                {
                    filterTag.HasDescriptionOverride = toolStripMenuItemOverrideName.Checked;
                    filterTag.UseMatchType = toolStripMenuItemMatchType.Checked;
                    filterTag.UseMatchNpcId = toolStripMenuItemMatchId.Checked;
                    filterTag.Description = toolStripTextBoxNameOverrride.Text;
                    uint.TryParse(toolStripTextBoxMatchId.Text, out filterTag.MatchNpcId);
                    filterTag.Mode = (GameData.OverlaySettings.DisplayMode)toolStripComboBoxLabelMode.SelectedItem;
                    filterTag.MatchType = (MemoryLayout.ActorType)toolStripComboBoxMatchType.SelectedItem;

                    foreach (ListViewItem lvi in listViewPresetEdit.Items)
                    {
                        if (lvi.Tag == filterTag)
                        {
                            UpdateFilterItem(lvi);
                            UpdateOverlaySettings();
                            break;
                        }
                    }
                }
            }
        }

        private void toolStripMenuItemSelectColor_Click(object sender, EventArgs e)
        {
            ActorFilter filterTag = (ActorFilter)contextMenuStripFilters.Tag;
            colorDialog1.Color = filterTag.Pen.Color;

            DialogResult dr = colorDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                filterTag.Pen = new Pen(colorDialog1.Color);
            }

            contextMenuStripFilters.Close();
        }

        private void toolStripMenuItemAddFilter_Click(object sender, EventArgs e)
        {
            ActorFilter filterOb = new ActorFilter();
            filterOb.Description = "??";
            filterOb.Pen = Pens.Gray;
            filterOb.Mode = GameData.OverlaySettings.DisplayMode.WhenClose;
            filterOb.MatchType = MemoryLayout.ActorType.Player;
            filterOb.MatchNpcId = 0;
            filterOb.UseMatchType = true;
            filterOb.UseMatchNpcId = false;

            CreateFilterItem(filterOb);
            contextMenuStripFilters.Close();
        }

        #endregion

    }
}
