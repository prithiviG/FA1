using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;
using FA.BusinessLayer;
using FA.BusinessObjects;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace FA
{
    public partial class frmCashBank : DevExpress.XtraEditors.XtraForm
    {
        #region Variable

        DataSet m_dsCB;
        string m_sType;
        int mode;
        int m_iPAccId;
        int m_iLevelNo;
        int m_iCompanyId;

        int m_lAccountId;
        DataTable m_dtCashBank;
        DataSet m_dsCheque;
        DataView m_dvDataNew;
        DataTable m_dtData;
        DataTable m_dtGroup;
        DataTable dtData;
        DataSet m_dsChqDet;
        DataTable m_dtChqNo;
        DataTable dtDataChqNo;
        int sumUsedVal;
        int sumCancelVal;
        int iChequeId;
        bool m_bAdd;
        string s_FilePath = "";
        bool bDW = false;
        RepositoryItemButtonEdit btnReport;
        #endregion

        #region Object
        List<ChequeTrans> chequeTransCol;

        #endregion

        #region Constructor

        public frmCashBank()
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

        private void frmCashBank_Load(object sender, EventArgs e)
        {
            SuspendLayout();
            
            defaultLookAndFeel1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Blue";

            DoubleBuffered = true;
            
            mode = 0;
            Fill_Company();
            
            radDockBankCashDet.Visible = false;
            clsStatic.SetMyGraphics();
            CheckPermission();
            ResumeLayout();

            
            grdCashBankView.OptionsBehavior.AllowIncrementalSearch = true;
            grdCashBankView.Appearance.FocusedRow.BackColor = Color.Teal;
            grdCashBankView.Appearance.FocusedRow.ForeColor = Color.White;
            grdCashBankView.Appearance.HideSelectionRow.BackColor = Color.Teal;
            grdCashBankView.Appearance.HideSelectionRow.ForeColor = Color.White;

            grdViewCheque.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewCheque.Appearance.FocusedRow.BackColor = Color.Teal;
            grdViewCheque.Appearance.FocusedRow.ForeColor = Color.White;
            grdViewCheque.Appearance.HideSelectionRow.BackColor = Color.Teal;
            grdViewCheque.Appearance.HideSelectionRow.ForeColor = Color.White;

            grdViewChqNo.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewChqNo.Appearance.FocusedRow.BackColor = Color.Teal;
            grdViewChqNo.Appearance.FocusedRow.ForeColor = Color.White;
            grdViewChqNo.Appearance.HideSelectionRow.BackColor = Color.Teal;
            grdViewChqNo.Appearance.HideSelectionRow.ForeColor = Color.White;

            grdViewChqNoDet.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewChqNoDet.Appearance.FocusedRow.BackColor = Color.Teal;
            grdViewChqNoDet.Appearance.FocusedRow.ForeColor = Color.White;
            grdViewChqNoDet.Appearance.HideSelectionRow.BackColor = Color.Teal;
            grdViewChqNoDet.Appearance.HideSelectionRow.ForeColor = Color.White;
        }

        private void frmCashBank_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                Close();
            }
        }

        private void frmCashBank_FormClosed(object sender, FormClosedEventArgs e)
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
        }

        #endregion

        #region Button Events

        private void barbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_sType == "B")
            {
                if (BsfGlobal.FindPermission("Bank-Add") == false)
                {
                    MessageBox.Show("No Rights to Access this event", "Build SuperFast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                m_bAdd = true;
                m_lAccountId = 0;
                clsStatic.UpdateChildren(dwBankInfo.Controls, false);
                twCashBankList.DockState = Telerik.WinControls.UI.Docking.DockState.Hidden;
                mode = 0;
                barbtnAdd.Enabled = false;
                barbtnOk.Enabled = true;
                //btnCheque.Visible = false;
                dwCheque.Hide();
                Set_VGridBankInfo();
                vGridCashDet.Focus();
               
            }
            else
            {
                if (BsfGlobal.FindPermission("Cash-Add") == false)
                {
                    MessageBox.Show("No Rights to Access this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                clsStatic.UpdateChildren(dwCashInfo.Controls, false);
                twCashBankList.DockState = Telerik.WinControls.UI.Docking.DockState.Hidden;
                mode = 0;
                barbtnAdd.Enabled = false;
                barbtnOk.Enabled = true;
                //btnCheque.Visible = false;
                dwCheque.Hide();
                m_lAccountId = 0;
                m_bAdd = true;
                Set_VGridCashInfo();
                vGridBankDet.Focus();
            }
        }

        private void barbtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dxErrProvider.ClearErrors();
            if (barbtnAdd.Enabled == false)
            {
                barbtnAdd.Enabled = true;
                ClearData();
                grdCashBankView.FocusedRowHandle = -1;
                grdCashBankView.FocusedRowHandle = 0;
                grdCashBankView.Focus();
                if (BsfGlobal.FindPermission("Bank-Edit") == false) { clsStatic.UpdateChildren(twCashBankList.Controls, true); twCashBankList.Enabled = false; }
                if (BsfGlobal.FindPermission("Cash-Edit") == false) { clsStatic.UpdateChildren(twCashBankList.Controls, true); twCashBankList.Enabled = false; }
                twCashBankList.DockState = Telerik.WinControls.UI.Docking.DockState.Docked;
            }
            else
                Close();
        }

        private void barbtnOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dxErrProvider.ClearErrors();
            if (m_sType == "B")
            {
                if (mode == 0)
                {
                    if (CashBankBL.Check_BankName_Exists(m_iCompanyId, vGridBankDet.Rows["BankName"].Properties.Value.ToString()) == true)
                    {
                        DialogResult reply = MessageBox.Show("Bank Name already Exist", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (reply == DialogResult.OK)
                        {
                            vGridBankDet.Rows["BankName"].Properties.Value = string.Empty;
                            return;
                        }
                    }
                }
                if (Convert.ToString(vGridBankDet.Rows["BankName"].Properties.Value.ToString()) == "")
                {
                    DialogResult reply = MessageBox.Show("Enter Bank", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    vGridBankDet.FocusedRow = vGridBankDet.Rows["BankName"];
                    vGridBankDet.Focus();
                    return;
                }
                
                else if (vGridBankDet.Rows["Branch"].Properties.Value.ToString() == "")
                {
                    MessageBox.Show("Enter Branch", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    vGridBankDet.FocusedRow = vGridBankDet.Rows["Branch"];
                    vGridBankDet.Focus();
                    return;
                }
                else if (vGridBankDet.Rows["Validity"].Properties.Value.ToString() == "0")
                {
                    MessageBox.Show("Enter Cheque Validity", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    vGridBankDet.FocusedRow = vGridBankDet.Rows["Validity"];
                    vGridBankDet.Focus();
                    return;
                }
                
                if (Convert.ToBoolean(vGridBankDet.Rows["NewGroup"].Properties.Value) == true)
                {
                    if (vGridBankDet.Rows["GroupName"].Properties.Value == null || vGridBankDet.Rows["GroupName"].Properties.Value.ToString().Trim() == string.Empty)
                    {
                        MessageBox.Show("Enter Group", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        vGridBankDet.FocusedRow = vGridBankDet.Rows["GroupName"];
                        vGridBankDet.Focus();
                        return;
                    }
                }
               
                else if (vGridBankDet.Rows["UnderGroup"].Properties.Value == null || vGridBankDet.Rows["UnderGroup"].Properties.Value.ToString().Trim() == string.Empty)
                {
                    MessageBox.Show("Select Group", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    vGridBankDet.FocusedRow = vGridBankDet.Rows["UnderGroup"];
                    vGridBankDet.Focus();
                    return;
                }
                //else
                //{
                //    if (vGridBankDet.Rows["ReportName"].Properties.Value == null || vGridBankDet.Rows["ReportName"].Properties.Value.ToString().Trim() == string.Empty)
                //    {
                //        MessageBox.Show("Import Report Format", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //        vGridBankDet.FocusedRow = vGridBankDet.Rows["ReportName"];
                //        vGridBankDet.Focus();
                //        return;
                //    }
                //}

            }
            else
            {
                if (vGridCashDet.Rows["CashName"].Properties.Value == null || vGridCashDet.Rows["CashName"].Properties.Value.ToString().Trim() == string.Empty)
                {
                    MessageBox.Show(" Enter Cash", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    vGridCashDet.FocusedRow = vGridCashDet.Rows["CashName"];
                    vGridCashDet.Focus();
                    return;
                }
              
                if (Convert.ToBoolean(vGridCashDet.Rows["NewGroup"].Properties.Value) == true)
                {
                    if (vGridCashDet.Rows["GroupName"].Properties.Value == null || vGridCashDet.Rows["GroupName"].Properties.Value.ToString().Trim() == string.Empty)
                    {
                        MessageBox.Show("Enter Group", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        vGridCashDet.FocusedRow = vGridCashDet.Rows["GroupName"];
                        vGridCashDet.Focus();
                        return;
                    }
                }
                else
                {
                    if (vGridCashDet.Rows["UnderGroup"].Properties.Value == null || vGridCashDet.Rows["UnderGroup"].Properties.Value.ToString().Trim() == string.Empty)
                    {
                        MessageBox.Show("Select Group", "FA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        vGridCashDet.FocusedRow = vGridCashDet.Rows["UnderGroup"];
                        vGridCashDet.Focus();
                        return;
                    }
                }
            }

            if (m_sType == "B")
            {
                AssignData();
                CashBankBL.Update_Bank(m_sType);
                m_bAdd = false;
                ClearData();
            }
            else
            {
                AssignData();
                CashBankBL.Update_Cash(m_sType);
                m_bAdd = false;
                ClearData();
            }
            cboCompany_EditValueChanged(sender, e);
        }

        private void barbtnChqAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (BsfGlobal.FindPermission("Cheque-Add") == false)
            {
                MessageBox.Show("No Rights to Access this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            barbtnChqAdd.Enabled = false;
            barbtnChqEdit.Enabled = false;
            barbtnChqDelete.Enabled = false;
            dwChqList.Hide();
            dwDetails.Show();
            mode = 0;
            ChequeMaster.CheckUsedcheque = new ArrayList();
            ChequeMaster.CheckCancelledcheque = new ArrayList();
            Fill_Data_ChqDet();
            vGridChqInfo.Focus();
        }

        private void barbtnChqEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (grdViewCheque.FocusedRowHandle < 0) return;
            if (BsfGlobal.FindPermission("Cheque-Edit") == false)
            {
                MessageBox.Show("No Rights to Access this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            barbtnChqAdd.Enabled = false;
            barbtnChqEdit.Enabled = false;
            barbtnChqDelete.Enabled = false;
            dwChqList.Hide();
            dwDetails.Show();
            iChequeId = Convert.ToInt32(grdViewCheque.GetRowCellValue(grdViewCheque.FocusedRowHandle, "ChequeId").ToString());
            mode = 1;
            Fill_Data_ChqDet();
            vGridChqInfo.Focus();
        }

        private void barbtnChqDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (grdViewCheque.FocusedRowHandle < 0) return;
            if (BsfGlobal.FindPermission("Cheque-Delete") == false)
            {
                MessageBox.Show("No Rights to Access this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (MessageBox.Show("Do you want to delete?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sumUsedVal = 0;
                    sumCancelVal = 0;
                    iChequeId = Convert.ToInt32(grdViewCheque.GetRowCellValue(grdViewCheque.FocusedRowHandle, "ChequeId").ToString());
                    m_dsCheque = new DataSet();
                    m_dsCheque = ChequeBL.GetGetChequeDetails();
                    if (m_dsCheque.Tables[1].Rows.Count > 0)
                    {
                        m_dvDataNew = new DataView(m_dsCheque.Tables[1]) { RowFilter = String.Format("ChequeId='{0}'", iChequeId) };
                        m_dtData = m_dvDataNew.ToTable();
                    }
                    for (int m = 0; m < m_dtData.Rows.Count; m++)
                    {
                        if (m_dtData.Rows[m]["Used"].ToString() == "True")
                        {
                            sumUsedVal = sumUsedVal + 1;
                        }
                        if (m_dtData.Rows[m]["Cancel"].ToString() == "True")
                        {
                            sumCancelVal = sumCancelVal + 1;
                        }
                    }
                    if (sumCancelVal == 0 && sumUsedVal == 0)
                    {
                        if (ChequeBL.Delete_ChequeNo(iChequeId) == true)
                        {
                            grdViewCheque.DeleteRow(grdViewCheque.FocusedRowHandle);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The specified cheques used / cancelled", "FA", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barbtnChqOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            if (mode == 0)
            {

                ChequeMaster.StartNo = Convert.ToInt32( vGridChqInfo.Rows["StartNo"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal( vGridChqInfo.Rows["StartNo"].Properties.Value));
                ChequeMaster.NoofLeaves = Convert.ToInt32(vGridChqInfo.Rows["NoofLeaves"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal(vGridChqInfo.Rows["NoofLeaves"].Properties.Value));
                ChequeMaster.EndNo = Convert.ToInt32(vGridChqInfo.Rows["EndNo"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal(vGridChqInfo.Rows["EndNo"].Properties.Value));
                ChequeMaster.ChequeRDate = Convert.ToDateTime(vGridChqInfo.Rows["RecdDate"].Properties.Value); 
                ChequeMaster.Remarks = vGridChqInfo.Rows["Remarks"].Properties.Value.ToString();
                ChequeMaster.AccountID = m_lAccountId;
                ChequeMaster.CompanyId = BsfGlobal.g_lCompanyId;
                ChequeMaster.ChequeNoWidth = Convert.ToInt32(vGridChqInfo.Rows["Width"].Properties.Value);
                chequeTransCol = new List<ChequeTrans>();

                if ((ChequeMaster.NoofLeaves != 0) && (ChequeMaster.EndNo != 0) && (ChequeMaster.StartNo != 0))
                {
                    chequeTransCol.Clear();
                    for (int i = 0; i < m_dtChqNo.Rows.Count; i++)
                    {
                        chequeTransCol.Add(new ChequeTrans()
                        {
                            ChequeID = ChequeMaster.ChequeID,
                            AccountId = m_lAccountId,
                            ChequeNo = Convert.ToString(m_dtChqNo.Rows[i]["ChequeNo"].ToString()),
                            CancelDate = Convert.ToDateTime(null),
                            CancelRemarks = ""

                        });
                    }
                }
                ChequeBL.InsertChequeMaster(chequeTransCol);
                vGridChqInfo.Rows["StartNo"].Properties.Value = string.Empty; ;
                vGridChqInfo.Rows["EndNo"].Properties.Value = string.Empty; ;
                vGridChqInfo.Rows["NoofLeaves"].Properties.Value = string.Empty; ;
                vGridChqInfo.Rows["Width"].Properties.Value = string.Empty; ;
                vGridChqInfo.Rows["RecdDate"].Properties.Value = string.Empty; ;
                vGridChqInfo.Rows["Remarks"].Properties.Value = string.Empty; ;
            }
            if (mode == 1)
            {
                if (ChequeBL.Cheque_Used(iChequeId) == true)
                {
                    MessageBox.Show("Cheque No's used for transactions, Can't generate again");
                    return;
                }
                else if (ChequeBL.Cheque_Cancelled(iChequeId) == true)
                {
                    MessageBox.Show("Cheque No's (Cancel) used for transactions, Can't generate again");
                    return;
                }

                ChequeMaster.StartNo = Convert.ToInt32(vGridChqInfo.Rows["StartNo"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal(vGridChqInfo.Rows["StartNo"].Properties.Value));
                ChequeMaster.NoofLeaves = Convert.ToInt32(vGridChqInfo.Rows["NoofLeaves"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal(vGridChqInfo.Rows["NoofLeaves"].Properties.Value));
                ChequeMaster.EndNo = Convert.ToInt32(vGridChqInfo.Rows["EndNo"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal(vGridChqInfo.Rows["EndNo"].Properties.Value));
                ChequeMaster.ChequeRDate = Convert.ToDateTime(vGridChqInfo.Rows["RecdDate"].Properties.Value);
                ChequeMaster.Remarks = vGridChqInfo.Rows["Remarks"].Properties.Value.ToString();
                ChequeMaster.AccountID = m_lAccountId;
                ChequeMaster.CompanyId = BsfGlobal.g_lCompanyId;
                ChequeMaster.ChequeNoWidth = Convert.ToInt32(vGridChqInfo.Rows["Width"].Properties.Value);



                if ((ChequeMaster.NoofLeaves != 0) && (ChequeMaster.EndNo != 0) && (ChequeMaster.StartNo != 0))
                {
                    chequeTransCol.Clear();
                    for (int i = 0; i < m_dtChqNo.Rows.Count; i++)
                    {
                        chequeTransCol.Add(new ChequeTrans()
                        {
                            ChequeID = iChequeId,
                            AccountId = m_lAccountId,
                            ChequeNo = Convert.ToString(m_dtChqNo.Rows[i]["ChequeNo"].ToString()),
                            CancelDate = Convert.ToDateTime(null),
                            CancelRemarks = ""
                        });
                    }
                }
                ChequeBL.UpdateChequeMaster(chequeTransCol);
                vGridChqInfo.Rows["StartNo"].Properties.Value = string.Empty;
                vGridChqInfo.Rows["EndNo"].Properties.Value = string.Empty; 
                vGridChqInfo.Rows["NoofLeaves"].Properties.Value = string.Empty; 
                vGridChqInfo.Rows["Width"].Properties.Value = string.Empty; 
                vGridChqInfo.Rows["RecdDate"].Properties.Value = DateTime.Now;
                vGridChqInfo.Rows["Remarks"].Properties.Value = string.Empty; 
            }
            m_dsChqDet = new DataSet();
            m_dsChqDet = ChequeBL.GetGetChequeDetails();
            barbtnChqAdd.Enabled = true;
            barbtnChqEdit.Enabled = true;
            barbtnChqDelete.Enabled = true;
            Fill_ChqDet_Grid();
            dwChqList.Show();
            dwDetails.Hide();
        }

        private void barbtnChqCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barbtnChqAdd.Enabled = true;
            barbtnChqEdit.Enabled = true;
            barbtnChqDelete.Enabled = true;
            dwChqList.Show();
            dwDetails.Hide();
        }

        private void barbtnChqDet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (grdViewCheque.FocusedRowHandle < 0) return;
            iChequeId = Convert.ToInt32(grdViewCheque.GetRowCellValue(grdViewCheque.FocusedRowHandle, "ChequeId").ToString());
            //dwCheque.Text = txtBank.Text + " - Cheque No Details";
            Fill_ChqNoDet_Grid();
            dwChqNoDet.Show();
            dwCheque.Hide();
            grdViewChqNoDet.FocusedRowHandle = 0;
            grdViewChqNoDet.FocusedColumn = grdViewChqNoDet.VisibleColumns[0];
            grdViewChqNoDet.Focus();
        }

        private void barbtnChqNoCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dwChqNoDet.Hide();
            dwCheque.Show();
            grdViewCheque.Focus();
        }

        private void barbtnChqNoOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool bOk = ChequeBL.Update_ChequeNo();
            ChequeBL.ChequeNo = new DataTable();
            grdViewChqNoDet.CloseEditor();
            ChequeBL.ChequeNo = dtDataChqNo.GetChanges(DataRowState.Modified);
            if (bOk == true)
            {
                dwChqNoDet.Hide();
                dwCheque.Show();
                grdViewCheque.Focus();
            }
        }

        #endregion

        #region Grid Events

        private void grdBankView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (grdCashBankView.FocusedRowHandle >= 0 && grdCashBankView.RowCount>0)
            {
                if (m_sType == "B")
                {
                    if (grdCashBankView.GetFocusedRowCellValue("BankName").ToString() != null)
                    {
                        mode = 1;
                        dwBank.Show();
                        dwBankInfo.Show();
                        dwCashInfo.Hide();
                        //btnCheque.Visible = true;
                        CashBankBL.AccId = Convert.ToInt32(grdCashBankView.GetFocusedRowCellValue("AccountId"));
                        m_lAccountId = Convert.ToInt32(grdCashBankView.GetFocusedRowCellValue("AccountId"));
                        Set_VGridBankInfo();
                        dwCheque.Hide();
                        dwChqNoDet.Hide();
                    }
                }
                else
                {
                    if (grdCashBankView.GetFocusedRowCellValue("BankName").ToString() != null)
                    {
                        mode = 1;
                        dwBank.Show();
                        dwBankInfo.Hide();
                        dwCashInfo.Show();
                        CashBankBL.AccId = Convert.ToInt32(grdCashBankView.GetFocusedRowCellValue("AccountId"));
                        m_lAccountId = Convert.ToInt32(grdCashBankView.GetFocusedRowCellValue("AccountId"));
                        Set_VGridCashInfo();
                       
                    }
                }
            }
        }

        private void grdChqNoDet_ProcessGridKey(object sender, KeyEventArgs e)
        {
            DevExpress.XtraGrid.GridControl grid = sender as DevExpress.XtraGrid.GridControl;
            GridView gridView = grid.FocusedView as GridView;
            if (!gridView.IsEditing && e.KeyCode == Keys.Enter)
            {
                SendKeys.SendWait("{TAB}");
                e.Handled = true;
            }
        }

        private void grdViewChqNoDet_ShownEditor(object sender, EventArgs e)
        {
            bool bUsed = false;
            bool bCancel = false;
            GridView view = sender as GridView;

            bUsed = (bool)view.GetRowCellValue(view.FocusedRowHandle, "Used");
            bCancel = (bool)view.GetRowCellValue(view.FocusedRowHandle, "Cancel");
            if (bUsed == true) grdViewChqNoDet.HideEditor();
            if (bCancel == false && (view.FocusedColumn.FieldName == "CancelDate" || view.FocusedColumn.FieldName == "Remarks"))
            {
                grdViewChqNoDet.HideEditor();
            }
        }

        private void grdViewChqNoDet_HiddenEditor(object sender, EventArgs e)
        {
            grdViewChqNoDet.UpdateCurrentRow();
        }
        #endregion

        #region TextBox Events

        #endregion

        #region Combo Box Events

        private void cboCompany_EditValueChanged(object sender, EventArgs e)
        {
            Fill_CashOrBank();
            //btnCheque.Visible = true;
            CheckPermission();
            grdCashBank.Focus();
        }

        #endregion

        #region Radio Events

        private void radType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fill_CashOrBank();
        }

        #endregion

        #region Function

        private void CheckPermission()
        {
            if (BsfGlobal.g_sUnPermissionMode == "H")
            {
                if (BsfGlobal.FindPermission("Bank-Add") == false) { barbtnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never; }
                if (BsfGlobal.FindPermission("Bank-Edit") == false) { barbtnOk.Visibility = DevExpress.XtraBars.BarItemVisibility.Never; }

                if (BsfGlobal.FindPermission("Cash-Add") == false) { barbtnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never; }
                if (BsfGlobal.FindPermission("Cash-Edit") == false) { barbtnOk.Visibility = DevExpress.XtraBars.BarItemVisibility.Never; }

            }
            else if (BsfGlobal.g_sUnPermissionMode == "D")
            {
                if (BsfGlobal.FindPermission("Bank-Add") == false) { barbtnAdd.Enabled = false; }
                if (BsfGlobal.FindPermission("Bank-Edit") == false) { barbtnOk.Enabled = false; }

                if (BsfGlobal.FindPermission("Cash-Add") == false) { barbtnAdd.Enabled = false; }
                if (BsfGlobal.FindPermission("Cash-Edit") == false) { barbtnOk.Enabled = false; }
            }

            if (BsfGlobal.FindPermission("Bank-Edit") == false)
            {
                clsStatic.UpdateChildren(dwBankInfo.Controls, true);
                barbtnOk.Enabled = false;
            }

            if (BsfGlobal.FindPermission("Cash-Edit") == false)
            {
                clsStatic.UpdateChildren(dwCashInfo.Controls, true);
                barbtnOk.Enabled = false;
            }
        }

        public void AssignData()
        {
            if (m_sType == "B")
            {
                if (mode == 0) CashBankBL.AccId = 0;
                CashBankBL.CompanyId = m_iCompanyId;
                CashBankBL.Mode = mode;
                CashBankBL.LevelNo = m_iLevelNo;
                CashBankBL.AccName = vGridBankDet.Rows["BankName"].Properties.Value.ToString();
                CashBankBL.AccountNo = vGridBankDet.Rows["AccountNo"].Properties.Value.ToString();
                CashBankBL.Branch = vGridBankDet.Rows["Branch"].Properties.Value.ToString();
                CashBankBL.Address1 = vGridBankDet.Rows["Address"].Properties.Value.ToString();
                CashBankBL.Address2 = "";
                CashBankBL.ContactPerson = vGridBankDet.Rows["Contact"].Properties.Value.ToString();
                CashBankBL.Mobile = vGridBankDet.Rows["Mobile"].Properties.Value.ToString();
                CashBankBL.Phone = vGridBankDet.Rows["Phone"].Properties.Value.ToString();
                CashBankBL.Fax = vGridBankDet.Rows["Fax"].Properties.Value.ToString();
                CashBankBL.Country= vGridBankDet.Rows["Country"].Properties.Value.ToString();
                CashBankBL.State = vGridBankDet.Rows["State"].Properties.Value.ToString();
                CashBankBL.City = vGridBankDet.Rows["City"].Properties.Value.ToString();
                CashBankBL.Pincode = "";
                CashBankBL.IFSCCode = vGridBankDet.Rows["IFSCCode"].Properties.Value.ToString();
                CashBankBL.CALimit = Convert.ToDecimal(clsStatic.IsNullCheck(vGridBankDet.Rows["CALimit"].Properties.Value.ToString(),datatypes.vartypenumeric));
                CashBankBL.BGLimit = Convert.ToDecimal(clsStatic.IsNullCheck(vGridBankDet.Rows["BGLimit"].Properties.Value,datatypes.vartypenumeric));
                CashBankBL.LCLimit = Convert.ToDecimal(clsStatic.IsNullCheck(vGridBankDet.Rows["LCLimit"].Properties.Value,datatypes.vartypenumeric));
                CashBankBL.LCDuration = Convert.ToInt32(clsStatic.IsNullCheck(vGridBankDet.Rows["LCDuration"].Properties.Value, datatypes.vartypenumeric));
                CashBankBL.Validity= Convert.ToInt32(clsStatic.IsNullCheck(vGridBankDet.Rows["Validity"].Properties.Value, datatypes.vartypenumeric));

                if (Convert.ToBoolean(vGridBankDet.Rows["NewGroup"].Properties.Value) == true)
                {
                    CashBankBL.IsCashBankGroup = true;
                    CashBankBL.GroupName = vGridBankDet.Rows["GroupName"].Properties.Value.ToString();
                    CashBankBL.CashBankGroupId = 0;
                    CashBankBL.PAccId = 0;
                }
                else
                {
                    CashBankBL.IsCashBankGroup = false;
                    CashBankBL.GroupName = "";
                    CashBankBL.CashBankGroupId = Convert.ToInt32(vGridBankDet.Rows["UnderGroup"].Properties.Value);
                    CashBankBL.PAccId = Convert.ToInt32(vGridBankDet.Rows["UnderGroup"].Properties.Value);
                }
                CashBankBL.ReportName = Convert.ToString(clsStatic.IsNullCheck(vGridBankDet.Rows["ReportName"].Properties.Value, datatypes.vartypestring));
            }
            else if (m_sType == "C")
            {
                if (mode == 0) CashBankBL.AccId = 0;
                CashBankBL.CompanyId = m_iCompanyId;
                CashBankBL.Mode = mode;
                CashBankBL.AccName = vGridCashDet.Rows["CashName"].Properties.Value.ToString();
                if (Convert.ToBoolean(vGridCashDet.Rows["NewGroup"].Properties.Value) == true)
                {
                    CashBankBL.IsCashBankGroup = true;
                    CashBankBL.GroupName = vGridCashDet.Rows["GroupName"].Properties.Value.ToString();
                    CashBankBL.CashBankGroupId = 0;
                    CashBankBL.PAccId =0;
                }
                else
                {
                    CashBankBL.IsCashBankGroup = false;
                    CashBankBL.GroupName = "";
                    CashBankBL.CashBankGroupId = Convert.ToInt32(vGridCashDet.Rows["UnderGroup"].Properties.Value);
                    CashBankBL.PAccId = Convert.ToInt32(vGridCashDet.Rows["UnderGroup"].Properties.Value); 
                }
            }
        }

        private void ClearData()
        {
            if (m_sType == "B")
            {
                vGridBankDet.Rows["BankName"].Properties.Value = string.Empty;
                vGridBankDet.Rows["Branch"].Properties.Value = string.Empty;
                vGridBankDet.Rows["Address"].Properties.Value = string.Empty;
                vGridBankDet.Rows["Contact"].Properties.Value = string.Empty;
                vGridBankDet.Rows["Mobile"].Properties.Value = string.Empty;
                vGridBankDet.Rows["Phone"].Properties.Value = string.Empty;
                vGridBankDet.Rows["Fax"].Properties.Value = string.Empty;
                vGridBankDet.Rows["Country"].Properties.Value = string.Empty;
                vGridBankDet.Rows["State"].Properties.Value = string.Empty;
                vGridBankDet.Rows["City"].Properties.Value = string.Empty;
                vGridBankDet.Rows["IFSCCode"].Properties.Value = string.Empty;
                vGridBankDet.Rows["CALimit"].Properties.Value = string.Empty;
                vGridBankDet.Rows["BGLimit"].Properties.Value = string.Empty;
                vGridBankDet.Rows["ODLimit"].Properties.Value = string.Empty;
                vGridBankDet.Rows["LCLimit"].Properties.Value = string.Empty;
                vGridBankDet.Rows["LCDuration"].Properties.Value = string.Empty;
                vGridBankDet.Rows["ODLimit"].Properties.Value = string.Empty;
                vGridBankDet.Rows["LCLimit"].Properties.Value = string.Empty;
                vGridBankDet.Rows["LCDuration"].Properties.Value = string.Empty;

            }
            else
            {
                vGridCashDet.Rows["CashName"].Properties.Value = string.Empty;
                //vGridDet.Rows["ParentAccountId"].Properties.Value = null;
            }
            m_bAdd = false;
        }
    
    
        private void Fill_Company()
        {
            m_dsCB = new DataSet();
            m_dsCB = CompanysBL.Get_Companys();
            cboCompany.Properties.NullText = "--Select Company--";
            cboCompany.Properties.DataSource = m_dsCB.Tables[2];
            cboCompany.Properties.ForceInitialize();
            cboCompany.Properties.PopulateColumns();
            cboCompany.Properties.DisplayMember = "ShortName";
            cboCompany.Properties.ValueMember = "CompanyId";
            cboCompany.Properties.Columns["CompanyId"].Visible = false;
            cboCompany.Properties.Columns["ShortName"].Visible = true;
            cboCompany.Properties.Columns["CompanyName"].Visible = false;
        }

        private void Fill_CashOrBank()
        {
            m_sType = string.Empty;

            if (cboCompany.EditValue == null) return;
            if (Convert.ToInt32(cboCompany.EditValue.ToString()) == 0) return;
            m_iCompanyId = Convert.ToInt32(cboCompany.EditValue.ToString());

            if (radType.SelectedIndex == 0)
            {
                if (BsfGlobal.FindPermission("Bank-View") == false)
                {
                    MessageBox.Show("No Rights to proceed this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                radDockBankCashDet.Visible = true;
                twCashBankList.Show();
                twCashBankList.DockState = Telerik.WinControls.UI.Docking.DockState.Docked;
                dwCheque.Hide();
                dwBank.Show();
                dwBankInfo.Show();
                dwCashInfo.Hide();
                dwChqNoDet.Hide();
                barbtnAdd.Enabled = true;
                m_sType = "B";
                m_dtGroup = new DataTable();
                m_dtGroup = CashBankBL.Get_CashBankParent(ref m_iPAccId, ref m_iLevelNo);
                Fill_CashBankList();
                Set_VGridBankInfo();

            }

            if (radType.SelectedIndex == 1)
            {
                if (BsfGlobal.FindPermission("Cash-View") == false)
                {
                    MessageBox.Show("No Rights to proceed this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                radDockBankCashDet.Visible = true;
                twCashBankList.Show();
                twCashBankList.DockState = Telerik.WinControls.UI.Docking.DockState.Docked;
                dwCheque.Hide();
                dwBank.Show();
                dwBankInfo.Hide();
                dwCashInfo.Show();
                dwChqNoDet.Hide();
                barbtnAdd.Enabled = true;
                m_sType = "C";
                m_dtGroup = new DataTable();
                m_dtGroup = CashBankBL.Get_CashBankParent(ref m_iPAccId, ref m_iLevelNo);
                Fill_CashBankList();
                Set_VGridCashInfo();
            }
        }

        private void Fill_CashBankList()
        {
            DataView dvDataNew = null;
            DataTable dtData = null;
            m_lAccountId = 0;
            m_dtCashBank = new DataTable();
            m_dtCashBank = CashBankBL.Get_CashBank(m_iCompanyId);
            grdCashBankView.Columns.Clear();
            grdCashBank.DataSource = null;
            grdCashBank.ForceInitialize();
            
            if (m_sType == "B")
            {
                dvDataNew = new DataView(m_dtCashBank) { RowFilter = "CashOrBank='B'" };
                dtData = dvDataNew.ToTable();
            }
            else
            {
                dvDataNew = new DataView(m_dtCashBank) { RowFilter = "CashOrBank='C'" };
                dtData = dvDataNew.ToTable();
            }
            grdCashBankView.FocusedRowHandle = -1;
            grdCashBank.DataSource = dtData;
            grdCashBankView.Columns["CashBankId"].Visible = false;
            grdCashBankView.Columns["AccountNo"].Visible = false;
            grdCashBankView.Columns["AccountId"].Visible = false;
            grdCashBankView.Columns["CashOrBank"].Visible = false;
            grdCashBankView.Columns["Address1"].Visible = false;
            grdCashBankView.Columns["Address2"].Visible = false;
            grdCashBankView.Columns["City"].Visible = false;
            grdCashBankView.Columns["State"].Visible = false;
            grdCashBankView.Columns["Pincode"].Visible = false;
            grdCashBankView.Columns["Country"].Visible = false;
            grdCashBankView.Columns["Fax"].Visible = false;
            grdCashBankView.Columns["CALimit"].Visible = false;
            grdCashBankView.Columns["ODLimit"].Visible = false;
            grdCashBankView.Columns["BGLimit"].Visible = false;
            grdCashBankView.Columns["LCLimit"].Visible = false;
            grdCashBankView.Columns["OpeningBalance"].Visible = false;
            grdCashBankView.Columns["LCDuration"].Visible = false;
            grdCashBankView.Columns["ParentAccountId"].Visible = false;
            grdCashBankView.Columns["ReportName"].Visible = false;
            grdCashBankView.Columns["Validity"].Visible = false;

            grdCashBankView.Columns["BankName"].OptionsColumn.AllowEdit = false;
            grdCashBankView.Columns["BankName"].Caption = "Bank/Cash Name";
            grdCashBankView.Columns["BankName"].Visible = true;
            grdCashBankView.Columns["Branch"].OptionsColumn.AllowEdit = false;
            grdCashBankView.Columns["Branch"].Visible = false;
            grdCashBankView.Columns["IFSCCode"].OptionsColumn.AllowEdit = false;
            grdCashBankView.Columns["IFSCCode"].Visible = false;
            grdCashBankView.Columns["ContactPerson"].OptionsColumn.AllowEdit = false;
            grdCashBankView.Columns["ContactPerson"].Visible = false;
            grdCashBankView.Columns["Mobile"].OptionsColumn.AllowEdit = false;
            grdCashBankView.Columns["Mobile"].Visible = false;
            grdCashBankView.Columns["Phone"].OptionsColumn.AllowEdit = false;
            grdCashBankView.Columns["Phone"].Visible = false;
            grdCashBankView.Appearance.HeaderPanel.Font = new Font(grdCashBankView.Appearance.HeaderPanel.Font, FontStyle.Bold);
            grdCashBankView.Appearance.FooterPanel.Font = new Font(grdCashBankView.Appearance.HeaderPanel.Font, FontStyle.Bold);
            
            grdCashBankView.FocusedRowHandle = 0;
            grdCashBankView.FocusedColumn = grdCashBankView.VisibleColumns[0];
            grdCashBankView.Focus();

            if (dtData.Rows.Count == 0)
            {
                if (m_sType == "C")
                    clsStatic.UpdateChildren(dwCashInfo.Controls, true);
                else
                    clsStatic.UpdateChildren(dwBankInfo.Controls, true);
            }
            else
            {
                if (m_sType == "C")
                    clsStatic.UpdateChildren(dwCashInfo.Controls, false);
                else
                    clsStatic.UpdateChildren(dwBankInfo.Controls, false);
            }
        }


        private void Fill_ChqDet_Grid()
        {
            m_dsCheque = new DataSet();
            m_dsCheque = ChequeBL.GetGetChequeDetails();
            if (m_dsCheque.Tables.Count > 0)
            {
                if (m_dsCheque.Tables["ChqDetails"].Rows.Count > 0)
                {
                    
                    m_dvDataNew = new DataView(m_dsCheque.Tables["ChqDetails"]) { RowFilter = String.Format("AccountId='{0}'", m_lAccountId) };
                    m_dtData = m_dvDataNew.ToTable();
                    grdCheque.DataSource = null;
                    grdViewCheque.Columns.Clear();
                    grdCheque.DataSource = m_dtData;
                    grdCheque.ForceInitialize();
                    grdViewCheque.PopulateColumns();

                    grdViewCheque.Columns["ChequeId"].Visible = false;
                    grdViewCheque.Columns["AccountId"].Visible = false;
                    grdViewCheque.Columns["ChequeRDate"].Visible = false;

                    grdViewCheque.Columns["StartNo"].Visible = true;
                    grdViewCheque.Columns["StartNo"].OptionsColumn.AllowEdit = false;
                    grdViewCheque.Columns["StartNo"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdViewCheque.Columns["StartNo"].Width = 75;

                    grdViewCheque.Columns["ChequeNoWidth"].Visible = true;
                    grdViewCheque.Columns["ChequeNoWidth"].Caption = "Width";
                    grdViewCheque.Columns["ChequeNoWidth"].OptionsColumn.AllowEdit = false;
                    grdViewCheque.Columns["ChequeNoWidth"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

                    grdViewCheque.Columns["NoofLeaves"].Visible = true;
                    grdViewCheque.Columns["NoofLeaves"].OptionsColumn.AllowEdit = false;
                    grdViewCheque.Columns["NoofLeaves"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdViewCheque.Columns["NoofLeaves"].Width = 85;

                    grdViewCheque.Columns["EndNo"].Visible = true;
                    grdViewCheque.Columns["EndNo"].OptionsColumn.AllowEdit = false;
                    grdViewCheque.Columns["EndNo"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    grdViewCheque.Columns["EndNo"].Width = 75;

                    grdViewCheque.Columns["Used"].Visible = true;
                    grdViewCheque.Columns["Used"].OptionsColumn.AllowEdit = false;
                    grdViewCheque.Columns["Used"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

                    grdViewCheque.Columns["Cancel"].Visible = true;
                    grdViewCheque.Columns["Cancel"].OptionsColumn.AllowEdit = false;
                    grdViewCheque.Columns["Cancel"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

                    grdViewCheque.Columns["Balance"].Visible = true;
                    grdViewCheque.Columns["Balance"].OptionsColumn.AllowEdit = false;
                    grdViewCheque.Columns["Balance"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

                    grdViewCheque.Columns["CompanyId"].Visible = false;

                    grdViewCheque.OptionsCustomization.AllowFilter = false;
                    grdViewCheque.OptionsBehavior.AllowIncrementalSearch = true;
                    grdViewCheque.OptionsView.ShowAutoFilterRow = false;
                    grdViewCheque.OptionsView.ShowViewCaption = false;
                    grdViewCheque.OptionsView.ShowFooter = false;
                    grdViewCheque.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
                    grdViewCheque.OptionsSelection.InvertSelection = true;
                    grdViewCheque.OptionsView.ColumnAutoWidth = true;
                    grdViewCheque.Appearance.HeaderPanel.Font = new Font(grdViewCheque.Appearance.HeaderPanel.Font, FontStyle.Bold);
                    grdViewCheque.FocusedRowHandle = 0;
                    grdViewCheque.FocusedColumn = grdViewCheque.VisibleColumns[0];
                }
            }
        }

        private void Fill_Data_ChqDet()
        {
            if (mode == 1)
            {
                ChequeMaster.CheckUsedcheque = new ArrayList();
                ChequeMaster.CheckCancelledcheque = new ArrayList();
                m_dsChqDet = new DataSet();
                m_dsChqDet = ChequeBL.GetGetChequeDetails();
                if (m_dsChqDet.Tables[0].Rows.Count > 0)
                {
                    
                    m_dvDataNew = new DataView(m_dsChqDet.Tables[0]) { RowFilter = String.Format("ChequeId='{0}'", iChequeId) };
                    dtData = m_dvDataNew.ToTable();
                    for (int i = 0; i < dtData.Rows.Count; i++)
                    {
                      
                        vGridChqInfo.Rows["StartNo"].Properties.Value = dtData.Rows[i]["StartNo"].ToString();
                        vGridChqInfo.Rows["NoofLeaves"].Properties.Value = dtData.Rows[i]["NoofLeaves"].ToString();
                        vGridChqInfo.Rows["EndNo"].Properties.Value = dtData.Rows[i]["EndNo"].ToString();
                        vGridChqInfo.Rows["Width"].Properties.Value = dtData.Rows[i]["ChequeNoWidth"].ToString();
                        vGridChqInfo.Rows["RecdDate"].Properties.Value = Convert.ToDateTime(dtData.Rows[i]["ChequeRDate"].ToString());
                        vGridChqInfo.Rows["Remarks"].Properties.Value = dtData.Rows[i]["Remarks"].ToString();
                    }

                    m_dvDataNew = new DataView(m_dsChqDet.Tables[1]) { RowFilter = String.Format("ChequeId='{0}'", iChequeId) };
                    dtData = m_dvDataNew.ToTable();
                    for (int i = 0; i < dtData.Rows.Count; i++)
                    {
                        if (dtData.Rows[i]["Used"].ToString() == "True")
                        {
                            ChequeMaster.CheckUsedcheque.Add(dtData.Rows[i]["Used"].ToString());
                        }
                        else if (dtData.Rows[i]["Cancel"].ToString() == "True")
                        {
                            ChequeMaster.CheckCancelledcheque.Add(dtData.Rows[i]["Cancel"].ToString());
                        }
                    }

                    if (ChequeMaster.CheckUsedcheque.Count > 0 || ChequeMaster.CheckCancelledcheque.Count > 0)
                    {
                        vGridChqInfo.Rows["StartNo"].Properties.ReadOnly = true;
                        vGridChqInfo.Rows["EndNo"].Properties.ReadOnly = true;
                        vGridChqInfo.Rows["NoofLeaves"].Properties.ReadOnly = true;
                        vGridChqInfo.Rows["Width"].Properties.ReadOnly = true;
                        vGridChqInfo.Rows["RecdDate"].Properties.ReadOnly = true;
                        vGridChqInfo.Rows["Remarks"].Properties.ReadOnly = true;
            
                    }
                    else
                    {
                        vGridChqInfo.Rows["StartNo"].Properties.ReadOnly = false;
                        vGridChqInfo.Rows["EndNo"].Properties.ReadOnly = true;
                        vGridChqInfo.Rows["NoofLeaves"].Properties.ReadOnly = false;
                        vGridChqInfo.Rows["Width"].Properties.ReadOnly = false;
                        vGridChqInfo.Rows["RecdDate"].Properties.ReadOnly = false;
                        vGridChqInfo.Rows["Remarks"].Properties.ReadOnly = false;
            
                    }
                    Generate_ChqNo();
                }
            }
            else
            {
                grdChqNo.DataSource = null;
                vGridChqInfo.Rows["StartNo"].Properties.Value = string.Empty;
                vGridChqInfo.Rows["EndNo"].Properties.Value = string.Empty;
                vGridChqInfo.Rows["NoofLeaves"].Properties.Value = string.Empty;
                vGridChqInfo.Rows["Width"].Properties.Value = string.Empty;
                vGridChqInfo.Rows["RecdDate"].Properties.Value =DateTime.Now;
                vGridChqInfo.Rows["Remarks"].Properties.Value = string.Empty;
                        
                vGridChqInfo.Rows["StartNo"].Properties.ReadOnly = false;
                vGridChqInfo.Rows["EndNo"].Properties.ReadOnly = true;
                vGridChqInfo.Rows["NoofLeaves"].Properties.ReadOnly = false;
                vGridChqInfo.Rows["Width"].Properties.ReadOnly = false;
                vGridChqInfo.Rows["RecdDate"].Properties.ReadOnly = false;
                vGridChqInfo.Rows["Remarks"].Properties.ReadOnly = false;
            }

        }

        private void Generate_ChqNo()
        {
            int iWidth = Convert.ToInt32( vGridChqInfo.Rows["Width"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal( vGridChqInfo.Rows["Width"].Properties.Value));
            int iStart = Convert.ToInt32(vGridChqInfo.Rows["StartNo"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal(vGridChqInfo.Rows["StartNo"].Properties.Value));
            int iTotal = Convert.ToInt32(vGridChqInfo.Rows["NoofLeaves"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToDecimal(vGridChqInfo.Rows["NoofLeaves"].Properties.Value));
            vGridChqInfo.Rows["EndNo"].Properties.Value = Convert.ToString(iStart + iTotal - 1);
            int iEnd = Convert.ToInt32(vGridChqInfo.Rows["EndNo"].Properties.Value.ToString() == string.Empty ? 0 : Convert.ToInt32(vGridChqInfo.Rows["EndNo"].Properties.Value));
            m_dtChqNo = new DataTable();


            if (iWidth != 0)
            {
                ChequeMaster.DtChequeNo = null;
                ChequeMaster.DtChequeNo = new DataTable();
                ChequeMaster.DtChequeNo.Columns.Add("ChequeNo");
                ChequeMaster.DtChequeNo.Columns.Add("AccountId");
                ChequeMaster.DtChequeNo.Columns.Add("CompanyId");

                DataRow newrow;
                string s = "";
                int tcnt = 0;
                string sPre = "";
                tcnt = iWidth - iStart.ToString().Length;
                for (int k = 1; k <= iWidth; k++)
                {
                    sPre = sPre + "0";
                }

                for (int i = iStart; i <= iEnd; i++)
                {
                    newrow = ChequeMaster.DtChequeNo.NewRow();

                    if (iWidth > 0 && tcnt != 0)
                    {
                        for (int k = 1; k <= tcnt; k++)
                        {
                            s = i.ToString(sPre);
                            newrow["ChequeNo"] = s;
                            newrow["AccountId"] = m_lAccountId;
                            newrow["CompanyId"] = BsfGlobal.g_lCompanyId;
                            s = null;
                        }
                   }
                    else
                    {
                        newrow["ChequeNo"] = i.ToString();
                        newrow["AccountId"] = m_lAccountId;
                        newrow["CompanyId"] = BsfGlobal.g_lCompanyId;
                    }
                    ChequeMaster.DtChequeNo.Rows.Add(newrow);

               }

                grdChqNo.DataSource = null;
                grdViewChqNo.Columns.Clear();
                grdChqNo.DataSource = ChequeMaster.DtChequeNo;
                grdViewChqNo.PopulateColumns();
                grdViewChqNo.Columns["CompanyId"].Visible = false;
                grdViewChqNo.Columns["AccountId"].Visible = false;
                grdViewChqNo.Columns["ChequeNo"].Width = 100;
                grdChqNo.Refresh();
            }
            
            else
            {
                ChequeMaster.DtChequeNo = null;
                ChequeMaster.DtChequeNo = new DataTable();
                ChequeMaster.DtChequeNo.Columns.Add("ChequeNo");
                ChequeMaster.DtChequeNo.Columns.Add("AccountId");
                ChequeMaster.DtChequeNo.Columns.Add("CompanyId");
                DataRow newrow;
                for (int i = iStart; i <= iEnd; i++)
                {
                    newrow = ChequeMaster.DtChequeNo.NewRow();

                    newrow["ChequeNo"] = i.ToString();
                    newrow["AccountId"] = m_lAccountId;
                    newrow["CompanyId"] = BsfGlobal.g_lCompanyId;

                  ChequeMaster.DtChequeNo.Rows.Add(newrow);
                }
                grdChqNo.DataSource = ChequeMaster.DtChequeNo;
                grdViewChqNo.Columns["CompanyId"].Visible = false;
                grdViewChqNo.Columns["AccountId"].Visible = false;
                grdViewChqNo.Columns["ChequeNo"].Width = 150;
                grdChqNo.Refresh();
            }
            m_dtChqNo = (DataTable)grdChqNo.DataSource;
        }

        private void Fill_ChqNoDet_Grid()
        {
            bDW = false;
            dtDataChqNo = ChequeBL.Get_ChequeNo_Det(iChequeId);
            grdChqNoDet.DataSource = dtDataChqNo;

            grdViewChqNoDet.OptionsBehavior.Editable = true;
            grdViewChqNoDet.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
            grdViewChqNoDet.Columns["ChequeTransId"].Visible = false;
            grdViewChqNoDet.Columns["ChequeId"].Visible = false;

            grdViewChqNoDet.Columns["ChequeNo"].Width = 50;
            grdViewChqNoDet.Columns["ChequeNo"].OptionsColumn.AllowEdit = false;

            grdViewChqNoDet.Columns["Used"].Width = 40;
            grdViewChqNoDet.Columns["Used"].OptionsColumn.AllowEdit = false;

            grdViewChqNoDet.Columns["Cancel"].Width = 40;
            grdViewChqNoDet.Columns["Cancel"].OptionsColumn.AllowEdit = true;

            grdViewChqNoDet.Columns["CancelDate"].Width = 50;
            grdViewChqNoDet.Columns["CancelDate"].OptionsColumn.AllowEdit = true;

            grdViewChqNoDet.Columns["Remarks"].Width = 120;
            grdViewChqNoDet.Columns["Remarks"].OptionsColumn.AllowEdit = true;

            RepositoryItemCheckEdit chkCancel = new RepositoryItemCheckEdit();
            grdViewChqNoDet.Columns["Cancel"].ColumnEdit = chkCancel;
            chkCancel.CheckedChanged += chkCancel_CheckedChanged;
            RepositoryItemDateEdit deCancel = new RepositoryItemDateEdit();
            grdViewChqNoDet.Columns["CancelDate"].ColumnEdit = deCancel;

            grdViewChqNoDet.ShownEditor += grdViewChqNoDet_ShownEditor;
            grdViewChqNoDet.HiddenEditor += grdViewChqNoDet_HiddenEditor;

            grdViewChqNoDet.OptionsCustomization.AllowFilter = false;
            grdViewChqNoDet.OptionsBehavior.AllowIncrementalSearch = true;
            grdViewChqNoDet.OptionsView.ShowAutoFilterRow = false;
            grdViewChqNoDet.OptionsView.ShowViewCaption = false;
            grdViewChqNoDet.OptionsView.ShowFooter = false;
            grdViewChqNoDet.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            grdViewChqNoDet.OptionsSelection.InvertSelection = true;
            grdViewChqNoDet.OptionsView.ColumnAutoWidth = true;
            grdViewChqNoDet.Appearance.HeaderPanel.Font = new Font(grdViewChqNoDet.Appearance.HeaderPanel.Font, FontStyle.Bold);
            grdViewChqNoDet.FocusedRowHandle = 0;
            grdViewChqNoDet.FocusedColumn = grdViewChqNoDet.VisibleColumns[0];
            grdViewChqNoDet.OptionsNavigation.EnterMoveNextColumn = true;

            bDW = true;
        }

        void chkCancel_CheckedChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.CheckEdit editor = (DevExpress.XtraEditors.CheckEdit)sender;
            if (editor.Checked == true)
            {
                grdViewChqNoDet.SetRowCellValue(grdViewChqNoDet.FocusedRowHandle, "Cancel", true);
                grdViewChqNoDet.SetRowCellValue(grdViewChqNoDet.FocusedRowHandle, "CancelDate", DateTime.Now.ToString("dd/MMM/yyyy"));
            }
            else
            {
                grdViewChqNoDet.SetRowCellValue(grdViewChqNoDet.FocusedRowHandle, "CancelDate", null);
            }
        }

        #endregion

        private void Set_VGridCashInfo()
        {
            if (m_sType == null) return;
            vGridCashDet.Rows.Clear();

            #region Cash Name
            
            EditorRow editorRow1 = new EditorRow();
            editorRow1 = new EditorRow();
            editorRow1.Name = "CashName";
            editorRow1.Properties.Caption = "Cash Name";
            RepositoryItemTextEdit txtCashName= new RepositoryItemTextEdit() { MaxLength = 50 };
            editorRow1.Properties.RowEdit = txtCashName;
            txtCashName.EditValueChanged += txtCashName_EditValueChanged;
           
            if (m_lAccountId!=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("BankName").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;

            vGridCashDet.Rows.Add(editorRow1);

            #endregion

            #region New Group

            editorRow1 = new EditorRow() { Name = "NewGroup" };
            editorRow1.Properties.Caption = "New Group ";
            RepositoryItemCheckEdit chkNewGroup = new RepositoryItemCheckEdit() { NullStyle = StyleIndeterminate.Unchecked, GlyphAlignment = DevExpress.Utils.HorzAlignment.Near, Caption = "" };
            editorRow1.Properties.RowEdit = chkNewGroup;
            chkNewGroup.CheckedChanged += chkNewGroup_CheckedChanged;
            editorRow1.Properties.RowEdit = chkNewGroup;

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
                
            vGridCashDet.Rows.Add(editorRow1);

            #endregion

            #region Group Name
            
            editorRow1 = new EditorRow() { Name = "GroupName" };
            editorRow1.Properties.Caption = "Group Name";
            RepositoryItemTextEdit txtGroupName = new RepositoryItemTextEdit() { MaxLength = 50 };
            editorRow1.Properties.RowEdit = txtGroupName;
            editorRow1.Properties.Value = "";
            txtGroupName.EditValueChanged += txtGroupName_EditValueChanged;
            vGridCashDet.Rows.Add(editorRow1);

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;

            #endregion
            
            #region Under Group
            editorRow1 = new EditorRow() { Name = "UnderGroup" };
            editorRow1.Properties.Caption = "Under Group";
            RepositoryItemLookUpEdit cboCashGroup = new RepositoryItemLookUpEdit();
            editorRow1.Properties.RowEdit = cboCashGroup;
            DataView dvDataNew = new DataView(m_dtGroup) { RowFilter = "IsCash=1" };
            DataTable dtData = dvDataNew.ToTable();

            cboCashGroup.DataSource = null;
            cboCashGroup.Columns.Clear();
            cboCashGroup.NullText = "--Select Group--";
            cboCashGroup.DataSource = dtData;
            cboCashGroup.ForceInitialize();
            cboCashGroup.PopulateColumns();
            cboCashGroup.DisplayMember = "AccountName";
            cboCashGroup.ValueMember = "AccountId";
            cboCashGroup.Columns["AccountId"].Visible = false;
            cboCashGroup.Columns["AccountName"].Visible = true;
            cboCashGroup.Columns["LevelNo"].Visible = false;
            cboCashGroup.Columns["IsCash"].Visible = false;
            cboCashGroup.Columns["IsBank"].Visible = false;
            cboCashGroup.TextEditStyle = TextEditStyles.DisableTextEditor;
            cboCashGroup.ShowHeader = false;
            cboCashGroup.ShowFooter = false;
            dvDataNew.Dispose();
            dtData.Dispose();

            if (dtData.Rows.Count != 0)
            {
                cboCashGroup.EditValueChanged += cboCashGroup_EditValueChanged;
            }
            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            
            dvDataNew.Dispose();
            dtData.Dispose();
            

            if (m_lAccountId!=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("ParentAccountId").ToString();
            else
                editorRow1.Properties.Value = null;
            
            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;

            vGridCashDet.Rows.Add(editorRow1);

            #endregion
        }

        void cboCashGroup_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.LookUpEdit editor = (DevExpress.XtraEditors.LookUpEdit)sender;
            vGridCashDet.Rows["UnderGroup"].Properties.Value = editor.EditValue;
        }

        void txtGroupName_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridCashDet.Rows["GroupName"].Properties.Value = editor.EditValue;
        }

        void txtCashName_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridCashDet.Rows["CashName"].Properties.Value = editor.EditValue;
        }

        void chkNewGroup_CheckedChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.CheckEdit editor = (DevExpress.XtraEditors.CheckEdit)sender;
            vGridCashDet.Rows["NewGroup"].Properties.Value = editor.EditValue;

            if ((bool)vGridCashDet.Rows["NewGroup"].Properties.Value == true)
            {
                vGridCashDet.Rows["GroupName"].Properties.ReadOnly = false;
                vGridCashDet.Rows["UnderGroup"].Properties.ReadOnly = true;
                vGridCashDet.Rows["UnderGroup"].Properties.Value = null;
            }
            else
            {
                vGridCashDet.Rows["GroupName"].Properties.ReadOnly = true;
                vGridCashDet.Rows["UnderGroup"].Properties.ReadOnly = false;
            }
        }

        private void Set_VGridBankInfo()
        {
            vGridBankDet.Rows.Clear();

            # region Bank Name

            EditorRow editorRow1 = new EditorRow();
            editorRow1 = new EditorRow();
            editorRow1.Name = "BankName";
            editorRow1.Properties.Caption = "Bank Name";
            RepositoryItemTextEdit txtBankName = new RepositoryItemTextEdit() { MaxLength = 50 };
            editorRow1.Properties.RowEdit = txtBankName;
            txtBankName.EditValueChanged += txtBankName_EditValueChanged;
           
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("BankName").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            # region Branch Name

            editorRow1 = new EditorRow() { Name = "Branch" };
            editorRow1.Properties.Caption = "Branch";
            RepositoryItemTextEdit txtBranch = new RepositoryItemTextEdit() { MaxLength = 50 };
            editorRow1.Properties.RowEdit = txtBranch;
            txtBranch.EditValueChanged += txtBranch_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("Branch").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);
            
            #endregion

            #region Address

            editorRow1 = new EditorRow() { Name = "AccountNo" };
            editorRow1.Properties.Caption = "Account Number";
            RepositoryItemTextEdit txtAccNo = new RepositoryItemTextEdit() ;
            editorRow1.Properties.RowEdit = txtAccNo;
            txtAccNo.EditValueChanged += new EventHandler(txtAccNo_EditValueChanged);

            if (m_lAccountId != 0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("AccountNo").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);
            #endregion

            #region Address
            
            editorRow1 = new EditorRow() { Name = "Address" };
            editorRow1.Properties.Caption = "Address";
            RepositoryItemTextEdit txtAddress = new RepositoryItemTextEdit() { MaxLength = 100 };
            editorRow1.Properties.RowEdit = txtAddress;
            txtAddress.EditValueChanged += txtAddress_EditValueChanged;
            
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("Address1").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);
            #endregion

            #region Contact Person

            editorRow1 = new EditorRow() { Name = "Contact" };
            editorRow1.Properties.Caption = "Contact Person";
            RepositoryItemTextEdit txtContact = new RepositoryItemTextEdit() { MaxLength = 50 };
            editorRow1.Properties.RowEdit = txtContact;
            txtContact.EditValueChanged += txtContact_EditValueChanged;
           
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("ContactPerson").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region Mobile

            editorRow1 = new EditorRow() { Name = "Mobile" };
            editorRow1.Properties.Caption = "Mobile";
            RepositoryItemTextEdit txtMobile = new RepositoryItemTextEdit() { MaxLength = 100 };
            editorRow1.Properties.RowEdit = txtMobile;
            txtMobile.EditValueChanged += txtMobile_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("Mobile").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region Phone

            editorRow1 = new EditorRow() { Name = "Phone" };
            editorRow1.Properties.Caption = "Phone";
            RepositoryItemTextEdit txtPhone = new RepositoryItemTextEdit() { MaxLength=50 };
            editorRow1.Properties.RowEdit = txtPhone;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("Phone").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region Fax

            editorRow1 = new EditorRow() { Name = "Fax" };
            editorRow1.Properties.Caption = "Fax";
            RepositoryItemTextEdit txtFax = new RepositoryItemTextEdit() { MaxLength = 20 };
            editorRow1.Properties.RowEdit = txtFax;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("Fax").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region Country

            editorRow1 = new EditorRow() { Name = "Country" };
            editorRow1.Properties.Caption = "Country";
            RepositoryItemTextEdit txtCountry = new RepositoryItemTextEdit() { MaxLength = 50 };
            editorRow1.Properties.RowEdit = txtCountry;
            txtCountry.EditValueChanged += txtCountry_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("Country").ToString();

            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);
            #endregion

            #region State
            editorRow1 = new EditorRow() { Name = "State" };
            editorRow1.Properties.Caption = "State";
            RepositoryItemTextEdit txtState = new RepositoryItemTextEdit() { MaxLength = 50 };
            editorRow1.Properties.RowEdit = txtState;
            txtState.EditValueChanged += txtState_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("State").ToString();
            else

                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);
            #endregion

            #region City
            editorRow1 = new EditorRow() { Name = "City" };
            editorRow1.Properties.Caption = "City";
            RepositoryItemTextEdit txtCity = new RepositoryItemTextEdit() { MaxLength = 50 };
            editorRow1.Properties.RowEdit = txtCity;
            txtCity.EditValueChanged += txtCity_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("City").ToString();
            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);
            #endregion

            #region IFSC
            editorRow1 = new EditorRow() { Name = "IFSCCode" };
            editorRow1.Properties.Caption = "IFSC Code ";
            RepositoryItemTextEdit txtIFSC = new RepositoryItemTextEdit(){ MaxLength=20 };
            editorRow1.Properties.RowEdit = txtIFSC;
            txtIFSC.EditValueChanged += txtIFSC_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("IFSCCode").ToString();

            else
                editorRow1.Properties.Value = "";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region CA Limit
            
            editorRow1 = new EditorRow() { Name = "CALimit" };
            editorRow1.Properties.Caption = "CALimit ";
            RepositoryItemTextEdit txtCALimit= new RepositoryItemTextEdit() { MaxLength = 12 };
            editorRow1.Properties.RowEdit = txtCALimit;
            txtCALimit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            
            txtCALimit.Mask.EditMask = "##########.000";
            txtCALimit.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtCALimit.Spin += txtEdit_Spin;
            txtCALimit.KeyPress += txtEdit_KeyPress;
            txtCALimit.EditValueChanged += txtCALimit_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("CALimit").ToString();
            else
                editorRow1.Properties.Value = "0";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = clsStatic.sFormatAmt;
            vGridBankDet.Rows.Add(editorRow1);
            #endregion

            #region BG Limit

            editorRow1 = new EditorRow() { Name = "BGLimit" };
            editorRow1.Properties.Caption = "BGLimit";
            RepositoryItemTextEdit txtBGLimit = new RepositoryItemTextEdit() { MaxLength = 12 };
            editorRow1.Properties.RowEdit = txtBGLimit;
            txtBGLimit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtBGLimit.Mask.EditMask = "##########.000"; ;
            txtBGLimit.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtBGLimit.Spin += txtEdit_Spin;
            txtBGLimit.KeyPress += txtEdit_KeyPress;
            txtBGLimit.EditValueChanged += txtBGLimit_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("BGLimit").ToString();
            else
                editorRow1.Properties.Value = "0";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = clsStatic.sFormatAmt;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region OD Limit
            editorRow1 = new EditorRow() { Name = "ODLimit" };
            editorRow1.Properties.Caption = "ODLimit";
            RepositoryItemTextEdit txtODLimit = new RepositoryItemTextEdit() { MaxLength = 12 };
            editorRow1.Properties.RowEdit = txtODLimit;
            txtODLimit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtODLimit.Mask.EditMask = "##########.000"; ;
            txtODLimit.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtODLimit.Spin += txtEdit_Spin;
            txtODLimit.KeyPress += txtEdit_KeyPress;
            txtODLimit.EditValueChanged += txtODLimit_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("ODLimit").ToString();
            else
                editorRow1.Properties.Value = "0";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = clsStatic.sFormatAmt;

            #endregion

            #region LC Limit

            editorRow1 = new EditorRow() { Name = "LCLimit" };
            editorRow1.Properties.Caption = "LCLimit";
            RepositoryItemTextEdit txtLCLimit = new RepositoryItemTextEdit();
            editorRow1.Properties.RowEdit = txtLCLimit;
            txtLCLimit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtLCLimit.Mask.EditMask = "##########.000"; ;
            txtLCLimit.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtLCLimit.Spin += txtEdit_Spin;
            txtLCLimit.KeyPress += txtEdit_KeyPress;
            txtLCLimit.EditValueChanged += txtLCLimit_EditValueChanged;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("LCLimit").ToString();
            else
                editorRow1.Properties.Value = "0";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = clsStatic.sFormatAmt;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region LC Duration
           
            editorRow1 = new EditorRow() { Name = "LCDuration" };
            editorRow1.Properties.Caption = "LCDuration";
            RepositoryItemTextEdit txtLCDuration = new RepositoryItemTextEdit() { MaxLength = 3 };
            txtLCDuration.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtLCDuration.Mask.EditMask = "N0";
            txtLCDuration.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtLCDuration.Spin += txtEdit_Spin;
            txtLCDuration.KeyPress += txtEdit_KeyPress;
            txtLCDuration.EditValueChanged += txtLCDuration_EditValueChanged;
            editorRow1.Properties.RowEdit = txtLCDuration;
            if (m_lAccountId !=0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("LCDuration").ToString();
            else
                editorRow1.Properties.Value = "0";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = "N0";
            vGridBankDet.Rows.Add(editorRow1);
            
            #endregion

            #region New Group
            editorRow1 = new EditorRow() { Name = "NewGroup" };
            editorRow1.Properties.Caption = "New Group ";
            RepositoryItemCheckEdit chkNewBankGroup = new RepositoryItemCheckEdit() { NullStyle = StyleIndeterminate.Unchecked, GlyphAlignment = DevExpress.Utils.HorzAlignment.Near, Caption = "" };
            editorRow1.Properties.RowEdit = chkNewBankGroup;
            chkNewBankGroup.CheckedChanged += chkNewBankGroup_CheckedChanged;

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region Group Name
            editorRow1 = new EditorRow() { Name = "GroupName" };
            editorRow1.Properties.Caption = "Group Name";
            RepositoryItemTextEdit txtBankGroup= new RepositoryItemTextEdit();
            txtBankGroup.EditValueChanged += txtBankGroup_EditValueChanged;
            editorRow1.Properties.RowEdit = txtBankGroup;
            //if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);

            #endregion
                        
            #region Under Group
            editorRow1 = new EditorRow() { Name = "UnderGroup" };
            editorRow1.Properties.Caption = "Under Group";
            RepositoryItemLookUpEdit cboBankGroup = new RepositoryItemLookUpEdit();
            editorRow1.Properties.RowEdit = cboBankGroup;
            DataView dvDataNew = new DataView(m_dtGroup) { RowFilter = "IsBank=1" };
            DataTable dtData = dvDataNew.ToTable();
            cboBankGroup.Columns.Clear();
            cboBankGroup.NullText = "--Select Group--";
            cboBankGroup.DataSource = dtData;
            cboBankGroup.ForceInitialize();
            cboBankGroup.PopulateColumns();
            cboBankGroup.DisplayMember = "AccountName";
            cboBankGroup.ValueMember = "AccountId";
            cboBankGroup.Columns["AccountId"].Visible = false;
            cboBankGroup.Columns["AccountName"].Visible = true;
            cboBankGroup.Columns["LevelNo"].Visible = false;
            cboBankGroup.Columns["IsCash"].Visible = false;
            cboBankGroup.Columns["IsBank"].Visible = false;
            cboBankGroup.TextEditStyle = TextEditStyles.DisableTextEditor;
            cboBankGroup.ShowHeader = false;
            cboBankGroup.ShowFooter = false;
            dvDataNew.Dispose();
            dtData.Dispose();
            dvDataNew.Dispose();
            dtData.Dispose();
            cboBankGroup.EditValueChanged += cboBankGroup_EditValueChanged;

            if (m_lAccountId != 0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("ParentAccountId").ToString();
            else
                editorRow1.Properties.Value = null;

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;            
            vGridBankDet.Rows.Add(editorRow1);

            #endregion

            #region Cheque Info

            if (m_lAccountId != 0)
            {
                editorRow1 = new EditorRow() { Name = "ChequeDet" };
                editorRow1.Properties.Caption = "Cheque Info";
                RepositoryItemButtonEdit btnCheque = new RepositoryItemButtonEdit() { TextEditStyle = TextEditStyles.HideTextEditor };
                btnCheque.ButtonClick += btnCheque_ButtonClick;
                editorRow1.Properties.RowEdit = btnCheque;
                
                vGridBankDet.Rows.Add(editorRow1);
            }
            #endregion

            #region Cheque Validity
            editorRow1 = new EditorRow() { Name = "Validity" };
            editorRow1.Properties.Caption = "Validity (In Days)";
            RepositoryItemTextEdit txtValidity = new RepositoryItemTextEdit() { MaxLength = 3 };
            editorRow1.Properties.RowEdit = txtValidity;
            txtODLimit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtODLimit.Mask.EditMask = "###"; ;
            txtODLimit.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtODLimit.Spin += txtEdit_Spin;
            txtODLimit.KeyPress += txtEdit_KeyPress;
            txtODLimit.EditValueChanged += txtODLimit_EditValueChanged;
            if (m_lAccountId != 0)
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("Validity").ToString();
            else
                editorRow1.Properties.Value = "90";

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true;
            vGridBankDet.Rows.Add(editorRow1);
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = "N0";
            #endregion

            #region ReportName
            editorRow1 = new EditorRow() { Name = "ReportName" };
            editorRow1.Properties.Caption = "Report Name";
            btnReport = new RepositoryItemButtonEdit() { TextEditStyle = TextEditStyles.DisableTextEditor };
            btnReport.ButtonClick += new ButtonPressedEventHandler(btnReport_ButtonClick);
            editorRow1.Properties.RowEdit = btnReport;
            if (m_lAccountId != 0)
            {
                editorRow1.Properties.Value = grdCashBankView.GetFocusedRowCellValue("ReportName").ToString();
            }
            else
                editorRow1.Properties.Value = null;

            if (m_bAdd == false && m_lAccountId == 0) editorRow1.Properties.ReadOnly = true; 

            vGridBankDet.Rows.Add(editorRow1);

            #endregion

        }

        void txtAccNo_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["AccountNo"].Properties.Value = editor.EditValue;
        }

        void btnReport_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            s_FilePath = "";
            OpenFileDialog F_Dialog = new OpenFileDialog() { Filter = "Crystal Report Files (*.Rpt)|*.rpt"};
            DialogResult FD_Result = F_Dialog.ShowDialog();
            if (FD_Result == System.Windows.Forms.DialogResult.OK)
            {
                s_FilePath = F_Dialog.SafeFileName;
                vGridBankDet.SetCellValue(vGridBankDet.Rows["ReportName"], vGridBankDet.FocusedRecord, s_FilePath);
                btnReport.TextEditStyle = TextEditStyles.DisableTextEditor;
                vGridBankDet.Rows["ReportName"].Properties.Value = s_FilePath;
                SendKeys.Send("{UP}");
                SendKeys.Send("{DOWN}");
                vGridBankDet.Update();
            }
            
            else { return; }
        }

        void btnCheque_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (BsfGlobal.FindPermission("Cheque-View") == false)
            {
                MessageBox.Show("No Rights to proceed this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            barbtnChqAdd.Enabled = true;
            barbtnChqEdit.Enabled = true;
            barbtnChqDelete.Enabled = true;
            Set_vGridChqInfo();
            Fill_ChqDet_Grid();
            dwBank.Hide();
            dwCheque.Show();
            dwCheque.Select();
            dwChqNoDet.Hide();
            dwDetails.Hide();
        }

        private void Set_vGridChqInfo()
        {
            vGridChqInfo.Rows.Clear();

            #region Start No

            EditorRow editorRow1 = new EditorRow();
            editorRow1 = new EditorRow();
            editorRow1.Name = "StartNo";
            editorRow1.Properties.Caption = "Start No";
            RepositoryItemTextEdit txtStartNo = new RepositoryItemTextEdit() { MaxLength = 10, AllowNullInput = DevExpress.Utils.DefaultBoolean.False };
            txtStartNo.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtStartNo.Mask.EditMask = "n0";

            txtStartNo.Mask.EditMask = "-?\\d{1,10}";
            txtStartNo.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            
            //txtStartNo.Spin += txtEdit_Spin;
            //txtStartNo.KeyPress += txtEdit_KeyPress;
            txtStartNo.EditValueChanged += txtStartNo_EditValueChanged;
            txtStartNo.Leave += txtStartNo_Leave;
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = "N0";
            editorRow1.Properties.RowEdit = txtStartNo;
            editorRow1.Properties.Value = "";

            vGridChqInfo.Rows.Add(editorRow1);
           
            #endregion
           
            #region Width

            editorRow1 = new EditorRow() { Name = "Width" };
            editorRow1.Properties.Caption = "Width";
            RepositoryItemTextEdit txtWidth = new RepositoryItemTextEdit() { MaxLength = 2 };
            txtWidth.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtWidth.Mask.EditMask = "N0";
            txtWidth.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtWidth.Spin += txtEdit_Spin;
            txtWidth.KeyPress += txtEdit_KeyPress;
            txtWidth.EditValueChanged += txtWidth_EditValueChanged;
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = "N0";

            editorRow1.Properties.RowEdit = txtWidth;
            editorRow1.Properties.Value = "";

            vGridChqInfo.Rows.Add(editorRow1);
            #endregion

            #region Total Cheques

            editorRow1 = new EditorRow() { Name = "NoofLeaves" };
            editorRow1.Properties.Caption = "No. of Leaves";
            RepositoryItemTextEdit txtNoofLeaves = new RepositoryItemTextEdit() { MaxLength = 3 };
            txtNoofLeaves.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtNoofLeaves.Mask.EditMask = "N0";
            txtNoofLeaves.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            txtNoofLeaves.Spin += txtEdit_Spin;
            txtNoofLeaves.KeyPress += txtEdit_KeyPress;
            txtNoofLeaves.EditValueChanged += txtNoofLeaves_EditValueChanged;
            
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
            editorRow1.Properties.Format.FormatString = "N0";
            editorRow1.Properties.RowEdit = txtNoofLeaves;



            editorRow1.Properties.Value = "";

            vGridChqInfo.Rows.Add(editorRow1);
            #endregion

            #region EndNo 
            editorRow1 = new EditorRow() { Name = "EndNo" };
            editorRow1.Properties.Caption = "End No";
            RepositoryItemTextEdit txtEndNo = new RepositoryItemTextEdit() { MaxLength = 10 };
            editorRow1.Properties.RowEdit = txtEndNo;
         
            editorRow1.Properties.Value = "";
            editorRow1.Properties.ReadOnly = true;

            vGridChqInfo.Rows.Add(editorRow1);

            #endregion

            #region ReceivedDate

            editorRow1 = new EditorRow() { Name = "RecdDate" };
            editorRow1.Properties.Caption = "Received Date";
            RepositoryItemDateEdit txtRecdDate = new RepositoryItemDateEdit();
            txtRecdDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            txtRecdDate.Mask.EditMask = "dd/MM/yyyy";
            editorRow1.Properties.RowEdit = txtRecdDate;
            txtRecdDate.EditValueChanged += txtRecdDate_EditValueChanged;
            editorRow1.Properties.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            editorRow1.Properties.Format.FormatString = "dd/MM/yyyy";
            editorRow1.Properties.Value = DateTime.Now;

            vGridChqInfo.Rows.Add(editorRow1);
            
             #endregion

            #region Remarks

            editorRow1 = new EditorRow() { Name = "Remarks" };
            editorRow1.Properties.Caption = "Remarks ";
            RepositoryItemMemoEdit txtChqRemarks = new RepositoryItemMemoEdit();
            editorRow1.Properties.RowEdit = txtChqRemarks;
            txtChqRemarks.EditValueChanged += txtChqRemarks_EditValueChanged;

            editorRow1.Properties.Value = "";

            vGridChqInfo.Rows.Add(editorRow1);
              #endregion
        }

        void txtEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)45) e.Handled = true;
            
        }

        void txtEdit_Spin(object sender, SpinEventArgs e)
        {
            e.Handled = true;
        }

        void txtStartNo_Leave(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            if (editor.Text != "")
            {
                int iSNo = Convert.ToInt32(editor.Text);
                editor.Text = iSNo.ToString();
            }
            if (vGridChqInfo.Rows["Width"].Properties.Value.ToString() == "")
                vGridChqInfo.Rows["Width"].Properties.Value = Convert.ToString(editor.Text.Length);
        }

        void txtWidth_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridChqInfo.Rows["Width"].Properties.Value = editor.EditValue;
            if (vGridChqInfo.Rows["NoofLeaves"].Properties.Value.ToString() != string.Empty)
                Generate_ChqNo();
                
           
        }

        void txtChqRemarks_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.MemoEdit editor = (DevExpress.XtraEditors.MemoEdit)sender;
            vGridChqInfo.Rows["Remarks"].Properties.Value = editor.EditValue;
        }

        void txtRecdDate_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.DateEdit editor = (DevExpress.XtraEditors.DateEdit)sender;
            vGridChqInfo.Rows["RecdDate"].Properties.Value = editor.EditValue;
        }
     
        void txtNoofLeaves_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridChqInfo.Rows["NoofLeaves"].Properties.Value = editor.EditValue;
            if (vGridChqInfo.Rows["NoofLeaves"].Properties.Value.ToString() != string.Empty)
                Generate_ChqNo();
        }

        void txtStartNo_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridChqInfo.Rows["StartNo"].Properties.Value = editor.EditValue.ToString();
        }

        void cboBankGroup_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["UnderGroup"].Properties.Value = editor.EditValue;
        }

        void txtBankGroup_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["GroupName"].Properties.Value = editor.EditValue;
        }

        void txtLCDuration_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["LCDuration"].Properties.Value = editor.EditValue;
        }

        void txtLCLimit_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["LCLimit"].Properties.Value = editor.EditValue;
        }

        void txtODLimit_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["ODLimit"].Properties.Value = editor.EditValue;
        }

        void txtBGLimit_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["BGLimit"].Properties.Value = editor.EditValue;
        }

        void txtCALimit_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["CALimit"].Properties.Value = editor.EditValue;
        }

        void txtIFSC_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["IFSCCode"].Properties.Value = editor.EditValue;
        }

        void txtCity_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["City"].Properties.Value = editor.EditValue;
        }

        void txtState_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["State"].Properties.Value = editor.EditValue;
        }

        void txtCountry_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["Country"].Properties.Value = editor.EditValue;
        }

        void txtMobile_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["Mobile"].Properties.Value = editor.EditValue;
        }
        
        void txtContact_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["Contact"].Properties.Value = editor.EditValue;
        }

        void txtAddress_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["Address"].Properties.Value = editor.EditValue;
        }

        void txtBranch_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["Branch"].Properties.Value = editor.EditValue;
        }

        void txtBankName_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            vGridBankDet.Rows["BankName"].Properties.Value = editor.EditValue;
        }

        void chkNewBankGroup_CheckedChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.CheckEdit editor = (DevExpress.XtraEditors.CheckEdit)sender;
            vGridBankDet.Rows["NewGroup"].Properties.Value = editor.EditValue;

            if ((bool)vGridBankDet.Rows["NewGroup"].Properties.Value == true)
            {
                vGridBankDet.Rows["GroupName"].Properties.ReadOnly = false;
                vGridBankDet.Rows["UnderGroup"].Properties.ReadOnly = true;
                vGridBankDet.Rows["UnderGroup"].Properties.Value = null;
            }
            else
            {
                vGridBankDet.Rows["GroupName"].Properties.ReadOnly = true;
                vGridBankDet.Rows["UnderGroup"].Properties.ReadOnly = false;
            }
        }

        private void btnBankInfo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dwBank.Show();
            dwBankInfo.Show();
            dwCashInfo.Hide();
            dwCheque.Hide();
            dwChqNoDet.Hide();
            dwBankInfo.Select();
        }

        private void vGridChqInfo_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            //if (e.Row.Name=="NoofLeaves" && e.Value.ToString()!=string.Empty)
            //{
            //    Generate_ChqNo();
            //}
        }

        private void vGridBankDet_CellValueChanging(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            DevExpress.XtraVerticalGrid.Rows.BaseRow Row = vGridBankDet.Rows["ReportName"];
                       
            vGridBankDet.SetCellValue(Row,e.RecordIndex, s_FilePath);
        }

        private void vGridBankDet_ShownEditor(object sender, EventArgs e)
        {
            
            BaseRow row = vGridBankDet.Rows["ReportName"];
            vGridBankDet.SetCellValue(row, vGridBankDet.FocusedRecord, s_FilePath);
            vGridBankDet.UpdateFocusedRecord();
            vGridBankDet.ActiveEditor.IsModified = true;
            vGridBankDet.PostEditor();
        }

        private void btnChqNoDet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bDW = true;
            if (grdViewCheque.RowCount<=0) return;
            iChequeId = Convert.ToInt32(grdViewCheque.GetRowCellValue(grdViewCheque.FocusedRowHandle, "ChequeId").ToString());
            Fill_ChqNoDet_Grid();
            dwChqNoDet.Show();
            dwChqNoDet.Select();
            bDW = false;
        }

        private void radDockBankCashDet_ActiveWindowChanged(object sender, Telerik.WinControls.UI.Docking.DockWindowEventArgs e)
        {
            if (radDockBankCashDet.ActiveWindow.Name == "dwCheque" && bDW==false)
            {
                dwChqNoDet.Hide();
            }
        }
    }
}

            
