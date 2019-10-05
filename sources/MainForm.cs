using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        public MainForm()
        {
            InitializeComponent();

            listViewActors.ListViewItemSorter = actorListSorter;

            GameData_OnScannerStateChanged(GameData.ScannerState.MissingProcess);
            gameData.OnScannerStateChanged += GameData_OnScannerStateChanged;
            gameData.OnActorListChanged += GameData_OnActorListChanged;

            overlay.gameData = gameData;
            overlay.Show();
        }

        private void GameData_OnActorListChanged()
        {
            listViewActors.SuspendLayout();

            // remove missing
            for (int Idx = listViewActors.Items.Count - 1; Idx >= 0; Idx--)
            {
                GameData.ActorItem tagActor = (GameData.ActorItem)listViewActors.Items[Idx].Tag;
                if (!gameData.listActors.Contains(tagActor))
                {
                    listViewActors.Items.RemoveAt(Idx);
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
                        ListViewItem lvi = new ListViewItem(actor.ShowName);
                        lvi.Tag = actor;
                        lvi.SubItems.Add(actor.ShowType);
                        lvi.SubItems.Add(actor.ShowId);
                        lvi.SubItems.Add(actor.ShowDistance);

                        listViewActors.Items.Add(lvi);

                        // apply overlay settings - TODO: filters
                        actor.OverlaySettings.Mode = GameData.OverlaySettings.LabelMode.WhenClose;
                        actor.OverlaySettings.Description = actor.ShowName;
                        actor.OverlaySettings.DrawPen = Pens.Red;
                    }
                }
            }

            listViewActors.ResumeLayout();
            listViewActors.Sort();
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
            }
            else
            {
                int a = 1;
                a++;
            }
        }
    }
}
