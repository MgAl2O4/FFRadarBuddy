namespace FFRadarBuddy
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelScanState = new System.Windows.Forms.Panel();
            this.labelScanState = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonPresetOptions = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listViewPresetManage = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripManagePresets = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemPresetExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPresetDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonImportPreset = new System.Windows.Forms.Button();
            this.buttonAddPreset = new System.Windows.Forms.Button();
            this.checkBoxShowOnlyMatching = new System.Windows.Forms.CheckBox();
            this.listViewPresetEdit = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripFilters = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemOverrideName = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxNameOverrride = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemMatchType = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxMatchType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemMatchId = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxMatchId = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSelectColor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxLabelMode = new System.Windows.Forms.ToolStripComboBox();
            this.comboBoxPreset = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxPauseScan = new System.Windows.Forms.CheckBox();
            this.listViewActors = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDistance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.timerScan = new System.Windows.Forms.Timer(this.components);
            this.labelUpdateNotify = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.toolStripPresetListTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.labelFilterHint = new System.Windows.Forms.Label();
            this.toolStripMenuItemAddFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.panelScanState.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStripManagePresets.SuspendLayout();
            this.contextMenuStripFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelScanState
            // 
            this.panelScanState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelScanState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelScanState.Controls.Add(this.labelScanState);
            this.panelScanState.Location = new System.Drawing.Point(12, 12);
            this.panelScanState.Name = "panelScanState";
            this.panelScanState.Size = new System.Drawing.Size(793, 30);
            this.panelScanState.TabIndex = 0;
            // 
            // labelScanState
            // 
            this.labelScanState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelScanState.Location = new System.Drawing.Point(3, 0);
            this.labelScanState.Name = "labelScanState";
            this.labelScanState.Size = new System.Drawing.Size(785, 28);
            this.labelScanState.TabIndex = 0;
            this.labelScanState.Text = "State";
            this.labelScanState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 48);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonPresetOptions);
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxPreset);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkBoxPauseScan);
            this.splitContainer1.Panel2.Controls.Add(this.listViewActors);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(793, 429);
            this.splitContainer1.SplitterDistance = 342;
            this.splitContainer1.TabIndex = 1;
            // 
            // buttonPresetOptions
            // 
            this.buttonPresetOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPresetOptions.Location = new System.Drawing.Point(276, 2);
            this.buttonPresetOptions.Name = "buttonPresetOptions";
            this.buttonPresetOptions.Size = new System.Drawing.Size(63, 23);
            this.buttonPresetOptions.TabIndex = 4;
            this.buttonPresetOptions.Text = "Manage...";
            this.buttonPresetOptions.UseVisualStyleBackColor = true;
            this.buttonPresetOptions.Click += new System.EventHandler(this.buttonPresetOptions_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(0, 28);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listViewPresetManage);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.buttonImportPreset);
            this.splitContainer2.Panel1.Controls.Add(this.buttonAddPreset);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.labelFilterHint);
            this.splitContainer2.Panel2.Controls.Add(this.checkBoxShowOnlyMatching);
            this.splitContainer2.Panel2.Controls.Add(this.listViewPresetEdit);
            this.splitContainer2.Size = new System.Drawing.Size(342, 401);
            this.splitContainer2.SplitterDistance = 183;
            this.splitContainer2.TabIndex = 3;
            // 
            // listViewPresetManage
            // 
            this.listViewPresetManage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPresetManage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.listViewPresetManage.ContextMenuStrip = this.contextMenuStripManagePresets;
            this.listViewPresetManage.FullRowSelect = true;
            this.listViewPresetManage.LabelEdit = true;
            this.listViewPresetManage.Location = new System.Drawing.Point(0, 0);
            this.listViewPresetManage.Margin = new System.Windows.Forms.Padding(0);
            this.listViewPresetManage.MultiSelect = false;
            this.listViewPresetManage.Name = "listViewPresetManage";
            this.listViewPresetManage.Size = new System.Drawing.Size(342, 154);
            this.listViewPresetManage.TabIndex = 3;
            this.listViewPresetManage.UseCompatibleStateImageBehavior = false;
            this.listViewPresetManage.View = System.Windows.Forms.View.Details;
            this.listViewPresetManage.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewPresetManage_AfterLabelEdit);
            this.listViewPresetManage.SelectedIndexChanged += new System.EventHandler(this.listViewPresetManage_SelectedIndexChanged);
            this.listViewPresetManage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewPresetManage_KeyDown);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            this.columnHeader4.Width = 338;
            // 
            // contextMenuStripManagePresets
            // 
            this.contextMenuStripManagePresets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPresetListTextBox,
            this.toolStripMenuItemPresetExport,
            this.toolStripMenuItemPresetDelete});
            this.contextMenuStripManagePresets.Name = "contextMenuStripManagePresets";
            this.contextMenuStripManagePresets.Size = new System.Drawing.Size(175, 73);
            this.contextMenuStripManagePresets.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripManagePresets_Opening);
            // 
            // toolStripMenuItemPresetExport
            // 
            this.toolStripMenuItemPresetExport.Name = "toolStripMenuItemPresetExport";
            this.toolStripMenuItemPresetExport.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItemPresetExport.Text = "Export to clipboard";
            this.toolStripMenuItemPresetExport.Click += new System.EventHandler(this.toolStripMenuItemPresetExport_Click);
            // 
            // toolStripMenuItemPresetDelete
            // 
            this.toolStripMenuItemPresetDelete.Name = "toolStripMenuItemPresetDelete";
            this.toolStripMenuItemPresetDelete.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItemPresetDelete.Text = "Delete";
            this.toolStripMenuItemPresetDelete.Click += new System.EventHandler(this.toolStripMenuItemPresetDelete_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(364, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Right click preset for more options";
            // 
            // buttonImportPreset
            // 
            this.buttonImportPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonImportPreset.Location = new System.Drawing.Point(84, 157);
            this.buttonImportPreset.Name = "buttonImportPreset";
            this.buttonImportPreset.Size = new System.Drawing.Size(75, 23);
            this.buttonImportPreset.TabIndex = 1;
            this.buttonImportPreset.Text = "Import";
            this.buttonImportPreset.UseVisualStyleBackColor = true;
            this.buttonImportPreset.Click += new System.EventHandler(this.buttonImportPreset_Click);
            // 
            // buttonAddPreset
            // 
            this.buttonAddPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddPreset.Location = new System.Drawing.Point(3, 157);
            this.buttonAddPreset.Name = "buttonAddPreset";
            this.buttonAddPreset.Size = new System.Drawing.Size(75, 23);
            this.buttonAddPreset.TabIndex = 0;
            this.buttonAddPreset.Text = "Add new";
            this.buttonAddPreset.UseVisualStyleBackColor = true;
            this.buttonAddPreset.Click += new System.EventHandler(this.buttonAddPreset_Click);
            // 
            // checkBoxShowOnlyMatching
            // 
            this.checkBoxShowOnlyMatching.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxShowOnlyMatching.AutoSize = true;
            this.checkBoxShowOnlyMatching.Location = new System.Drawing.Point(7, 191);
            this.checkBoxShowOnlyMatching.Name = "checkBoxShowOnlyMatching";
            this.checkBoxShowOnlyMatching.Size = new System.Drawing.Size(121, 17);
            this.checkBoxShowOnlyMatching.TabIndex = 1;
            this.checkBoxShowOnlyMatching.Text = "Show only matching";
            this.checkBoxShowOnlyMatching.UseVisualStyleBackColor = true;
            this.checkBoxShowOnlyMatching.CheckedChanged += new System.EventHandler(this.checkBoxShowOnlyMatching_CheckedChanged);
            // 
            // listViewPresetEdit
            // 
            this.listViewPresetEdit.AllowDrop = true;
            this.listViewPresetEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPresetEdit.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.listViewPresetEdit.ContextMenuStrip = this.contextMenuStripFilters;
            this.listViewPresetEdit.FullRowSelect = true;
            this.listViewPresetEdit.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewPresetEdit.Location = new System.Drawing.Point(0, 0);
            this.listViewPresetEdit.MultiSelect = false;
            this.listViewPresetEdit.Name = "listViewPresetEdit";
            this.listViewPresetEdit.OwnerDraw = true;
            this.listViewPresetEdit.Size = new System.Drawing.Size(342, 185);
            this.listViewPresetEdit.TabIndex = 0;
            this.listViewPresetEdit.UseCompatibleStateImageBehavior = false;
            this.listViewPresetEdit.View = System.Windows.Forms.View.Details;
            this.listViewPresetEdit.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listViewPresetEdit_DrawColumnHeader);
            this.listViewPresetEdit.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listViewPresetEdit_DrawItem);
            this.listViewPresetEdit.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listViewPresetEdit_DrawSubItem);
            this.listViewPresetEdit.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewPresetEdit_DragDrop);
            this.listViewPresetEdit.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewPresetEdit_DragEnter);
            this.listViewPresetEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewPresetEdit_KeyDown);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Description";
            this.columnHeader5.Width = 102;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Type";
            this.columnHeader6.Width = 68;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "ID";
            this.columnHeader7.Width = 29;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Color";
            this.columnHeader8.Width = 37;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Style";
            this.columnHeader9.Width = 100;
            // 
            // contextMenuStripFilters
            // 
            this.contextMenuStripFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemOverrideName,
            this.toolStripTextBoxNameOverrride,
            this.toolStripSeparator2,
            this.toolStripMenuItemMatchType,
            this.toolStripComboBoxMatchType,
            this.toolStripSeparator1,
            this.toolStripMenuItemMatchId,
            this.toolStripTextBoxMatchId,
            this.toolStripSeparator3,
            this.toolStripMenuItemSelectColor,
            this.toolStripComboBoxLabelMode,
            this.toolStripMenuItemAddFilter});
            this.contextMenuStripFilters.Name = "contextMenuStripFilters";
            this.contextMenuStripFilters.Size = new System.Drawing.Size(264, 258);
            this.contextMenuStripFilters.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStripFilters_Closing);
            this.contextMenuStripFilters.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripFilters_Opening);
            this.contextMenuStripFilters.Opened += new System.EventHandler(this.contextMenuStripFilters_Opened);
            // 
            // toolStripMenuItemOverrideName
            // 
            this.toolStripMenuItemOverrideName.CheckOnClick = true;
            this.toolStripMenuItemOverrideName.Name = "toolStripMenuItemOverrideName";
            this.toolStripMenuItemOverrideName.Size = new System.Drawing.Size(263, 22);
            this.toolStripMenuItemOverrideName.Text = "Use name override (click to change)";
            // 
            // toolStripTextBoxNameOverrride
            // 
            this.toolStripTextBoxNameOverrride.Name = "toolStripTextBoxNameOverrride";
            this.toolStripTextBoxNameOverrride.Size = new System.Drawing.Size(100, 23);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(260, 6);
            // 
            // toolStripMenuItemMatchType
            // 
            this.toolStripMenuItemMatchType.CheckOnClick = true;
            this.toolStripMenuItemMatchType.Name = "toolStripMenuItemMatchType";
            this.toolStripMenuItemMatchType.Size = new System.Drawing.Size(263, 22);
            this.toolStripMenuItemMatchType.Text = "Match type (click to change)";
            // 
            // toolStripComboBoxMatchType
            // 
            this.toolStripComboBoxMatchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxMatchType.Name = "toolStripComboBoxMatchType";
            this.toolStripComboBoxMatchType.Size = new System.Drawing.Size(121, 23);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(260, 6);
            // 
            // toolStripMenuItemMatchId
            // 
            this.toolStripMenuItemMatchId.CheckOnClick = true;
            this.toolStripMenuItemMatchId.Name = "toolStripMenuItemMatchId";
            this.toolStripMenuItemMatchId.Size = new System.Drawing.Size(263, 22);
            this.toolStripMenuItemMatchId.Text = "Match ID (click to change)";
            // 
            // toolStripTextBoxMatchId
            // 
            this.toolStripTextBoxMatchId.Name = "toolStripTextBoxMatchId";
            this.toolStripTextBoxMatchId.Size = new System.Drawing.Size(100, 23);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(260, 6);
            // 
            // toolStripMenuItemSelectColor
            // 
            this.toolStripMenuItemSelectColor.Name = "toolStripMenuItemSelectColor";
            this.toolStripMenuItemSelectColor.Size = new System.Drawing.Size(263, 22);
            this.toolStripMenuItemSelectColor.Text = "Select color...";
            this.toolStripMenuItemSelectColor.Click += new System.EventHandler(this.toolStripMenuItemSelectColor_Click);
            // 
            // toolStripComboBoxLabelMode
            // 
            this.toolStripComboBoxLabelMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxLabelMode.Name = "toolStripComboBoxLabelMode";
            this.toolStripComboBoxLabelMode.Size = new System.Drawing.Size(121, 23);
            // 
            // comboBoxPreset
            // 
            this.comboBoxPreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPreset.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxPreset.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPreset.FormattingEnabled = true;
            this.comboBoxPreset.Location = new System.Drawing.Point(50, 3);
            this.comboBoxPreset.Name = "comboBoxPreset";
            this.comboBoxPreset.Size = new System.Drawing.Size(220, 21);
            this.comboBoxPreset.TabIndex = 1;
            this.comboBoxPreset.SelectedIndexChanged += new System.EventHandler(this.comboBoxPreset_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Preset:";
            // 
            // checkBoxPauseScan
            // 
            this.checkBoxPauseScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPauseScan.AutoSize = true;
            this.checkBoxPauseScan.Location = new System.Drawing.Point(346, 6);
            this.checkBoxPauseScan.Name = "checkBoxPauseScan";
            this.checkBoxPauseScan.Size = new System.Drawing.Size(97, 17);
            this.checkBoxPauseScan.TabIndex = 2;
            this.checkBoxPauseScan.Text = "Pause updates";
            this.checkBoxPauseScan.UseVisualStyleBackColor = true;
            this.checkBoxPauseScan.CheckedChanged += new System.EventHandler(this.checkBoxPauseScan_CheckedChanged);
            // 
            // listViewActors
            // 
            this.listViewActors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewActors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeaderDistance});
            this.listViewActors.FullRowSelect = true;
            this.listViewActors.Location = new System.Drawing.Point(0, 28);
            this.listViewActors.Margin = new System.Windows.Forms.Padding(0);
            this.listViewActors.MultiSelect = false;
            this.listViewActors.Name = "listViewActors";
            this.listViewActors.Size = new System.Drawing.Size(447, 401);
            this.listViewActors.TabIndex = 1;
            this.listViewActors.UseCompatibleStateImageBehavior = false;
            this.listViewActors.View = System.Windows.Forms.View.Details;
            this.listViewActors.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewActors_ColumnClick);
            this.listViewActors.SelectedIndexChanged += new System.EventHandler(this.listViewActors_SelectedIndexChanged);
            this.listViewActors.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listViewActors_MouseMove);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 163;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 117;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "ID";
            this.columnHeader3.Width = 79;
            // 
            // columnHeaderDistance
            // 
            this.columnHeaderDistance.Text = "Distance";
            this.columnHeaderDistance.Width = 81;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Nearby actors:";
            // 
            // timerScan
            // 
            this.timerScan.Enabled = true;
            this.timerScan.Interval = 1000;
            this.timerScan.Tick += new System.EventHandler(this.timerScan_Tick);
            // 
            // labelUpdateNotify
            // 
            this.labelUpdateNotify.BackColor = System.Drawing.Color.Lime;
            this.labelUpdateNotify.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelUpdateNotify.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelUpdateNotify.Location = new System.Drawing.Point(0, 434);
            this.labelUpdateNotify.Name = "labelUpdateNotify";
            this.labelUpdateNotify.Size = new System.Drawing.Size(817, 55);
            this.labelUpdateNotify.TabIndex = 25;
            this.labelUpdateNotify.Text = "New version downloaded, please restart program to finish update. Click here to hi" +
    "de.";
            this.labelUpdateNotify.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelUpdateNotify.Visible = false;
            this.labelUpdateNotify.Click += new System.EventHandler(this.labelUpdateNotify_Click);
            // 
            // toolStripPresetListTextBox
            // 
            this.toolStripPresetListTextBox.Name = "toolStripPresetListTextBox";
            this.toolStripPresetListTextBox.Size = new System.Drawing.Size(100, 23);
            this.toolStripPresetListTextBox.TextChanged += new System.EventHandler(this.toolStripPresetListTextBox_TextChanged);
            // 
            // labelFilterHint
            // 
            this.labelFilterHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFilterHint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labelFilterHint.Location = new System.Drawing.Point(1, 162);
            this.labelFilterHint.Name = "labelFilterHint";
            this.labelFilterHint.Size = new System.Drawing.Size(340, 21);
            this.labelFilterHint.TabIndex = 2;
            this.labelFilterHint.Text = "Filter list empty, right click or drag && drop from actor list >>";
            this.labelFilterHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStripMenuItemAddFilter
            // 
            this.toolStripMenuItemAddFilter.Name = "toolStripMenuItemAddFilter";
            this.toolStripMenuItemAddFilter.Size = new System.Drawing.Size(263, 22);
            this.toolStripMenuItemAddFilter.Text = "Add filter";
            this.toolStripMenuItemAddFilter.Click += new System.EventHandler(this.toolStripMenuItemAddFilter_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 489);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelScanState);
            this.Controls.Add(this.labelUpdateNotify);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "FF Radar Buddy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.panelScanState.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStripManagePresets.ResumeLayout(false);
            this.contextMenuStripManagePresets.PerformLayout();
            this.contextMenuStripFilters.ResumeLayout(false);
            this.contextMenuStripFilters.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelScanState;
        private System.Windows.Forms.Label labelScanState;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox comboBoxPreset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeaderDistance;
        private System.Windows.Forms.Timer timerScan;
        private System.Windows.Forms.ListView listViewActors;
        private System.Windows.Forms.CheckBox checkBoxPauseScan;
        private System.Windows.Forms.Button buttonPresetOptions;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonImportPreset;
        private System.Windows.Forms.Button buttonAddPreset;
        private System.Windows.Forms.ListView listViewPresetManage;
        private System.Windows.Forms.Label labelUpdateNotify;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripManagePresets;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPresetExport;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPresetDelete;
        private System.Windows.Forms.ListView listViewPresetEdit;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.CheckBox checkBoxShowOnlyMatching;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFilters;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOverrideName;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxNameOverrride;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMatchType;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxMatchType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMatchId;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxMatchId;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectColor;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxLabelMode;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripTextBox toolStripPresetListTextBox;
        private System.Windows.Forms.Label labelFilterHint;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddFilter;
    }
}

