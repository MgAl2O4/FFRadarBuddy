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
            this.listViewFilters = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBoxPreset = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewActors = new FFRadarBuddy.ListViewDoubleBuffered();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDistance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.timerScan = new System.Windows.Forms.Timer(this.components);
            this.panelScanState.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.panelScanState.Size = new System.Drawing.Size(766, 30);
            this.panelScanState.TabIndex = 0;
            // 
            // labelScanState
            // 
            this.labelScanState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelScanState.Location = new System.Drawing.Point(3, 0);
            this.labelScanState.Name = "labelScanState";
            this.labelScanState.Size = new System.Drawing.Size(758, 28);
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
            this.splitContainer1.Panel1.Controls.Add(this.listViewFilters);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxPreset);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewActors);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(766, 433);
            this.splitContainer1.SplitterDistance = 331;
            this.splitContainer1.TabIndex = 1;
            // 
            // listViewFilters
            // 
            this.listViewFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFilters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6});
            this.listViewFilters.FullRowSelect = true;
            this.listViewFilters.Location = new System.Drawing.Point(3, 30);
            this.listViewFilters.MultiSelect = false;
            this.listViewFilters.Name = "listViewFilters";
            this.listViewFilters.Size = new System.Drawing.Size(325, 403);
            this.listViewFilters.TabIndex = 2;
            this.listViewFilters.UseCompatibleStateImageBehavior = false;
            this.listViewFilters.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "TODO STUFF";
            this.columnHeader6.Width = 114;
            // 
            // comboBoxPreset
            // 
            this.comboBoxPreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPreset.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxPreset.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPreset.FormattingEnabled = true;
            this.comboBoxPreset.Location = new System.Drawing.Point(74, 3);
            this.comboBoxPreset.Name = "comboBoxPreset";
            this.comboBoxPreset.Size = new System.Drawing.Size(254, 21);
            this.comboBoxPreset.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter preset:";
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
            this.listViewActors.Location = new System.Drawing.Point(3, 30);
            this.listViewActors.MultiSelect = false;
            this.listViewActors.Name = "listViewActors";
            this.listViewActors.Size = new System.Drawing.Size(424, 403);
            this.listViewActors.TabIndex = 1;
            this.listViewActors.UseCompatibleStateImageBehavior = false;
            this.listViewActors.View = System.Windows.Forms.View.Details;
            this.listViewActors.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewActors_ColumnClick);
            this.listViewActors.SelectedIndexChanged += new System.EventHandler(this.listViewActors_SelectedIndexChanged);
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
            this.columnHeader3.Width = 70;
            // 
            // columnHeaderDistance
            // 
            this.columnHeaderDistance.Text = "Distance";
            this.columnHeaderDistance.Width = 67;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 493);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelScanState);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelScanState;
        private System.Windows.Forms.Label labelScanState;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox comboBoxPreset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewFilters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeaderDistance;
        private System.Windows.Forms.Timer timerScan;
        private ListViewDoubleBuffered listViewActors;
    }
}

