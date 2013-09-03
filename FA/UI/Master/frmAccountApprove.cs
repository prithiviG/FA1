using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FA.BusinessLayer;
using System.Collections;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.Utils.Drawing;

namespace FA
{
    public partial class frmAccountApprove : DevExpress.XtraEditors.XtraForm
    {
        #region Variables
        DataTable m_dtUsers = new DataTable();
        DataSet m_dsAccount = new DataSet();
        DataTable m_tAccount = new DataTable();
        int m_iUserId = 0;
        int m_iMaxLevel;
        ArrayList m_aAccList;
        bool m_bLoad;
        #endregion

        public frmAccountApprove()
        {
            InitializeComponent();
        }

        private void frmAccountApprove_Load(object sender, EventArgs e)
        {
            clsStatic.SetMyGraphics();
            Populate_Users();
            grdUsers.Focus();
            Populate_Account_Approval();
        }

        public void Execute()
        {
            Show();
        }

        private void Populate_Users()
        {
            m_dtUsers = UserBL.GetUserDet();
            grdUsers.DataSource = m_dtUsers;
            grdUsers.ForceInitialize();
            grdViewUsers.PopulateColumns();

            grdViewUsers.Columns["UserId"].Visible = false;
            grdViewUsers.Columns["UserName"].OptionsColumn.AllowEdit = false;
            grdViewUsers.Appearance.HeaderPanel.Font = new Font(grdViewUsers.Appearance.HeaderPanel.Font, FontStyle.Bold);

        }

        private void Populate_Account_Approval()
        {

            m_dsAccount = FinStmtsBL.Get_Account_ApproveDet(m_iUserId);

            m_iMaxLevel = 0;
            DataTable dt = new DataTable();
            dt = m_dsAccount.Tables["LevelNo"];

            if (dt.Rows.Count > 0) { m_iMaxLevel = Convert.ToInt32(dt.Rows[0]["LevelNo"].ToString()); }
            dt.Dispose();
            if (m_iMaxLevel != 0) { m_iMaxLevel = m_iMaxLevel - 1; }

            m_bLoad = true;
            m_tAccount = new DataTable();
            m_tAccount = m_dsAccount.Tables["IFRSFS"];

            tvAccView.RootValue = 0;
            tvAccView.ParentFieldName = "ParentAccountId";

            tvAccView.KeyFieldName = "AccountId";

            tvAccView.DataSource = m_tAccount;
            tvAccView.Columns["AccountName"].Caption = "Account Name";
            tvAccView.Columns["AccountName"].Visible = true;
            tvAccView.Columns["LastLevel"].Visible = false;
            tvAccView.Columns["UserId"].Visible = false;

            tvAccView.Columns["AccountName"].OptionsColumn.AllowEdit = false;

            tvAccView.Appearance.HeaderPanel.Font = new Font(tvAccView.Appearance.HeaderPanel.Font, FontStyle.Bold);
            tvAccView.Appearance.FooterPanel.Font = new Font(tvAccView.Appearance.HeaderPanel.Font, FontStyle.Bold);
            tvAccView.BestFitColumns();
            tvAccView.ExpandAll();
            m_bLoad = false;
           
        }

        private void btncancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void btnOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_aAccList = new ArrayList();

            foreach (TreeListNode ndParent in tvAccView.Nodes)
            {
                if (ndParent.Level == 0) { GetAccountId(ndParent); }
            }
            bool bSuccess = FinStmtsBL.Update_Account_Approve(m_aAccList, m_iUserId);
            if (bSuccess == true) Close();
        }

        private void GetAccountId(TreeListNode argNode)
        {
            object sAccId;
            if (argNode.Checked == true)
            {
                if (argNode["LastLevel"].ToString() == "Y")
                {
                    sAccId = argNode["AccountId"].ToString();
                    m_aAccList.Add(sAccId);
                }
            }
            foreach (TreeListNode ndChild in argNode.Nodes)
            {
                GetAccountId(ndChild);
            }

        }

        private void tvAccView_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
            Cursor.Current = Cursors.Default;
        }

        private static void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        private static void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;
                CheckState state;
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    if (!check.Equals(state))
                    {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }

        private void tvAccView_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }

        private void tvAccView_GetNodeDisplayValue(object sender, DevExpress.XtraTreeList.GetNodeDisplayValueEventArgs e)
        {
            if (m_bLoad == false) return;
            if (Convert.ToInt32(e.Node.GetValue("UserId").ToString()) > 0)
            {
                e.Node.CheckState = CheckState.Checked;
                Cursor.Current = Cursors.WaitCursor;
                SetCheckedChildNodes(e.Node, e.Node.CheckState);
                SetCheckedParentNodes(e.Node, e.Node.CheckState);
                Cursor.Current = Cursors.Default;

            }
        }

        private void tvAccView_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            if (e.Node.Level == 0 || e.Node.Level == 1) { e.NodeImageIndex = 0; }
            else { e.NodeImageIndex = 1; }
        }

        private void tvAccView_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node.Level == 0 || e.Node.Level == 1)
                e.Appearance.Font = new Font(e.Appearance.Font.Name, 10, FontStyle.Bold);
            else
                e.Appearance.Font = new Font(e.Appearance.Font.Name, 8, FontStyle.Regular);
        }

        private void grdViewUsers_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            GridView view = (GridView)sender;

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void grdViewUsers_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            m_iUserId = Convert.ToInt32(grdViewUsers.GetRowCellValue(grdViewUsers.FocusedRowHandle, "UserId"));
            Populate_Account_Approval();
        }

        private void tvAccView_CustomDrawNodeCheckBox(object sender, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            if (e.Node.CheckState == CheckState.Indeterminate && (sender as TreeList).LookAndFeel.UseWindowsXPTheme == false)
            {
                e.Painter.CalcObjectBounds(e.ObjectArgs);
                //ControlPaint.DrawCheckBox(e.Cache.Graphics, (e.ObjectArgs as CheckObjectInfoArgs).GlyphRect | ButtonState.Inactive);
                CheckObjectInfoArgs checkObjectInfoArgs = (CheckObjectInfoArgs)e.ObjectArgs;
                checkObjectInfoArgs.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style9;
                e.Handled = true;
            }
        }
    }
}