namespace FA
{
    partial class frmBRSPrevYr
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
            this.vGridBRS = new DevExpress.XtraVerticalGrid.VGridControl();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.standaloneBarDockControl4 = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnRegister = new DevExpress.XtraBars.BarButtonItem();
            this.btnOk = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.standaloneBarDockControl5 = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.dockRemarks = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.txtNarration = new DevExpress.XtraEditors.MemoEdit();
            this.btnExit = new DevExpress.XtraBars.BarButtonItem();
            this.btnBRSExit = new DevExpress.XtraBars.BarButtonItem();
            this.documentWindow2 = new Telerik.WinControls.UI.Docking.DocumentWindow();
            this.grpDetails = new DevExpress.XtraEditors.GroupControl();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.grdBRSReg = new DevExpress.XtraGrid.GridControl();
            this.grdViewBRSReg = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.vGridBRS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockRemarks.SuspendLayout();
            this.dockPanel2_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNarration.Properties)).BeginInit();
            this.documentWindow2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdBRSReg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewBRSReg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // vGridBRS
            // 
            this.vGridBRS.Appearance.RowHeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Bold);
            this.vGridBRS.Appearance.RowHeaderPanel.Options.UseFont = true;
            this.vGridBRS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vGridBRS.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.vGridBRS.LayoutStyle = DevExpress.XtraVerticalGrid.LayoutViewStyle.SingleRecordView;
            this.vGridBRS.Location = new System.Drawing.Point(2, 2);
            this.vGridBRS.LookAndFeel.SkinName = "Money Twins";
            this.vGridBRS.LookAndFeel.UseDefaultLookAndFeel = false;
            this.vGridBRS.Name = "vGridBRS";
            this.vGridBRS.OptionsBehavior.ResizeRowHeaders = false;
            this.vGridBRS.OptionsBehavior.UseEnterAsTab = true;
            this.vGridBRS.OptionsView.AutoScaleBands = true;
            this.vGridBRS.OptionsView.MinRowAutoHeight = 15;
            this.vGridBRS.RecordWidth = 150;
            this.vGridBRS.RowHeaderWidth = 50;
            this.vGridBRS.Size = new System.Drawing.Size(660, 279);
            this.vGridBRS.TabIndex = 0;
            this.vGridBRS.ShownEditor += new System.EventHandler(this.vGridBRS_ShownEditor);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.DockControls.Add(this.standaloneBarDockControl4);
            this.barManager1.DockControls.Add(this.standaloneBarDockControl5);
            this.barManager1.DockManager = this.dockManager1;
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnAdd,
            this.btnDelete,
            this.btnOk,
            this.btnCancel,
            this.btnExit,
            this.btnBRSExit,
            this.btnRegister});
            this.barManager1.MaxItemId = 11;
            // 
            // bar1
            // 
            this.bar1.BarName = "Custom 3";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAdd, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnDelete, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.StandaloneBarDockControl = this.standaloneBarDockControl4;
            this.bar1.Text = "Custom 3";
            // 
            // btnAdd
            // 
            this.btnAdd.Caption = "Add";
            this.btnAdd.Glyph = global::FA.Properties.Resources.application_add;
            this.btnAdd.Id = 2;
            this.btnAdd.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ItemAppearance.Normal.Options.UseFont = true;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAdd_ItemClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "Delete";
            this.btnDelete.Glyph = global::FA.Properties.Resources.application_delete;
            this.btnDelete.Id = 4;
            this.btnDelete.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ItemAppearance.Normal.Options.UseFont = true;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
            // 
            // standaloneBarDockControl4
            // 
            this.standaloneBarDockControl4.CausesValidation = false;
            this.standaloneBarDockControl4.Dock = System.Windows.Forms.DockStyle.Top;
            this.standaloneBarDockControl4.Location = new System.Drawing.Point(0, 0);
            this.standaloneBarDockControl4.Name = "standaloneBarDockControl4";
            this.standaloneBarDockControl4.Size = new System.Drawing.Size(664, 23);
            this.standaloneBarDockControl4.Text = "standaloneBarDockControl4";
            // 
            // bar2
            // 
            this.bar2.BarName = "Custom 4";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRegister),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnOk, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnCancel, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.StandaloneBarDockControl = this.standaloneBarDockControl5;
            this.bar2.Text = "Custom 4";
            // 
            // btnRegister
            // 
            this.btnRegister.Caption = ">>Show Details";
            this.btnRegister.Id = 10;
            this.btnRegister.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ItemAppearance.Normal.Options.UseFont = true;
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRegister_ItemClick);
            // 
            // btnOk
            // 
            this.btnOk.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnOk.Caption = "Ok";
            this.btnOk.Glyph = global::FA.Properties.Resources.Ok16;
            this.btnOk.Id = 6;
            this.btnOk.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnOk.ItemAppearance.Normal.Options.UseFont = true;
            this.btnOk.Name = "btnOk";
            this.btnOk.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnOk_ItemClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Caption = "Cancel";
            this.btnCancel.Glyph = global::FA.Properties.Resources.Cancel16;
            this.btnCancel.Id = 7;
            this.btnCancel.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ItemAppearance.Normal.Options.UseFont = true;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCancel_ItemClick);
            // 
            // standaloneBarDockControl5
            // 
            this.standaloneBarDockControl5.CausesValidation = false;
            this.standaloneBarDockControl5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.standaloneBarDockControl5.Location = new System.Drawing.Point(2, 258);
            this.standaloneBarDockControl5.Name = "standaloneBarDockControl5";
            this.standaloneBarDockControl5.Size = new System.Drawing.Size(660, 23);
            this.standaloneBarDockControl5.Text = "standaloneBarDockControl5";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(664, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 406);
            this.barDockControlBottom.Size = new System.Drawing.Size(664, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 406);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(664, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 406);
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.HiddenPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel1});
            this.dockManager1.MenuManager = this.barManager1;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockRemarks});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.dockPanel1_Container);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.dockPanel1.ID = new System.Guid("534a2c4d-87ce-44c1-a361-9d5435138d7a");
            this.dockPanel1.Location = new System.Drawing.Point(0, 95);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockPanel1.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.dockPanel1.SavedIndex = 0;
            this.dockPanel1.Size = new System.Drawing.Size(664, 200);
            this.dockPanel1.Text = "dockPanel1";
            this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Location = new System.Drawing.Point(3, 29);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(658, 168);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // dockRemarks
            // 
            this.dockRemarks.Controls.Add(this.dockPanel2_Container);
            this.dockRemarks.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.dockRemarks.ID = new System.Guid("570b657d-609e-4229-b232-f0cf6d44018a");
            this.dockRemarks.Location = new System.Drawing.Point(0, 306);
            this.dockRemarks.Name = "dockRemarks";
            this.dockRemarks.Options.ShowCloseButton = false;
            this.dockRemarks.OriginalSize = new System.Drawing.Size(200, 100);
            this.dockRemarks.Size = new System.Drawing.Size(664, 100);
            this.dockRemarks.Text = "Remarks";
            // 
            // dockPanel2_Container
            // 
            this.dockPanel2_Container.Controls.Add(this.txtNarration);
            this.dockPanel2_Container.Location = new System.Drawing.Point(3, 29);
            this.dockPanel2_Container.Name = "dockPanel2_Container";
            this.dockPanel2_Container.Size = new System.Drawing.Size(658, 68);
            this.dockPanel2_Container.TabIndex = 0;
            // 
            // txtNarration
            // 
            this.txtNarration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNarration.Location = new System.Drawing.Point(0, 0);
            this.txtNarration.MenuManager = this.barManager1;
            this.txtNarration.Name = "txtNarration";
            this.txtNarration.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.txtNarration.Properties.Appearance.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtNarration.Properties.Appearance.Options.UseFont = true;
            this.txtNarration.Properties.Appearance.Options.UseForeColor = true;
            this.txtNarration.Size = new System.Drawing.Size(658, 68);
            this.txtNarration.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnExit.Caption = "Exit";
            this.btnExit.Glyph = global::FA.Properties.Resources.exit1;
            this.btnExit.Id = 8;
            this.btnExit.Name = "btnExit";
            this.btnExit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExit_ItemClick);
            // 
            // btnBRSExit
            // 
            this.btnBRSExit.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnBRSExit.Caption = "Exit";
            this.btnBRSExit.Glyph = global::FA.Properties.Resources.exit1;
            this.btnBRSExit.Id = 9;
            this.btnBRSExit.Name = "btnBRSExit";
            this.btnBRSExit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnBRSExit_ItemClick);
            // 
            // documentWindow2
            // 
            this.documentWindow2.CloseAction = Telerik.WinControls.UI.Docking.DockWindowCloseAction.Hide;
            this.documentWindow2.Controls.Add(this.grpDetails);
            this.documentWindow2.DesiredDockState = Telerik.WinControls.UI.Docking.DockState.Hidden;
            this.documentWindow2.DocumentButtons = Telerik.WinControls.UI.Docking.DocumentStripButtons.ActiveWindowList;
            this.documentWindow2.Location = new System.Drawing.Point(6, 29);
            this.documentWindow2.Name = "documentWindow2";
            this.documentWindow2.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.TabbedDocument;
            this.documentWindow2.Size = new System.Drawing.Size(642, 250);
            this.documentWindow2.Text = "BRS ";
            this.documentWindow2.ToolCaptionButtons = Telerik.WinControls.UI.Docking.ToolStripCaptionButtons.None;
            // 
            // grpDetails
            // 
            this.grpDetails.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.grpDetails.Appearance.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.grpDetails.Appearance.Options.UseBackColor = true;
            this.grpDetails.Appearance.Options.UseFont = true;
            this.grpDetails.AppearanceCaption.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.grpDetails.AppearanceCaption.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grpDetails.AppearanceCaption.Options.UseFont = true;
            this.grpDetails.AppearanceCaption.Options.UseForeColor = true;
            this.grpDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDetails.Location = new System.Drawing.Point(0, 0);
            this.grpDetails.LookAndFeel.SkinName = "Money Twins";
            this.grpDetails.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(642, 250);
            this.grpDetails.TabIndex = 2;
            this.grpDetails.Text = "General";
            // 
            // defaultLookAndFeel1
            // 
            this.defaultLookAndFeel1.LookAndFeel.SkinName = "Blue";
            // 
            // grdBRSReg
            // 
            this.grdBRSReg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdBRSReg.Location = new System.Drawing.Point(2, 2);
            this.grdBRSReg.LookAndFeel.SkinName = "Money Twins";
            this.grdBRSReg.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grdBRSReg.MainView = this.grdViewBRSReg;
            this.grdBRSReg.MenuManager = this.barManager1;
            this.grdBRSReg.Name = "grdBRSReg";
            this.grdBRSReg.Size = new System.Drawing.Size(660, 256);
            this.grdBRSReg.TabIndex = 7;
            this.grdBRSReg.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewBRSReg});
            // 
            // grdViewBRSReg
            // 
            this.grdViewBRSReg.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.grdViewBRSReg.GridControl = this.grdBRSReg;
            this.grdViewBRSReg.Name = "grdViewBRSReg";
            this.grdViewBRSReg.OptionsSelection.InvertSelection = true;
            this.grdViewBRSReg.OptionsView.ShowGroupPanel = false;
            this.grdViewBRSReg.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grdViewBRSReg_FocusedRowChanged);
            this.grdViewBRSReg.DoubleClick += new System.EventHandler(this.grdViewBRSReg_DoubleClick);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.grdBRSReg);
            this.panelControl1.Controls.Add(this.standaloneBarDockControl5);
            this.panelControl1.Controls.Add(this.vGridBRS);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 23);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(664, 283);
            this.panelControl1.TabIndex = 14;
            // 
            // frmBRSPrevYr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 406);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.dockRemarks);
            this.Controls.Add(this.standaloneBarDockControl4);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "Office 2010 Blue";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmBRSPrevYr";
            this.Text = "BRS Previous Year";
            this.Load += new System.EventHandler(this.frmBRSPrevYr_Load);
            ((System.ComponentModel.ISupportInitialize)(this.vGridBRS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockRemarks.ResumeLayout(false);
            this.dockPanel2_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtNarration.Properties)).EndInit();
            this.documentWindow2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdBRSReg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewBRSReg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.Docking.DocumentWindow documentWindow2;
        private DevExpress.XtraEditors.GroupControl grpDetails;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraVerticalGrid.VGridControl vGridBRS;
        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockRemarks;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraEditors.MemoEdit txtNarration;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btnAdd;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnOk;
        private DevExpress.XtraBars.BarButtonItem btnCancel;
        private DevExpress.XtraBars.BarButtonItem btnExit;
        private DevExpress.XtraBars.BarButtonItem btnBRSExit;
        private DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl4;
        private DevExpress.XtraGrid.GridControl grdBRSReg;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewBRSReg;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl5;
        private DevExpress.XtraBars.BarButtonItem btnRegister;
    }
}