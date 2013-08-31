using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FA.BusinessLayer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraPrinting;

namespace FA
{
    public partial class frmSLAnalysis: DevExpress.XtraEditors.XtraForm
    {

        #region Declaration

        DataTable m_dtSLType;
        DataTable m_dtSLedger;
        DataTable m_dtBills;
        DataSet m_dsVType;
        DataTable m_dtCC;
        DataTable m_dtAccType;
        int m_iSLTypeId, m_iSLId;
        DateTime m_dAsOn;
        string m_sGeneral;
        string m_sTransName;
        int m_iLevel;
        string m_sIds;
        bool m_bLoad;
        #endregion

        #region Objects
        #endregion

        #region Constructor

        public frmSLAnalysis()
        {
            InitializeComponent();
        }

        #endregion

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!DesignMode && IsHandleCreated)
                BeginInvoke(new MethodInvoker(() =>{base.OnSizeChanged(e);}));
            else
                base.OnSizeChanged(e);
        }

        #region Form Event

        private void frmSLAnalysis_Load(object sender, EventArgs e)
        {
            defaultLookAndFeel1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Blue";

            clsStatic.SetMyGraphics();
            m_sIds = string.Empty;
            dtAson.EditValue = Convert.ToDateTime(BsfGlobal.g_dEndDate);
            m_dAsOn = Convert.ToDateTime(dtAson.EditValue);
            Fill_CostCentre();
            Get_Account_Type();
            repdtAsOn.MinValue = Convert.ToDateTime(BsfGlobal.g_dStartDate);
            repdtAsOn.MaxValue = Convert.ToDateTime(BsfGlobal.g_dEndDate);
            dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            repcboVendorType.Items.Clear();
            repcboVendorType.Items.Add("(ALL)");
            repcboVendorType.Items.Add("Supply");
            repcboVendorType.Items.Add("Works");
            repcboVendorType.Items.Add("Service");
            repcboVendorType.Items.Add("Others");
            btnPrev.Enabled = false;
            dwSubLedger.Hide();
            dwBillDetails.Hide();
            CheckPermission();
            SendKeys.Send("{TAB}") ;
        }

        private void Get_Account_Type()
        {
            m_dtAccType = new DataTable();
            m_dtAccType = EntryBL.Get_SLAccType();
        }

        private void frmSLAnalysis_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (BsfGlobal.g_bWorkFlow == true)
            {
                if (BsfGlobal.g_bWorkFlowDialog == false)
                {
                    try
                    {
                        Parent.Controls.Owner.Hide();
                    }
                    catch 
                    {
                        //BsfGlobal.CustomException(ex.Message, ex.StackTrace);
                    }
                }
            }
            else
            {
                clsStatic.DW1.Hide();
            }

        }
             
        #endregion

        #region Bar Item Event

        private void cboCostCentre_EditValueChanged(object sender, EventArgs e)
        {
            SLAnalysisBL.CCId = Convert.ToInt32(cboCostCentre.EditValue.ToString());
            SLAnalysisBL.CCName = cboCostCentre.Description;
            Fill_SLType();
      
        }

        private void barEditPeriod_EditValueChanged(object sender, EventArgs e)
        {
            m_dAsOn = Convert.ToDateTime(dtAson.EditValue);
            Fill_SLType();
        }

        private void barButtonItemExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void barbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            if (radDock1.ActiveWindow.Name == "dwSLType")
            {
                PrintableComponentLink Link = new PrintableComponentLink(new PrintingSystem()) { PaperKind = System.Drawing.Printing.PaperKind.A4, Landscape = true, Component = grdSLType };
                Link.CreateMarginalHeaderArea += Link_CreateMarginalHeaderArea;
                Link.CreateMarginalFooterArea += Link_CreateMarginalFooterArea;
                Link.CreateDocument();
                Link.ShowPreview();
            }
                
            else if (radDock1.ActiveWindow.Name == "dwSubLedger")
            {
                PrintableComponentLink Link = new PrintableComponentLink(new PrintingSystem()) { PaperKind = System.Drawing.Printing.PaperKind.A4, Landscape = true, Component = grdSLedger};
                Link.CreateMarginalHeaderArea += Link_CreateMarginalHeaderArea;
                Link.CreateMarginalFooterArea += Link_CreateMarginalFooterArea;
                Link.CreateDocument();
                Link.ShowPreview();
            }
            else if (radDock1.ActiveWindow.Name == "dwBillDetails")
            {
                PrintableComponentLink Link = new PrintableComponentLink(new PrintingSystem()) { PaperKind = System.Drawing.Printing.PaperKind.A4, Landscape = true, Component = grdBill};
                Link.CreateMarginalHeaderArea += Link_CreateMarginalHeaderArea;
                Link.CreateMarginalFooterArea += Link_CreateMarginalFooterArea;
                Link.CreateDocument();
                Link.ShowPreview();
            }

            
        }

        void Link_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
        {
            PageInfoBrick pib = new PageInfoBrick()
            {
                PageInfo = PageInfo.Number,
                Rect = new RectangleF(0, 0, 300, 20),
                Alignment = BrickAlignment.Far,
                BorderWidth = 0,
                HorzAlignment = DevExpress.Utils.HorzAlignment.Far,
                Font = new Font("Arial", 8, FontStyle.Italic),
                Format = "Page : {0}"
            };

            e.Graph.DrawBrick(pib);
        }

        void Link_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e) 
        {
            string sHeader = null;
            TextBrick txtBrickHeader = default(TextBrick);
            TextBrick txtBrickCompany = default(TextBrick);
            if (radDock1.ActiveWindow.Name == "dwSLType")
                sHeader = String.Format(dwSLType.Text +"- {0} - As On {1} ", repcboCostCentre.GetDisplayText((int)cboCostCentre.EditValue),m_dAsOn.ToShortDateString());
            else if (radDock1.ActiveWindow.Name == "dwSubLedger")
                sHeader = String.Format(dwSubLedger.Text + "- {0} - As On {1} ", repcboCostCentre.GetDisplayText((int)cboCostCentre.EditValue), m_dAsOn.ToShortDateString());
            else if (radDock1.ActiveWindow.Name == "dwBillDetails")
                sHeader = String.Format(dwBillDetails.Text + "- {0} - As On {1} ", repcboCostCentre.GetDisplayText((int)cboCostCentre.EditValue), m_dAsOn.ToShortDateString());

            txtBrickCompany = e.Graph.DrawString(BsfGlobal.g_sCompanyName, Color.Navy, new RectangleF(0,0,e.Graph.ClientPageSize.Width,50), BorderSide.None);
            txtBrickCompany.Font = new Font("Arial", 12, FontStyle.Bold);
            txtBrickCompany.StringFormat = new BrickStringFormat(StringAlignment.Center);

            txtBrickHeader = e.Graph.DrawString(sHeader, Color.Navy, new RectangleF(0, 40, 700, 40), BorderSide.None);
            txtBrickHeader.Font = new Font("Arial", 10, FontStyle.Bold);
            txtBrickHeader.StringFormat = new BrickStringFormat(StringAlignment.Near);
            

            
        }
    
        #endregion

        #region Functions
                
        private void CheckPermission()
        {
            if (BsfGlobal.g_sUnPermissionMode == "H")
            {
                if (BsfGlobal.FindPermission("General-Ledger-Print") == false) { barbtnPrint.Visibility = DevExpress.XtraBars.BarItemVisibility.Never; }

            }
            else if (BsfGlobal.g_sUnPermissionMode == "D")
            {
                if (BsfGlobal.FindPermission("General-Ledger-Print") == false) { barbtnPrint.Enabled = false; }
            }

        }

        public void Execute()
        {
            Show();
        }

        private void Fill_CostCentre()
        {
            m_dtCC = new DataTable();
            m_dtCC = CommonBL.Get_CostCentre();
            m_dtCC = clsStatic.AddSelectToDataTable_All(m_dtCC, "(ALL)");
            repcboCostCentre.DataSource = null;
            repcboCostCentre.Columns.Clear();
            repcboCostCentre.NullText = "--Select--";
            repcboCostCentre.DataSource = m_dtCC;
            repcboCostCentre.ForceInitialize();
            repcboCostCentre.PopulateColumns();
            repcboCostCentre.DisplayMember = "CostCentreName";
            repcboCostCentre.ValueMember = "CostCentreId";
            repcboCostCentre.Columns["CostCentreId"].Visible = false;
            repcboCostCentre.Columns["CostCentreName"].Visible = true;
            cboCostCentre.EditValue = -1;
        }

        private void Fill_SLType()
        {
            m_dtSLType= new DataTable();
            m_dtSLType = SLAnalysisBL.Get_SLType_Det(Convert.ToDateTime(dtAson.EditValue.ToString()));

            grdSLType.DataSource = null;
            grdViewSLType.Columns.Clear();
            grdSLType.DataSource = m_dtSLType;
            grdSLType.ForceInitialize();
            grdViewSLType.PopulateColumns();

            grdViewSLType.Columns["SLTypeId"].Visible = false;

            grdViewSLType.Columns["SLTypeName"].Visible = true;
            grdViewSLType.Columns["SLTypeName"].OptionsColumn.AllowEdit = false;
            grdViewSLType.Columns["SLTypeName"].Caption = "Type Name";
            grdViewSLType.Columns["SLTypeName"].Width = 400;
            grdViewSLType.Columns["SLTypeName"].Summary.Add(SummaryItemType.Count, "SLTypeName", "Total");
            //grdViewSLType.Columns["SLTypeName"].Summary.Add(SummaryItemType.Custom, "SLTypeName", "Balance");

            grdViewSLType.Columns["Debit"].Visible = true;
            grdViewSLType.Columns["Debit"].Caption = "Debit";
            grdViewSLType.Columns["Debit"].OptionsColumn.AllowEdit = false;
            grdViewSLType.Columns["Debit"].Width = 150;
            grdViewSLType.Columns["Debit"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewSLType.Columns["Debit"].DisplayFormat.FormatString = clsStatic.sFormatAmt;
            grdViewSLType.Columns["Debit"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            grdViewSLType.Columns["Debit"].Summary.Add(SummaryItemType.Sum, "Debit", clsStatic.sFormatTotAmt);
            //grdViewSLType.Columns["Debit"].Summary.Add(SummaryItemType.Custom, "Debit", clsStatic.sFormatTotAmt);
            grdViewSLType.Columns["Credit"].SummaryItem.Tag = 0;

            grdViewSLType.Columns["Credit"].Visible = true;
            grdViewSLType.Columns["Credit"].Caption = "Credit";
            grdViewSLType.Columns["Credit"].OptionsColumn.AllowEdit = false;
            grdViewSLType.Columns["Credit"].Width = 150;
            grdViewSLType.Columns["Credit"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewSLType.Columns["Credit"].DisplayFormat.FormatString = clsStatic.sFormatAmt;
            grdViewSLType.Columns["Credit"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            grdViewSLType.Columns["Credit"].Summary.Add(SummaryItemType.Sum, "Credit", clsStatic.sFormatTotAmt);
            grdViewSLType.Columns["Credit"].SummaryItem.Tag = 1;
            //grdViewSLType.Columns["Credit"].Summary.Add(SummaryItemType.Custom, "Credit", clsStatic.sFormatTotAmt);

            grdViewSLType.OptionsCustomization.AllowFilter = false;
            grdViewSLType.OptionsCustomization.AllowSort = false;
            grdViewSLType.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewSLType.OptionsView.ShowAutoFilterRow = true;
            grdViewSLType.OptionsView.ShowViewCaption = false;
            grdViewSLType.OptionsView.ShowFooter = true;
            grdViewSLType.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            grdViewSLType.OptionsSelection.InvertSelection = true;
            grdViewSLType.OptionsView.ColumnAutoWidth = true;
            grdViewSLType.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewSLType.Columns["Credit"].OptionsColumn.AllowIncrementalSearch = false;
            grdViewSLType.Columns["Debit"].OptionsColumn.AllowIncrementalSearch = false;
            grdViewSLType.Appearance.HeaderPanel.Font = new Font(grdViewSLType.Appearance.HeaderPanel.Font, FontStyle.Bold);
            grdViewSLType.Appearance.FooterPanel.Font = new Font(grdViewSLType.Appearance.FooterPanel.Font, FontStyle.Bold);
            grdViewSLType.FocusedRowHandle = 0;
            grdViewSLType.FocusedColumn = grdViewSLType.VisibleColumns[0];

            grdViewSLType.Appearance.FocusedRow.BackColor = Color.Teal;
            grdViewSLType.Appearance.FocusedRow.ForeColor = Color.White;
            grdViewSLType.Appearance.HideSelectionRow.BackColor = Color.Teal;
            grdViewSLType.Appearance.HideSelectionRow.ForeColor = Color.White;

        }

        private void Fill_TransDet()
        {
            if (m_iSLTypeId != 1)
                panelControl1.Visible = false;
            else
            {
                panelControl1.Visible = true;
                
            }
            if (SLAnalysisBL.VTypeId == 0)
            {
                m_dtSLedger = SLAnalysisBL.Get_SLType_TransDet(m_iSLTypeId, m_dAsOn,m_sIds);
                grdSLedger.DataSource = null;
                grdViewSLedger.Columns.Clear();
                grdSLedger.DataSource = m_dtSLedger;
            }
            else
            {
                Fill_Details_VendorType();
                grdSLedger.DataSource = null;
                grdViewSLedger.Columns.Clear();
                grdSLedger.DataSource = m_dsVType.Tables["VTypeInfo"];
            }
            grdSLedger.ForceInitialize();
            grdViewSLedger.PopulateColumns();
            grdViewSLedger.Columns["SLTypeId"].Visible = false;
            grdViewSLedger.Columns["SLId"].Visible = false;
            grdViewSLedger.Columns["SLName"].Visible = true;
            grdViewSLedger.Columns["SLName"].Caption = "Sub Ledger Name";
            grdViewSLedger.Columns["SLName"].Width = 400;
            grdViewSLedger.Columns["SLName"].Summary.Add(SummaryItemType.Count, "SLName", "Total");
            grdViewSLedger.Columns["SLName"].Summary.Add(SummaryItemType.Custom, "SLName", "Balance");


            grdViewSLedger.Columns["Debit"].Visible = true;
            grdViewSLedger.Columns["Debit"].Caption = "Debit";
            grdViewSLedger.Columns["Debit"].Width = 150;
            grdViewSLedger.Columns["Debit"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewSLedger.Columns["Debit"].DisplayFormat.FormatString = clsStatic.sFormatAmt;
            grdViewSLedger.Columns["Debit"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            grdViewSLedger.Columns["Debit"].Summary.Add(SummaryItemType.Sum, "Debit", clsStatic.sFormatTotAmt);
            grdViewSLedger.Columns["Debit"].Summary.Add(SummaryItemType.Custom, "Debit", clsStatic.sFormatTotAmt);
            grdViewSLedger.Columns["Credit"].SummaryItem.Tag = 0;

            grdViewSLedger.Columns["Credit"].Visible = true;
            grdViewSLedger.Columns["Credit"].Caption = "Credit";
            
            grdViewSLedger.Columns["Credit"].Width = 150;
            grdViewSLedger.Columns["Credit"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewSLedger.Columns["Credit"].DisplayFormat.FormatString = clsStatic.sFormatAmt;
            grdViewSLedger.Columns["Credit"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            grdViewSLedger.Columns["Credit"].Summary.Add(SummaryItemType.Sum, "Credit", clsStatic.sFormatTotAmt);
            grdViewSLedger.Columns["Credit"].SummaryItem.Tag = 1;
            grdViewSLedger.Columns["Credit"].Summary.Add(SummaryItemType.Custom, "Credit", clsStatic.sFormatTotAmt);

            grdViewSLedger.OptionsBehavior.Editable = false;
            grdViewSLedger.OptionsCustomization.AllowFilter = false;
            grdViewSLedger.OptionsCustomization.AllowSort = false;
            grdViewSLedger.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewSLedger.OptionsView.ShowAutoFilterRow = true;
            grdViewSLedger.OptionsView.ShowViewCaption = false;
            grdViewSLedger.OptionsView.ShowFooter = true;
            grdViewSLedger.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            grdViewSLedger.OptionsSelection.InvertSelection = true;
            grdViewSLedger.OptionsView.ColumnAutoWidth = true;
            grdViewSLedger.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewSLedger.Columns["Credit"].OptionsColumn.AllowIncrementalSearch = false;
            grdViewSLedger.Columns["Debit"].OptionsColumn.AllowIncrementalSearch = false;
            grdViewSLedger.Appearance.HeaderPanel.Font = new Font(grdViewSLedger.Appearance.HeaderPanel.Font, FontStyle.Bold);
            grdViewSLedger.Appearance.FooterPanel.Font = new Font(grdViewSLedger.Appearance.FooterPanel.Font, FontStyle.Bold);

            grdViewSLedger.FocusedRowHandle = 0;
            grdViewSLedger.FocusedColumn = grdViewSLedger.VisibleColumns[0];

            grdViewSLedger.Appearance.FocusedRow.BackColor = Color.Teal;
            grdViewSLedger.Appearance.FocusedRow.ForeColor = Color.White;
            grdViewSLedger.Appearance.HideSelectionRow.BackColor = Color.Teal;
            grdViewSLedger.Appearance.HideSelectionRow.ForeColor = Color.White;

        }

        private void Fill_Details_VendorType()
        {
            m_dsVType = new DataSet();
            m_dsVType = SLAnalysisBL.Get_VendorType_Details(SLAnalysisBL.VTypeId,m_dAsOn,m_sIds);
        }

        private void Fill_BillPay_Details()
        {
            decimal dOB = 0;
            if (SLAnalysisBL.VTypeId == 0)
            {
                m_dtBills= SLAnalysisBL.Get_SLType_BillDet(m_iSLTypeId,m_iSLId,m_dAsOn,m_sIds );
                dOB = SLAnalysisBL.Get_SLType_OpeningBalance(m_iSLTypeId, m_iSLId, SLAnalysisBL.CCId, m_sIds);
                DataRow newRow = m_dtBills.NewRow();
                newRow["VoucherDate"] = DBNull.Value;
                newRow["VoucherNo"] = "";
                newRow["SLTypeId"] = "-1";
                newRow["SLId"] = "-1";
                newRow["RelatedSLId"] = "-1";
                newRow["AccountId"] = "-1";
                newRow["AccountName"] = "Opening Balance";
                newRow["SubLedger"] = "";
                newRow["RelatedSubLedger"] = "";
                newRow["RefType"] = "";
                newRow["Debit"] = dOB >= 0 ? dOB : 0;
                newRow["Credit"] = dOB < 0 ? Math.Abs(dOB) : 0;

                m_dtBills.Rows.InsertAt(newRow, 0);
                grdBill.DataSource = null;
                grdViewBill.Columns.Clear();
                grdBill.DataSource = m_dtBills;

            }
            else
            {
                using (DataView dvDataNew = new DataView(m_dsVType.Tables["VTypeDet"]))
                {
                    DataTable dtDataNew = null;
                    dvDataNew.RowFilter = String.Format("SLTypeId={0} AND SLId={1} ", m_iSLTypeId, m_iSLId);
                    dtDataNew = dvDataNew.ToTable();

                    if (SLAnalysisBL.VTypeId != 4)
                    {
                        dOB = SLAnalysisBL.Get_VendorType_Balance(SLAnalysisBL.VTypeId, m_iSLId, SLAnalysisBL.CCId, m_sIds);
                        DataRow newRow = dtDataNew.NewRow();
                        newRow["VoucherDate"] = DBNull.Value;
                        newRow["VoucherNo"] = "";
                        newRow["SLTypeId"] = "-1";
                        newRow["SLId"] = "-1";
                        newRow["RelatedSLId"] = "-1";
                        newRow["AccountId"] = "-1";
                        newRow["AccountName"] = "Opening Balance";
                        newRow["SubLedger"] = "";
                        newRow["RelatedSubLedger"] = "";
                        newRow["RefType"] = "";
                        newRow["Debit"] = dOB >= 0 ? dOB : 0;
                        newRow["Credit"] = dOB < 0 ? Math.Abs(dOB) : 0;

                        dtDataNew.Rows.InsertAt(newRow, 0);
                    }
                    grdBill.DataSource = null;
                    grdViewBill.Columns.Clear();
                    grdBill.DataSource = dtDataNew;
                }
            }
            grdBill.ForceInitialize();
            grdViewBill.PopulateColumns();
            grdViewBill.Columns["AccountId"].Visible = false;
            grdViewBill.Columns["SLTypeId"].Visible = false;
            grdViewBill.Columns["SLId"].Visible = false;
            grdViewBill.Columns["RelatedSLId"].Visible = false;
            grdViewBill.Columns["SubLedger"].Visible = false;
            grdViewBill.Columns["ChequeNo"].Visible = false;
            grdViewBill.Columns["ChequeDate"].Visible = false;
            grdViewBill.Columns["Narration"].Visible = false;
            grdViewBill.Columns["CostCentre"].Visible = false;

            grdViewBill.Columns["VoucherDate"].Visible = true;
            grdViewBill.Columns["VoucherDate"].Caption = "Voucher Date";
            grdViewBill.Columns["VoucherDate"].Width = 40;

            grdViewBill.Columns["VoucherNo"].Visible = true;
            grdViewBill.Columns["VoucherNo"].Width = 40;

            grdViewBill.Columns["AccountName"].Visible = true;
            grdViewBill.Columns["AccountName"].Caption = "Account";
            grdViewBill.Columns["AccountName"].Width = 100;
            grdViewBill.Columns["AccountName"].Summary.Add(SummaryItemType.Count, "AccountName", "Total");
            grdViewBill.Columns["AccountName"].Summary.Add(SummaryItemType.Custom, "AccountName", "Balance");

            grdViewBill.Columns["RelatedAccount"].Visible = false;

            grdViewBill.Columns["RelatedSubLedger"].Visible = true;
            grdViewBill.Columns["RelatedSubLedger"].Caption = "Ref. Sub Ledger";
            grdViewBill.Columns["RelatedSubLedger"].Width = 100;
            

            grdViewBill.Columns["RefType"].Visible = true;
            grdViewBill.Columns["RefType"].Caption = "Type";
            grdViewBill.Columns["RefType"].Width = 30;
            grdViewBill.Columns["RefType"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            grdViewBill.Columns["RefType"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            grdViewBill.Columns["Debit"].Visible = true;
            grdViewBill.Columns["Debit"].Caption = "Debit";
            grdViewBill.Columns["Debit"].Width = 50;
            grdViewBill.Columns["Debit"].OptionsColumn.AllowEdit = false;
            grdViewBill.Columns["Debit"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewBill.Columns["Debit"].DisplayFormat.FormatString = clsStatic.sFormatAmt;
            grdViewBill.Columns["Debit"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            grdViewBill.Columns["Debit"].Summary.Add(SummaryItemType.Sum, "Debit", clsStatic.sFormatTotAmt);
            grdViewBill.Columns["Debit"].Summary.Add(SummaryItemType.Custom, "Debit", clsStatic.sFormatTotAmt);
            grdViewBill.Columns["Credit"].SummaryItem.Tag = 0;

            grdViewBill.Columns["Credit"].Visible = true;
            grdViewBill.Columns["Credit"].Caption = "Credit";
            grdViewBill.Columns["Credit"].Width = 50;

            grdViewBill.Columns["Credit"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewBill.Columns["Credit"].DisplayFormat.FormatString = clsStatic.sFormatAmt;
            grdViewBill.Columns["Credit"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            grdViewBill.Columns["Credit"].Summary.Add(SummaryItemType.Sum, "Credit", clsStatic.sFormatTotAmt);
            grdViewBill.Columns["Credit"].SummaryItem.Tag = 1;
            grdViewBill.Columns["Credit"].Summary.Add(SummaryItemType.Custom, "Credit", clsStatic.sFormatTotAmt);

            grdViewBill.OptionsBehavior.Editable = false;
            grdViewBill.OptionsCustomization.AllowFilter = false;
            grdViewBill.OptionsCustomization.AllowSort = false;
            grdViewBill.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewBill.OptionsView.ShowAutoFilterRow = true;
            grdViewBill.OptionsView.ShowViewCaption = false;
            grdViewBill.OptionsView.ShowFooter = true;
            grdViewBill.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            grdViewBill.OptionsSelection.InvertSelection = true;
            grdViewBill.OptionsView.ColumnAutoWidth = true;
            grdViewBill.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewBill.Columns["Credit"].OptionsColumn.AllowIncrementalSearch = false;
            grdViewBill.Columns["Debit"].OptionsColumn.AllowIncrementalSearch = false;
            grdViewBill.Appearance.HeaderPanel.Font = new Font(grdViewBill.Appearance.HeaderPanel.Font, FontStyle.Bold);
            grdViewBill.Appearance.FooterPanel.Font = new Font(grdViewBill.Appearance.FooterPanel.Font, FontStyle.Bold);

            grdViewBill.FocusedRowHandle = 0;
            grdViewBill.FocusedColumn = grdViewBill.VisibleColumns[0];

            grdViewBill.Appearance.FocusedRow.BackColor = Color.Teal;
            grdViewBill.Appearance.FocusedRow.ForeColor = Color.White;
            grdViewBill.Appearance.HideSelectionRow.BackColor = Color.Teal;
            grdViewBill.Appearance.HideSelectionRow.ForeColor = Color.White;

        }
        #endregion

        #region Grid Events

        private void grdViewTrans_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            GridView view = sender as GridView;
            decimal dDr = 0;
            decimal dCr = 0;

            if (e.Column.FieldName == "Debit")
            {
                dDr = Convert.ToDecimal(clsStatic.IsNullCheck(e.Value, datatypes.vartypenumeric));
                dCr = Convert.ToDecimal(clsStatic.IsNullCheck(view.GetRowCellValue(e.RowHandle, "Credit"), datatypes.vartypenumeric));
            }
            else if (e.Column.FieldName == "Credit")
            {
                dCr = Convert.ToDecimal(clsStatic.IsNullCheck(e.Value, datatypes.vartypenumeric));
                dDr = Convert.ToDecimal(clsStatic.IsNullCheck(view.GetRowCellValue(e.RowHandle, "Debit"), datatypes.vartypenumeric));
            }

            if (dDr == 0 && dCr == 0)
            {
                if (e.Column.FieldName == "Credit")
                {
                    e.DisplayText = null;

                }
            }
            if (dDr != 0 && dCr == 0)
            {
                if (e.Column.FieldName == "Credit")
                {
                    e.DisplayText = null;

                }
            }

            if (dCr != 0 && dDr == 0)
            {
                if (e.Column.FieldName == "Debit")
                {
                    e.DisplayText = null;

                }
            }
        }

        private void grdViewSL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
         
        }

        private void grdViewSL_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            switch (e.SummaryProcess)
            {
                case CustomSummaryProcess.Start:
                    e.TotalValue = new decimal();
                    break;
                case CustomSummaryProcess.Calculate:
                    decimal dDebit = (decimal)grdViewSLType.GetRowCellValue(e.RowHandle, "Debit");
                    decimal dCredit = (decimal)grdViewSLType.GetRowCellValue(e.RowHandle, "Credit");
                    e.TotalValue = (decimal)e.TotalValue + dDebit - dCredit;
                    break;
                case CustomSummaryProcess.Finalize:
                     if ((e.Item as GridSummaryItem).FieldName == "Debit")
                    {
                        if (e.TotalValue == null)
                            e.TotalValue = "";
                        else if ((decimal)e.TotalValue > 0)
                            e.TotalValue = Math.Abs((Decimal)e.TotalValue);
                        else
                            e.TotalValue = "";
                    }
                    if ((e.Item as GridSummaryItem).FieldName == "Credit")
                    {
                        if (e.TotalValue == null)
                            e.TotalValue = "";
                        else if ((decimal)e.TotalValue < 0)
                            e.TotalValue = Math.Abs((Decimal)e.TotalValue);
                        else
                            e.TotalValue = "";

                    }
                    break;
            }
        }

        private void grdViewDet_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            switch (e.SummaryProcess)
            {
                case CustomSummaryProcess.Start:
                    e.TotalValue = new decimal();
                    break;
                case CustomSummaryProcess.Calculate:
                    decimal dDebit = (decimal)grdViewSLedger.GetRowCellValue(e.RowHandle, "Debit");
                    decimal dCredit = (decimal)grdViewSLedger.GetRowCellValue(e.RowHandle, "Credit");
                    e.TotalValue = (decimal)e.TotalValue + dDebit - dCredit;
                    break;
                case CustomSummaryProcess.Finalize:
                   if ((e.Item as GridSummaryItem).FieldName == "Debit")
                    {
                        if (e.TotalValue == null)
                            e.TotalValue = "";
                        else if ((decimal)e.TotalValue > 0)
                            e.TotalValue = Math.Abs((Decimal)e.TotalValue);
                        else
                            e.TotalValue = "";
                    }
                    if ((e.Item as GridSummaryItem).FieldName == "Credit")
                    {
                        if (e.TotalValue == null)
                            e.TotalValue = "";
                        else if ((decimal)e.TotalValue < 0)
                            e.TotalValue = Math.Abs((Decimal)e.TotalValue);
                        else
                            e.TotalValue = "";

                    }
                    break;
            }
        }

        private void grdViewDet_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            
        }

        private void grdViewSL_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            GridView view = sender as GridView;
            decimal dDr = 0;
            decimal dCr = 0;

            if (e.Column.FieldName == "Debit")
            {
                dDr = Convert.ToDecimal(clsStatic.IsNullCheck(e.Value, datatypes.vartypenumeric));
                dCr = Convert.ToDecimal(clsStatic.IsNullCheck(view.GetRowCellValue(e.RowHandle, "Credit"), datatypes.vartypenumeric));
            }
            else if (e.Column.FieldName == "Credit")
            {
                dCr = Convert.ToDecimal(clsStatic.IsNullCheck(e.Value, datatypes.vartypenumeric));
                dDr = Convert.ToDecimal(clsStatic.IsNullCheck(view.GetRowCellValue(e.RowHandle, "Debit"), datatypes.vartypenumeric));
            }

            if (dDr == 0 && dCr == 0)
            {
                if (e.Column.FieldName == "Credit")
                {
                    e.DisplayText = null;

                }
            }
            if (dDr != 0 && dCr == 0)
            {
                if (e.Column.FieldName == "Credit")
                {
                    e.DisplayText = null;

                }
            }

            if (dCr != 0 && dDr == 0)
            {
                if (e.Column.FieldName == "Debit")
                {
                    e.DisplayText = null;

                }
            }
        }

        private void grdViewDet_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            GridView view = sender as GridView;
            decimal dDr = 0;
            decimal dCr = 0;

            if (e.Column.FieldName == "Debit")
            {
                dDr = Convert.ToDecimal(clsStatic.IsNullCheck(e.Value, datatypes.vartypenumeric));
                dCr = Convert.ToDecimal(clsStatic.IsNullCheck(view.GetRowCellValue(e.RowHandle, "Credit"), datatypes.vartypenumeric));
            }
            else if (e.Column.FieldName == "Credit")
            {
                dCr = Convert.ToDecimal(clsStatic.IsNullCheck(e.Value, datatypes.vartypenumeric));
                dDr = Convert.ToDecimal(clsStatic.IsNullCheck(view.GetRowCellValue(e.RowHandle, "Debit"), datatypes.vartypenumeric));
            }

            if (dDr == 0 && dCr == 0)
            {
                if (e.Column.FieldName == "Credit")
                {
                    e.DisplayText = null;

                }
            }
            if (dDr != 0 && dCr == 0)
            {
                if (e.Column.FieldName == "Credit")
                {
                    e.DisplayText = null;

                }
            }

            if (dCr != 0 && dDr == 0)
            {
                if (e.Column.FieldName == "Debit")
                {
                    e.DisplayText = null;

                }
            }
        }

        #endregion

        private void grdViewSL_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }

        }

        private void grdViewDet_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }

        }

        private void grdViewTrans_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }

        }

        private void grdViewSL_DoubleClick(object sender, EventArgs e)
        {
            btnNext.PerformClick();
        }

        private void grdViewDet_DoubleClick(object sender, EventArgs e)
        {
            btnNext.PerformClick();
        }

        private void btnNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_iLevel == 0)
            {
                if (grdViewSLType.FocusedRowHandle < 0) return;
                grdBill.DataSource = null;
                m_sGeneral = grdViewSLType.GetRowCellValue(grdViewSLType.FocusedRowHandle, "SLTypeName").ToString();
                m_iSLTypeId = Convert.ToInt32(grdViewSLType.GetRowCellValue(grdViewSLType.FocusedRowHandle, "SLTypeId").ToString());
                cboVendorType.EditValue = "(ALL)";
                Fill_Account_Type();
                Fill_TransDet();
                dwSubLedger.Show();
                dwSubLedger.Select();
                dwSubLedger.Text = "Sub Ledger Details : " + m_sGeneral;
                btnPrev.Enabled = true;
                m_iLevel = 1;
            }
            else if (m_iLevel == 1)
            {
                if (grdViewSLedger.FocusedRowHandle < 0) return;
                m_sTransName = grdViewSLedger.GetRowCellValue(grdViewSLedger.FocusedRowHandle, "SLName").ToString();
                m_iSLId = Convert.ToInt32(grdViewSLedger.GetRowCellValue(grdViewSLedger.FocusedRowHandle, "SLId").ToString());
                Fill_BillPay_Details();
                dwBillDetails.Show();
                dwBillDetails.Select();
                dwBillDetails.Text = "Transaction Details : " + m_sTransName;
                btnPrev.Enabled = true;
                m_iLevel = 2;
            }
        }

        private void Fill_Account_Type()
        {
            m_bLoad = true;
            string sIds = string.Empty;
            using (DataView dvDataNew = new DataView(m_dtAccType))
            {
                DataTable dtDataNew = null;
                dvDataNew.RowFilter = String.Format("SLTypeId='{0}'", m_iSLTypeId);
                dtDataNew = dvDataNew.ToTable();
                repcboAccType.DataSource = null;
                repcboAccType.NullText = "-- Select Account Type --";
                repcboAccType.DataSource = dtDataNew;
                repcboAccType.DisplayMember = "TypeName";
                repcboAccType.ValueMember = "TypeId";
                repcboAccType.AllowMultiSelect = false;
                cboAccountType.EditValue = null;

                for (int i = 0; i < dtDataNew.Rows.Count; i++)
                {
                    sIds = String.Format("{0}{1},", sIds, dtDataNew.Rows[i]["TypeId"]);
                }
            }
            if (sIds != string.Empty) 
            {
                sIds = sIds.TrimEnd(',');
                cboAccountType.EditValue=sIds;
            }
            
            m_bLoad = false;
        }

        private void btnPrev_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_iLevel == 2)
            {
                dwBillDetails.Hide();
                dwSLDet.Show();
                dwSLDet.Select();
                btnPrev.Enabled = true;
                m_iLevel = 1;
            }
            else if (m_iLevel == 1)
            {
                dwSLDet.Hide();
                dwSLType.Show();
                dwSLType.Select();
                btnPrev.Enabled = false;
                m_iLevel = 0;
            }
        }

        private void grdViewTrans_DoubleClick(object sender, EventArgs e)
        {

        }

        private void radDock1_ActiveWindowChanged(object sender, Telerik.WinControls.UI.Docking.DockWindowEventArgs e)
        {
            if (radDock1.ActiveWindow.Name == "dwSLType" && m_iLevel != 0)
            {
                m_iLevel = 0;
                dwSubLedger.Hide();
                dwBillDetails.Hide();
                grdViewSLType.Focus();
                dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            }
            else if (radDock1.ActiveWindow.Name == "dwSubLedger" && m_iLevel != 1)
            {
                m_iLevel = 1;
                dwBillDetails.Hide();
                grdViewSLedger.Focus();
                dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            }
        }

        private void cboVendorType_EditValueChanged(object sender, EventArgs e)
        {

            if (cboVendorType.EditValue.ToString() == ("(ALL)"))
                SLAnalysisBL.VTypeId = 0;
            else if (cboVendorType.EditValue.ToString() == ("Supply"))
                SLAnalysisBL.VTypeId = 1;
            else if (cboVendorType.EditValue.ToString() == ("Works"))
                SLAnalysisBL.VTypeId = 2;
            else if (cboVendorType.EditValue.ToString() == ("Service"))
                SLAnalysisBL.VTypeId = 3;
            else
                SLAnalysisBL.VTypeId = 4;

            Fill_TransDet();
        }

        private void grdViewBill_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            switch (e.SummaryProcess)
            {
                case CustomSummaryProcess.Start:
                    e.TotalValue = new decimal();
                    break;
                case CustomSummaryProcess.Calculate:
                    decimal dDebit = (decimal)grdViewBill.GetRowCellValue(e.RowHandle, "Debit");
                    decimal dCredit = (decimal)grdViewBill.GetRowCellValue(e.RowHandle, "Credit");
                    e.TotalValue = (decimal)e.TotalValue + dDebit - dCredit;
                    break;
                case CustomSummaryProcess.Finalize:
                    if ((e.Item as GridSummaryItem).FieldName == "Debit")
                    {
                        if (e.TotalValue == null)
                            e.TotalValue = "";
                        else if ((decimal)e.TotalValue > 0)
                            e.TotalValue = Math.Abs((Decimal)e.TotalValue);
                        else
                            e.TotalValue = "";
                    }
                    if ((e.Item as GridSummaryItem).FieldName == "Credit")
                    {
                        if (e.TotalValue == null)
                            e.TotalValue = "";
                        else if ((decimal)e.TotalValue < 0)
                            e.TotalValue = Math.Abs((Decimal)e.TotalValue);
                        else
                            e.TotalValue = "";

                    }
                    break;
            }
        }

        private void grdViewSLType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btnNext.PerformClick();
            }
           
        }

        private void grdViewSLedger_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btnNext.PerformClick();
            }
            else if (e.KeyChar == (char)27)
            {
                btnPrev.PerformClick();
            }
        }

        private void grdViewBill_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                btnPrev.PerformClick();
            }
        }

        private void cboAccountType_EditValueChanged(object sender, EventArgs e)
        {
            int iCnt=0;
            if (m_bLoad == true) return;
            m_sIds = string.Empty;
            for (int i = 0; i < repcboAccType.Items.Count; i++)
            {
                if (repcboAccType.Items[i].CheckState == CheckState.Checked) 
                    iCnt += 1;
            }
            
            if (cboAccountType.EditValue!=null)
            {
                m_sIds = cboAccountType.EditValue.ToString();
            }
            if (m_sIds != string.Empty)
            {
                Fill_TransDet();  
            }
            
        }

        private void grdViewBill_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (grdViewBill.FocusedRowHandle <= 0) return;
            dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            string sText = string.Empty;
            txtNarration.Text = "";
            sText += String.Format("Cheque No: {0}{1}", grdViewBill.GetRowCellValue(e.FocusedRowHandle, "ChequeNo"), Environment.NewLine);
            sText += String.Format("Cheque Date: {0}{1}", grdViewBill.GetRowCellValue(e.FocusedRowHandle, "ChequeDate"), Environment.NewLine);
            sText += String.Format("Cost Centre: {0}{1}", grdViewBill.GetRowCellValue(e.FocusedRowHandle, "CostCentre"), Environment.NewLine);
            sText += String.Format("Narration : {0}", grdViewBill.GetRowCellValue(e.FocusedRowHandle, "Narration"));
            txtNarration.Text = sText;
        }
    }
}
