using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FA.BusinessLayer;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Mask;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;

namespace FA
{
    public partial class frmBRSPrevYr : DevExpress.XtraEditors.XtraForm
    {
        #region Variable
        DataSet m_dsData = new DataSet();
        DataTable m_dtBook;
        DataTable m_dtAccHeads;
        DataTable m_dtAccount;
        DataTable m_dtSLAccType=new DataTable();
        DataTable m_dtLedgerType;
        DataTable m_dtFromCC;
        int m_iAccTypeId;
        DataTable dtData;
        DataTable dtSLType;
        DataTable dtSLName;
        DataTable m_dtBRSReg;
        RepositoryItemLookUpEdit cboSLType;
        RepositoryItemLookUpEdit repSubLedger;
        RepositoryItemRadioGroup radioType;
        int iTypeId;
        int m_iEntryId = 0;
        bool m_bDW = false;
        string m_sBookName = "";
        int m_iBookId = 0;
        RepositoryItemLookUpEdit cboAccHead = new RepositoryItemLookUpEdit();

        #endregion

        #region Constructor
        public frmBRSPrevYr()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load
        private void frmBRSPrevYr_Load(object sender, EventArgs e)
        {
            dwBRSReg.Select();
            dwBRS.Hide();
            PopulateDefaultData();
            dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockRemarks.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
         
        }
        #endregion

        #region Functions

        public void Execute(string arg_sBookName,int arg_iBookId)
        {
            m_iBookId = arg_iBookId;
            m_sBookName = arg_sBookName;
            Show();
        }
        private void PopulateDefaultData()
        {
       
            clsStatic.SetMyGraphics();

            m_dtBook = new DataTable();
             m_dtAccount = new DataTable();
            m_dtAccHeads = new DataTable();
            m_dtLedgerType = new DataTable();
            m_dtFromCC = new DataTable();

            m_dsData = EntryBL.GetEntryInfo();
            m_dtSLAccType = EntryBL.Get_SLAccType();

            m_dtBook = m_dsData.Tables[0];
            m_dtAccount = m_dsData.Tables[1];
            DataView dv = new DataView(m_dtAccount);
            dv.RowFilter = "AccountType<>'BA' AND AccountType<>'CA'";
            m_dtAccHeads = dv.ToTable();
            m_dtFromCC = m_dsData.Tables[3];

            DataView dvLType = new DataView(m_dsData.Tables[2]) { RowFilter = "SubLedgerType <> 'M' and SubLedgerType<>'W'" };
            m_dtLedgerType = dvLType.ToTable();
            Fill_VGrid_BRS();
            Fill_BRS_Register();
           
        }

        private void Fill_BRS_Register()
        {
            m_dtBRSReg = new DataTable();
            m_dtBRSReg = EntryBL.Get_BRSRegister_Det(m_iBookId);

            grdBRSReg.DataSource = m_dtBRSReg;
            grdBRSReg.ForceInitialize();
            grdViewBRSReg.PopulateColumns();

            grdViewBRSReg.Columns["ChequeNo"].Visible = false;
            grdViewBRSReg.Columns["ChequeDate"].Visible = false;
            grdViewBRSReg.Columns["Narration"].Visible = false;
            grdViewBRSReg.Columns["EntryId"].Visible = false;

            grdViewBRSReg.Columns["VoucherDate"].Visible = true;
            grdViewBRSReg.Columns["VoucherDate"].Caption = "VoucherDate";
            grdViewBRSReg.Columns["VoucherDate"].Width = 50;
            grdViewBRSReg.Columns["VoucherDate"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Near;
            grdViewBRSReg.Columns["VoucherDate"].OptionsColumn.AllowEdit = false;

            grdViewBRSReg.Columns["VoucherNo"].Visible = true;
            grdViewBRSReg.Columns["VoucherNo"].Caption = "VoucherNo";
            grdViewBRSReg.Columns["VoucherNo"].Width = 50;
            grdViewBRSReg.Columns["VoucherNo"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Near;
            grdViewBRSReg.Columns["VoucherNo"].OptionsColumn.AllowEdit = false;

            grdViewBRSReg.Columns["AccountName"].Visible = true;
            grdViewBRSReg.Columns["AccountName"].Caption = "AccountName";
            grdViewBRSReg.Columns["AccountName"].Width = 100;
            grdViewBRSReg.Columns["AccountName"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Near;
            
            grdViewBRSReg.Columns["AccountName"].OptionsColumn.AllowEdit = false;

            grdViewBRSReg.Columns["SubledgerName"].Visible = true;
            grdViewBRSReg.Columns["SubledgerName"].Caption = "SubledgerName";
            grdViewBRSReg.Columns["SubledgerName"].Width = 100;
            grdViewBRSReg.Columns["SubledgerName"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Near;
            grdViewBRSReg.Columns["SubledgerName"].OptionsColumn.AllowEdit = false;

            grdViewBRSReg.Columns["Debit"].Visible = true;
            grdViewBRSReg.Columns["Debit"].Caption = "Debit";
            grdViewBRSReg.Columns["Debit"].Width = 50;
            grdViewBRSReg.Columns["Debit"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far;
            grdViewBRSReg.Columns["Debit"].OptionsColumn.AllowEdit = false;

            grdViewBRSReg.Columns["Credit"].Visible = true;
            grdViewBRSReg.Columns["Credit"].Caption = "Credit";
            grdViewBRSReg.Columns["Credit"].Width = 50;
            grdViewBRSReg.Columns["Credit"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far;
            grdViewBRSReg.Columns["Credit"].OptionsColumn.AllowEdit = false;

            grdViewBRSReg.Columns["BRS"].Visible = true;
            RepositoryItemCheckEdit chkSel = new RepositoryItemCheckEdit();
            grdViewBRSReg.Columns["BRS"].Caption = "BRS";
            grdViewBRSReg.Columns["BRS"].Width = 50;
            grdViewBRSReg.Columns["BRS"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            grdViewBRSReg.Columns["BRS"].OptionsColumn.AllowEdit = true;
            grdViewBRSReg.Columns["BRS"].ColumnEdit = chkSel;
            chkSel.CheckedChanged += new EventHandler(chkSel_CheckedChanged);

            RepositoryItemDateEdit dtpVoucherDate = new RepositoryItemDateEdit();
            
            grdViewBRSReg.Columns["BRSDate"].Visible = true;
            grdViewBRSReg.Columns["BRSDate"].Caption = "BRSDate";
            grdViewBRSReg.Columns["BRSDate"].Width = 50;
            grdViewBRSReg.Columns["BRSDate"].AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Near;
            grdViewBRSReg.Columns["BRSDate"].OptionsColumn.AllowEdit = true;
            grdViewBRSReg.Columns["BRSDate"].ColumnEdit = dtpVoucherDate;
           

            grdViewBRSReg.OptionsCustomization.AllowFilter = false;
            grdViewBRSReg.OptionsCustomization.AllowSort = false;
            grdViewBRSReg.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewBRSReg.OptionsView.ShowViewCaption = false;
            grdViewBRSReg.OptionsView.ShowFooter = true;
            grdViewBRSReg.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            grdViewBRSReg.OptionsSelection.InvertSelection = true;
            grdViewBRSReg.OptionsView.ColumnAutoWidth = true;
            grdViewBRSReg.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewBRSReg.OptionsBehavior.Editable = true;
            grdViewBRSReg.Appearance.HeaderPanel.Font = new Font(grdViewBRSReg.Appearance.HeaderPanel.Font, FontStyle.Bold);
            grdViewBRSReg.Appearance.FooterPanel.Font = new Font(grdViewBRSReg.Appearance.FooterPanel.Font, FontStyle.Bold);

            grdViewBRSReg.Appearance.FocusedRow.BackColor = Color.Teal;
            grdViewBRSReg.Appearance.FocusedRow.ForeColor = Color.White;
            grdViewBRSReg.Appearance.HideSelectionRow.BackColor = Color.Teal;
            grdViewBRSReg.Appearance.HideSelectionRow.ForeColor = Color.White;
        }

        void chkSel_CheckedChanged(object sender, EventArgs e)
        {
            var editor = (CheckEdit)sender;
            DateTime dtVoucherDate = Convert.ToDateTime(grdViewBRSReg.GetRowCellValue(grdViewBRSReg.FocusedRowHandle, "VoucherDate"));

            DateTime dtBRSDate = Convert.ToDateTime(DateTime.Now.ToString("dd/MMM/yyyy"));

            if (editor.Checked == true)
            {
                if (dtBRSDate >= dtVoucherDate)
                {
                    grdViewBRSReg.SetRowCellValue(grdViewBRSReg.FocusedRowHandle, "BRSDate", dtBRSDate);
                }
                else
                {
                    MessageBox.Show("BRS Date Should be Greater then VoucherDate", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
               
            }
            else
            {
                grdViewBRSReg.SetRowCellValue(grdViewBRSReg.FocusedRowHandle, "BRSDate", DBNull.Value);
            }
            grdViewBRSReg.CloseEditor();
        }

        #endregion

        #region Button Events
        private void btnOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            if (vGridBRS.Rows["BookName"].Properties.Value == null)
            {
                MessageBox.Show("Select Book", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["BookName"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["Type"].Properties.Value == null)
            {
                MessageBox.Show("Select Type", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["Type"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["VoucherDate"].Properties.Value.ToString() == "")
            {
                vGridBRS.FocusedRow = vGridBRS.Rows["VoucherDate"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["VoucherNo"].Properties.Value == null)
            {
                MessageBox.Show("Enter VoucherNo", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["VoucherNo"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["Account"].Properties.Value==null)
            {
                MessageBox.Show("Select Account", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["Account"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["SLType"].Properties.Value==null)
            {
                MessageBox.Show("Select Sub Ledger Type", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["SLType"];
                vGridBRS.Focus();
                return;

            }
            if (vGridBRS.Rows["SubLedger"].Properties.Value == null)
            {
                MessageBox.Show("Select SubLedger", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["SubLedger"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["CostCentre"].Properties.Value == null)
            {
                MessageBox.Show("Select CostCentre", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["CostCentre"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["ChequeNo"].Properties.Value == null)
            {
                MessageBox.Show("Enter ChequeNo", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["ChequeNo"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["ChequeDate"].Properties.Value == null)
            {
                MessageBox.Show("Select ChequeDate", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["ChequeDate"];
                vGridBRS.Focus();
                return;
            }
            if (vGridBRS.Rows["Amount"].Properties.Value == null)
            {
                MessageBox.Show("Enter Amount", "FA", MessageBoxButtons.OK);
                vGridBRS.FocusedRow = vGridBRS.Rows["Amount"];
                vGridBRS.Focus();
                return;
            }

            BRSPrevYrBO.Book = m_iBookId;
            if (Convert.ToInt32(vGridBRS.Rows["Type"].Properties.Value) == 0)
            {
                BRSPrevYrBO.Type = 'P';
            }
            else if (Convert.ToInt32(vGridBRS.Rows["Type"].Properties.Value) == 1)
            {
                BRSPrevYrBO.Type = 'R';
            }
            else
                BRSPrevYrBO.Type = 'T';

            BRSPrevYrBO.VoucherDate = Convert.ToDateTime(clsStatic.IsNullCheck(vGridBRS.Rows["VoucherDate"].Properties.Value, datatypes.VarTypeDate));
            BRSPrevYrBO.VoucherNo = Convert.ToString(clsStatic.IsNullCheck(vGridBRS.Rows["VoucherNo"].Properties.Value,datatypes.vartypestring));
            BRSPrevYrBO.Account = Convert.ToInt32(clsStatic.IsNullCheck(vGridBRS.Rows["Account"].Properties.Value,datatypes.vartypenumeric));
            BRSPrevYrBO.SubLedgerType = Convert.ToInt32(vGridBRS.Rows["SLType"].Properties.Value);
            BRSPrevYrBO.SubLedger = Convert.ToInt32(vGridBRS.Rows["SubLedger"].Properties.Value);
            BRSPrevYrBO.CostCentre = Convert.ToInt32(clsStatic.IsNullCheck(vGridBRS.Rows["CostCentre"].Properties.Value,datatypes.vartypenumeric));
            BRSPrevYrBO.Amount = Convert.ToDecimal(clsStatic.IsNullCheck(vGridBRS.Rows["Amount"].Properties.Value,datatypes.vartypenumeric));
            BRSPrevYrBO.ChequeNo = Convert.ToString(clsStatic.IsNullCheck(vGridBRS.Rows["ChequeNo"].Properties.Value,datatypes.vartypestring));
            BRSPrevYrBO.ChequeDate = Convert.ToDateTime(clsStatic.IsNullCheck(vGridBRS.Rows["ChequeDate"].Properties.Value,datatypes.VarTypeDate));
            BRSPrevYrBO.Remarks = Convert.ToString(clsStatic.IsNullCheck(vGridBRS.Rows["Remarks"].Properties.Value, datatypes.vartypestring));

            if (m_iEntryId == 0)
            {
                EntryBL.Update_BRS_PervYr(m_iEntryId);
                Clear_BRS_VGrid();
                Fill_BRS_Register();
                dwBRSReg.Show();
                dwBRSReg.Select();
            }
            else
            {
                EntryBL.Update_BRS_PervYr(m_iEntryId);
                Fill_BRS_Register();
                this.Close();
            }
            grdViewBRSReg.FocusedRowHandle = 0;
        }

        private void Clear_BRS_VGrid()
        {
            vGridBRS.Rows["BookName"].Properties.Value = "--Select Book--";
            vGridBRS.Rows["Type"].Properties.Value = string.Empty;
            vGridBRS.Rows["VoucherDate"].Properties.Value = string.Empty;
            vGridBRS.Rows["VoucherNo"].Properties.Value = string.Empty;
            vGridBRS.Rows["Account"].Properties.Value = string.Empty;
            vGridBRS.Rows["SLType"].Properties.Value = string.Empty;
            vGridBRS.Rows["SubLedger"].Properties.Value = string.Empty;
            vGridBRS.Rows["CostCentre"].Properties.Value = string.Empty;
            vGridBRS.Rows["Amount"].Properties.Value = string.Empty;
            vGridBRS.Rows["ChequeNo"].Properties.Value = string.Empty;
            vGridBRS.Rows["ChequeDate"].Properties.Value = string.Empty;
            vGridBRS.Rows["Remarks"].Properties.Value = string.Empty;
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_iEntryId != 0)
            {
                this.Close();
            }
            else
            {
                Clear_BRS_VGrid();
            }
        }

        #endregion

        #region Vertical Grid
        private void Fill_VGrid_BRS()
        {
            vGridBRS.Rows.Clear();

            #region BookName
            EditorRow editorRow1 = new EditorRow();
            editorRow1 = new EditorRow();
            editorRow1.Name = "BookName";
            editorRow1.Properties.Caption = "Book Name";
            //RepositoryItemLookUpEdit cboBook = new RepositoryItemLookUpEdit();

            //using (DataView dv = new DataView(m_dtBook))
            //{
            //    DataTable dtBook = new DataTable();
            //    dv.RowFilter = "AccountType='BA'";

            //    dtBook = dv.ToTable();
            //    cboBook.DataSource = null;
            //    cboBook.NullText = "-- Select Book --";

            //    if (dtBook != null)
            //    {
            //        cboBook.DataSource = dtBook;
            //        cboBook.DisplayMember = "AccountName";
            //        cboBook.ValueMember = "AccountID";
            //        cboBook.PopulateColumns();
            //        cboBook.Columns["AccountID"].Visible = false;
            //        cboBook.Columns["AccountType"].Visible = false;
            //        cboBook.ShowFooter = false;
            //        cboBook.ShowHeader = false;
                    
            //    }
            //}
            //cboBook.EditValueChanged += new EventHandler(cboBook_EditValueChanged);
            //editorRow1.Properties.RowEdit = cboBook;
            editorRow1.Properties.Value = m_sBookName;
            editorRow1.Properties.ReadOnly = true;
            vGridBRS.Rows.Add(editorRow1);
           
            #endregion

            #region Type

            editorRow1 = new EditorRow();
            editorRow1.Name = "Type";
            editorRow1.Properties.Caption = "Type";

            
            radioType = new RepositoryItemRadioGroup();
            vGridBRS.RepositoryItems.Add(radioType);
            radioType.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "Payment", true));
            radioType.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(1,"Receipt",true));
            radioType.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(2, "Transfer", true));
            editorRow1.Properties.Value = 0;
            editorRow1.Properties.RowEdit = radioType;
            radioType.SelectedIndexChanged += new EventHandler(radioType_SelectedIndexChanged);
            vGridBRS.Rows.Add(editorRow1);

            vGridBRS.FocusedRow = vGridBRS.Rows["Type"];
            #endregion

            #region V. Date
            editorRow1 = new EditorRow() { Name = "VoucherDate" };
            editorRow1.Properties.Caption = "Voucher Date ";
            RepositoryItemDateEdit dtpVDate = new RepositoryItemDateEdit();
            dtpVDate.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
            dtpVDate.Mask.EditMask = "dd/MM/yyyy";
            dtpVDate.MaxValue = BsfGlobal.g_dStartDate.AddDays(-1);
            dtpVDate.MinValue = BsfGlobal.g_dStartDate.AddYears(-1);


            editorRow1.Properties.RowEdit = dtpVDate;
            dtpVDate.EditValueChanged += new EventHandler(dtpVDate_EditValueChanged);

            editorRow1.Properties.Format.FormatType = FormatType.DateTime;
            editorRow1.Properties.Format.FormatString = "dd/MM/yyyy";
            editorRow1.Properties.Value = BsfGlobal.g_dStartDate.AddDays(-1);
          
            vGridBRS.Rows.Add(editorRow1);
            #endregion

            #region V. No
            editorRow1 = new EditorRow() { Name = "VoucherNo" };
            editorRow1.Properties.Caption = "Voucher No" ;
            RepositoryItemTextEdit txtVNo = new RepositoryItemTextEdit();
            txtVNo.EditValueChanged += new EventHandler(txtVNo_EditValueChanged);
            editorRow1.Properties.RowEdit = txtVNo;
            vGridBRS.Rows.Add(editorRow1);
            #endregion

            #region Account Head
           
                editorRow1 = new EditorRow();
                editorRow1.Name = "Account";
                editorRow1.Properties.Caption = "Account Head ";
                cboAccHead = new RepositoryItemLookUpEdit();
                cboAccHead.DataSource = null;
                cboAccHead.NullText = "-- Select Account Head --";
                cboAccHead.DataSource = m_dtAccHeads;
                cboAccHead.PopulateColumns();
                cboAccHead.DisplayMember = "AccountName";
                cboAccHead.ValueMember = "AccountId";
                cboAccHead.Columns["AccountId"].Visible = false;
                cboAccHead.Columns["AccountType"].Visible = false;
                cboAccHead.Columns["TypeId"].Visible = false;
                cboAccHead.ShowFooter = false;
                cboAccHead.ShowHeader = false;

                cboAccHead.EditValueChanged += new EventHandler(cboAccHead_EditValueChanged);
                editorRow1.Properties.RowEdit = cboAccHead;
                vGridBRS.Rows.Add(editorRow1);
            #endregion

            #region Sub Ledger Type
             
            editorRow1 = new EditorRow();
            editorRow1.Name = "SLType";
            editorRow1.Properties.Caption = "Sub Ledger Type ";

            RepositoryItemLookUpEdit cboSLType = new RepositoryItemLookUpEdit();
            cboSLType.NullText = "--Select Sub Ledger Type--";
            cboSLType.DataSource = null;
            DataTable dtSLType = m_dtLedgerType.Copy();
            dtSLType = clsStatic.AddSelectToDataTable(dtSLType, "None");
            cboSLType.DataSource = dtSLType;
            cboSLType.ForceInitialize();
            cboSLType.PopulateColumns();
            cboSLType.DisplayMember = "SubLedgerTypeName";
            cboSLType.ValueMember = "SubLedgerTypeId";
            cboSLType.Columns["SubLedgerTypeId"].Visible = false;
            cboSLType.Columns["SubLedgerType"].Visible = false;
            cboSLType.ShowFooter = false;
            cboSLType.ShowHeader = false;
          
            cboSLType.EditValueChanged += new EventHandler(cboSLType_EditValueChanged);
            editorRow1.Properties.RowEdit = cboSLType;
            vGridBRS.Rows.Add(editorRow1);

            #endregion

            #region SubLedger
            editorRow1 = new EditorRow();
            editorRow1.Name = "SubLedger";
            editorRow1.Properties.Caption = "Sub Ledger ";
            repSubLedger = new RepositoryItemLookUpEdit();
            repSubLedger.NullText = "--Select Sub Ledger--";
            DataTable dtSLName = m_dsData.Tables[4].Copy();
            dtSLName = clsStatic.AddSelectToDataTable(dtSLName, "None");
            repSubLedger.DataSource = dtSLName;
            repSubLedger.ForceInitialize();
            repSubLedger.PopulateColumns();
            repSubLedger.DisplayMember = "SubLedgerName";
            repSubLedger.ValueMember = "SubLedgerId";
            repSubLedger.Columns["SubLedgerId"].Visible = false;
            repSubLedger.Columns["SubLedgerTypeId"].Visible = false;
            repSubLedger.Columns["RefId"].Visible = false;
            repSubLedger.ShowFooter = false;
            repSubLedger.ShowHeader = false;
            editorRow1.Properties.RowEdit = repSubLedger;
            vGridBRS.Rows.Add(editorRow1);
            #endregion

            #region Cost Centre

            editorRow1 = new EditorRow();
            editorRow1.Name = "CostCentre";
            editorRow1.Properties.Caption = "Cost Centre ";
            RepositoryItemLookUpEdit cboCostCentre = new RepositoryItemLookUpEdit();
            cboCostCentre.NullText = "-- Select Cost Centre --";
            cboCostCentre.DataSource = null;
            DataTable m_dtFromCC = m_dsData.Tables[3].Copy();
            m_dtFromCC = clsStatic.AddSelectToDataTable(m_dtFromCC, "None");
            cboCostCentre.DataSource = m_dtFromCC;
            cboCostCentre.ForceInitialize();
            cboCostCentre.PopulateColumns();
            cboCostCentre.DisplayMember = "CostCentreName";
            cboCostCentre.ValueMember = "CostCentreId";
            cboCostCentre.Columns["CostCentreId"].Visible = false;
            cboCostCentre.Columns["StateId"].Visible = false;
            cboCostCentre.ShowFooter = false;
            cboCostCentre.ShowHeader = false;
            m_dtFromCC.Dispose();
            editorRow1.Properties.RowEdit = cboCostCentre;
            cboCostCentre.EditValueChanged += new EventHandler(cboCostCentre_EditValueChanged);
            vGridBRS.Rows.Add(editorRow1);
            #endregion

            #region Cheque No
            
            editorRow1 = new EditorRow() { Name = "ChequeNo" };
            editorRow1.Properties.Caption = "Cheque No";
            RepositoryItemTextEdit txtCheqNo = new RepositoryItemTextEdit();
            txtCheqNo.EditValueChanged += new EventHandler(txtCheqNo_EditValueChanged);
            editorRow1.Properties.RowEdit = txtCheqNo;
            vGridBRS.Rows.Add(editorRow1);
            #endregion

            #region Cheque Date
            
            editorRow1 = new EditorRow() { Name = "ChequeDate" };
            editorRow1.Properties.Caption = "Cheque Date";
            RepositoryItemDateEdit dtpChequeDate = new RepositoryItemDateEdit();
            editorRow1.Properties.RowEdit = dtpChequeDate;
            dtpChequeDate.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
            dtpChequeDate.Mask.EditMask = "dd/MM/yyyy";
            editorRow1.Properties.Format.FormatType = FormatType.DateTime;
            editorRow1.Properties.Format.FormatString = "dd/MM/yyyy";
            editorRow1.Properties.Value = DateTime.Now;
            dtpChequeDate.EditValueChanged += new EventHandler(dtpChequeDate_EditValueChanged);

            editorRow1.Properties.RowEdit = dtpVDate;
            vGridBRS.Rows.Add(editorRow1);
            #endregion

            #region Amount
            editorRow1 = new EditorRow() { Name = "Amount" };
            editorRow1.Properties.Caption = "Amount";
            RepositoryItemTextEdit txtAmount = new RepositoryItemTextEdit();
            editorRow1.Properties.RowEdit = txtAmount;
            txtAmount.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtAmount.Mask.EditMask =clsStatic.g_iCurrencyDigit == 3 ? "############.000" : "############.00";
            txtAmount.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtAmount.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtAmount_Spin);
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = clsStatic.sFormatAmt;
            txtAmount.EditValueChanged += new EventHandler(txtAmount_EditValueChanged);
            vGridBRS.Rows.Add(editorRow1);

            #endregion

            #region Remarks
            editorRow1 = new EditorRow() { Name = "Remarks"};
            editorRow1.Properties.Caption = "Remarks";
            RepositoryItemMemoEdit txtRemarks = new RepositoryItemMemoEdit();
            editorRow1.Properties.RowEdit = txtRemarks;
            editorRow1.Properties.Row.Height = 100;
            txtRemarks.EditValueChanged += new EventHandler(txtRemarks_EditValueChanged);
            vGridBRS.Rows.Add(editorRow1);
            #endregion

            vGridBRS.Appearance.RecordValue.TextOptions.HAlignment = HorzAlignment.Near;

        }

        void radioType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = (sender as DevExpress.XtraEditors.RadioGroup).SelectedIndex;
            vGridBRS.Rows["Type"].Properties.Value = selectedIndex;

             DataView dv = new DataView(m_dtAccount);
             vGridBRS.Rows["Account"].Properties.Value = null;
             if (selectedIndex == 0 || selectedIndex == 1)
             {
                 dv.RowFilter = "AccountType<>'BA' AND AccountType<>'CA'";
                 m_dtAccHeads = dv.ToTable();
             }
             else
             {
                 dv.RowFilter = "((AccountType='BA' OR AccountType='CA') AND AccountId<>" + m_iBookId + ")";
                 m_dtAccHeads = dv.ToTable();
                 
             }
             cboAccHead.DataSource = null;
             cboAccHead.NullText = "-- Select Account Head --";
             cboAccHead.DataSource = m_dtAccHeads;
             cboAccHead.ForceInitialize();
             cboAccHead.PopulateColumns();
             cboAccHead.Columns["AccountId"].Visible = false;
             cboAccHead.Columns["AccountType"].Visible = false;
             cboAccHead.Columns["TypeId"].Visible = false;
        }

        void txtAmount_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBRS.Rows["Amount"].Properties.Value = editor.EditValue;
        }

        void txtCheqNo_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBRS.Rows["ChequeNo"].Properties.Value = editor.EditValue;
        }

        void cboBook_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.LookUpEdit editor = (DevExpress.XtraEditors.LookUpEdit)sender;
            vGridBRS.Rows["BookName"].Properties.Value = editor.EditValue;
        }

        void txtAmount_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region Editvalue Changed Event
       
        void cboSLType_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.LookUpEdit editor = (DevExpress.XtraEditors.LookUpEdit)sender;
            
            DataRowView row = editor.Properties.GetDataSourceRowByKeyValue(editor.EditValue) as DataRowView;
            if (row != null)
            {
        
                iTypeId = Convert.ToInt32(row["SubLedgerTypeId"]);
                vGridBRS.Rows["SLType"].Properties.Value = editor.EditValue;
            }
            else
            {
                iTypeId = 0;
            }
            vGridBRS.Rows["SubLedger"].Properties.Value = null;
            using (DataView dv = new DataView(m_dsData.Tables[4]) { RowFilter = "SubLedgerTypeId = " + iTypeId })
            {
                dtSLName = dv.ToTable();
                if (dtSLName.Rows.Count==0)
                    dtSLName = clsStatic.AddSelectToDataTable(dtSLName, "None");
            }
          
          
        }

        void txtRemarks_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBRS.Rows["Remarks"].Properties.Value = editor.EditValue;

        }

        void txtVNo_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBRS.Rows["VoucherNo"].Properties.Value = editor.EditValue;
        }
        void cboCostCentre_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.LookUpEdit editor = (DevExpress.XtraEditors.LookUpEdit)sender;
            vGridBRS.Rows["CostCentre"].Properties.Value = editor.EditValue;
        }

        void dtpChequeDate_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.DateEdit editor = (DevExpress.XtraEditors.DateEdit)sender;
            vGridBRS.Rows["ChequeDate"].Properties.Value = editor.EditValue;
        }

        void dtpVDate_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.DateEdit editor = (DevExpress.XtraEditors.DateEdit)sender;
            vGridBRS.Rows["VoucherDate"].Properties.Value = editor.EditValue;
        }

        void cboAccHead_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.LookUpEdit editor = (DevExpress.XtraEditors.LookUpEdit)sender;
            DataRowView row = editor.Properties.GetDataSourceRowByKeyValue(editor.EditValue) as DataRowView;
            if (row != null)
            {
                m_iAccTypeId =Convert.ToInt32(row["TypeId"].ToString());
            }
            vGridBRS.Rows["SLType"].Properties.Value = null;
            vGridBRS.Rows["SubLedger"].Properties.Value = null;
        }

        void radioType_EditValueChanged(object sender, EventArgs e)
        {
            int selectedIndex = (sender as DevExpress.XtraEditors.RadioGroup).SelectedIndex;
            if (selectedIndex == 1)
            {
                vGridBRS.Rows["Type"].Properties.Value = selectedIndex;
            }
            else
            {
                vGridBRS.Rows["Type"].Properties.Value = 0;
            }
            //int selectedIndex = (sender as DevExpress.XtraEditors.RadioGroup).SelectedIndex;
            //////DevExpress.XtraEditors.RadioGroup editor = (DevExpress.XtraEditors.RadioGroup)sender;
            //object currentValue = (sender as DevExpress.XtraEditors.RadioGroup).EditValue;
            //vGridBRS.Rows["Type"].Properties.Value = selectedIndex;
        }
        #endregion

        #region Gridview Shown Event
        private void vGridBRS_ShownEditor(object sender, EventArgs e)
        {
            cboSLType = new RepositoryItemLookUpEdit();

            dtData = new DataTable();
            dtSLType = new DataTable();
            string sIds = string.Empty;

            using (DataView dvData = new DataView(m_dtSLAccType) { RowFilter = String.Format("TypeId = {0}", m_iAccTypeId) })
            {
                dtData = dvData.ToTable();
            }
            
            foreach (DataRow dr in dtData.Rows)
            {
                sIds = String.Format("{0}'{1}',", sIds, dr["SLTypeId"]);

            }
            if (sIds == string.Empty) sIds = "0,";

            using (DataView dvData = new DataView(m_dtLedgerType.Copy()) { RowFilter = String.Format("SubLedgerTypeId IN ({0})", sIds.TrimEnd(',')) })
            {
                dtSLType = dvData.ToTable();
                if (dtSLType.Rows.Count==0)
                    dtSLType = clsStatic.AddSelectToDataTable(dtSLType, "None");

            }

            dtData.Dispose();
            dtSLType.Dispose();

            if (vGridBRS.FocusedRow.Name == "SLType")
            {
                if (dtSLType==null || dtSLType.Rows.Count == 0) {  return; }
                (vGridBRS.ActiveEditor as LookUpEdit).Properties.DataSource = dtSLType;
                (vGridBRS.ActiveEditor as LookUpEdit).Properties.DisplayMember = "SubLedgerTypeName";
                (vGridBRS.ActiveEditor as LookUpEdit).Properties.ValueMember = "SubLedgerTypeId";
                
            }
            if (vGridBRS.FocusedRow.Name == "SubLedger")
            {
                (vGridBRS.ActiveEditor as LookUpEdit).Properties.DataSource = dtSLName;
                if (dtSLName==null || dtSLName.Rows.Count == 0) {  return; }
                (vGridBRS.ActiveEditor as LookUpEdit).Properties.DisplayMember = "SubLedgerName";
                (vGridBRS.ActiveEditor as LookUpEdit).Properties.ValueMember = "SubLedgerId";
                (vGridBRS.ActiveEditor as LookUpEdit).Properties.ShowFooter = false;
                (vGridBRS.ActiveEditor as LookUpEdit).Properties.ShowHeader= false;
            }

        }
        #endregion

        private void grdViewBRSReg_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            string sText = string.Empty;
            txtNarration.Text = "";

            sText += String.Format("ChequeNo   : {0}{1}", grdViewBRSReg.GetRowCellValue(e.FocusedRowHandle, "ChequeNo"), Environment.NewLine);
            sText += String.Format("ChequeDate : {0}{1}", grdViewBRSReg.GetRowCellValue(e.FocusedRowHandle, "ChequeDate"), Environment.NewLine);
            sText += String.Format("Narration  : {0}{1}", grdViewBRSReg.GetRowCellValue(e.FocusedRowHandle, "Narration"), Environment.NewLine);
            txtNarration.Text = sText;
            dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockRemarks.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
        }

        private void grdViewBRSReg_HiddenEditor(object sender, EventArgs e)
        {
            grdViewBRSReg.UpdateCurrentRow();
            BRSPrevYrBO.EntryId = (int)grdViewBRSReg.GetRowCellValue(grdViewBRSReg.FocusedRowHandle, "EntryId");

            if (grdViewBRSReg.GetRowCellValue(grdViewBRSReg.FocusedRowHandle, "BRSDate") == DBNull.Value)
                BRSPrevYrBO.BRSDate = Convert.ToDateTime(null);
            else
                BRSPrevYrBO.BRSDate = (DateTime)grdViewBRSReg.GetRowCellValue(grdViewBRSReg.FocusedRowHandle, "BRSDate");

            if (EntryBL.Update_BRS_PerviousYr() == true)
                m_dtBRSReg.AcceptChanges();
        }

        private void grdViewBRSReg_DoubleClick(object sender, EventArgs e)
        {
           
            DataTable dt = new DataTable();
            if (grdViewBRSReg.FocusedRowHandle < 0) return;
            m_iEntryId = Convert.ToInt32(grdViewBRSReg.GetFocusedRowCellValue("EntryId"));
            dt = EntryBL.Edit_BRS_Det(m_iEntryId);

            if (dt.Rows.Count > 0)
            {
                vGridBRS.Rows["BookName"].Properties.Value = Convert.ToInt32(clsStatic.IsNullCheck(dt.Rows[0]["BookId"],datatypes.vartypenumeric));

                if (Convert.ToString(dt.Rows[0]["EntryType"]) == "P")
                {
                    vGridBRS.Rows["Type"].Properties.Value = 0;
                }
                else if (Convert.ToString(dt.Rows[0]["EntryType"]) == "R")
                {
                    vGridBRS.Rows["Type"].Properties.Value = 1;
                }
                else
                {
                    vGridBRS.Rows["Type"].Properties.Value = 2;
                }
                vGridBRS.Rows["VoucherDate"].Properties.Value = Convert.ToDateTime(dt.Rows[0]["VoucherDate"]);
                vGridBRS.Rows["VoucherNo"].Properties.Value = Convert.ToString(dt.Rows[0]["VoucherNo"]);
                vGridBRS.Rows["Account"].Properties.Value = Convert.ToInt32(dt.Rows[0]["AccountId"]);
                vGridBRS.Rows["SLType"].Properties.Value = Convert.ToInt32(dt.Rows[0]["SubLedgerTypeId"]);
                vGridBRS.Rows["SubLedger"].Properties.Value = Convert.ToInt32(dt.Rows[0]["SubLedgerId"]);
                vGridBRS.Rows["CostCentre"].Properties.Value = Convert.ToInt32(dt.Rows[0]["CostCentreId"]);
                vGridBRS.Rows["Remarks"].Properties.Value = Convert.ToString(dt.Rows[0]["Narration"]);
                vGridBRS.Rows["Amount"].Properties.Value = Convert.ToDecimal(dt.Rows[0]["Amount"]);
                vGridBRS.Rows["ChequeNo"].Properties.Value = Convert.ToString(dt.Rows[0]["ChequeNo"]);
                vGridBRS.Rows["ChequeDate"].Properties.Value = Convert.ToDateTime(dt.Rows[0]["ChequeDate"]);

                vGridBRS.Rows["BookName"].Properties.ReadOnly = true;
                dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;

                if (Convert.ToBoolean(grdViewBRSReg.GetRowCellValue(grdViewBRSReg.FocusedRowHandle, "BRS")) == true)
                {
                    btnOk.Enabled = false;
                }

                else
                {
                    btnOk.Enabled = true;
                }
                btnBRSExit.Enabled = false;
               
                dwBRS.Show();
                dwBRS.Select();
            }
        }

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           this.Close();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_bDW = true;
            dwBRS.Show();
            dwBRS.Select();
            dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            Fill_VGrid_BRS();
            btnOk.Enabled = true;
            btnBRSExit.Enabled = true;
            m_bDW = false;
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
          
            if (grdViewBRSReg.FocusedRowHandle < 0) return;

            if (MessageBox.Show("Do You Want to Delete Row?", "FA", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                m_iEntryId = Convert.ToInt32(grdViewBRSReg.GetFocusedRowCellValue("EntryId"));
                EntryBL.Delete_BRS(m_iEntryId);
                grdViewBRSReg.DeleteRow(grdViewBRSReg.FocusedRowHandle);
            }
            else
                return;
        }

        private void radDock1_ActiveWindowChanged(object sender, Telerik.WinControls.UI.Docking.DockWindowEventArgs e)
        {
            if (radDock1.ActiveWindow.Name == "dwBRSReg" && m_bDW==false)
            {
                dwBRS.Hide();
                dwBRSReg.Show();
                grdBRSReg.Focus();
                dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                dockRemarks.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
               
            }
            if (radDock1.ActiveWindow.Name == "dwBRS")
            {
                dwBRS.Show();
                dockRemarks.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            }
        }

        private void btnBRSExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(m_iEntryId==0)
            {
                btnBRSExit.Enabled = true;
                this.Close();
            }
            else
            {
                btnBRSExit.Enabled = false;
            }
        }

    }
}