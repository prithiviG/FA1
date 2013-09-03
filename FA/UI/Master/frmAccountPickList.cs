using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using FA.BusinessLayer;

namespace FA
{
    public partial class frmAccountPickList : DevExpress.XtraEditors.XtraForm
    {
        #region Variable
        readonly DataTable m_tGroup=null;
        DataTable m_tAccount;
        ArrayList m_aAccountId;
        bool m_bCashBankOnly;
        #endregion

        #region Constructor
        public frmAccountPickList()
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


        public ArrayList Execute (bool arg_bCashBank)
        {
            m_bCashBankOnly = arg_bCashBank;

            ShowDialog();
            return m_aAccountId;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            m_aAccountId = new ArrayList();
            Close();
        }

        private void frmAcountPickList_Load(object sender, EventArgs e)
        {
            //GetAccountsDir();
            defaultLookAndFeel1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Blue";
            Fill_AccountsDir();
        }


        private void AddParent(TreeNode argNode, int ParentId, int LevelNo)
        {
            int iCtr = 0;
            int iNodeId = 0;
            DataView dvDataNew = null;
            DataTable dtDataNew = null;
            if (LevelNo == 1)
            {
                dvDataNew = new DataView(m_tGroup);
                dvDataNew.RowFilter = String.Format("AccountGroupId='{0}'", ParentId);
                dtDataNew = dvDataNew.ToTable();
            }
            else
            {
                dvDataNew = new DataView(m_tAccount);
                dvDataNew.RowFilter = String.Format("ParentAccountId='{0}'", ParentId);
                dtDataNew = dvDataNew.ToTable();
            }
            for (iCtr = 0; iCtr < dtDataNew.Rows.Count; iCtr++)
            {
                iNodeId = Convert.ToInt16(dtDataNew.Rows[iCtr]["AccountId"].ToString());
                var TNode = new TreeNode { Tag = iNodeId, Text = dtDataNew.Rows[iCtr]["AccountName"].ToString(), ImageIndex = LevelNo };
                argNode.Nodes.Add(TNode);
                AddParent(TNode, iNodeId, 2);
            }
        }

        private void Fill_AccountsDir()
        {
            //var bitmap = new Bitmap("images.jpg"); // or get it from resource
            //var iconHandle = bitmap.GetHicon();
            //var icon = System.Drawing.Icon.FromHandle(iconHandle);
            try
            {

                m_tAccount = new DataTable();
                m_tAccount = m_bCashBankOnly == false ? CompanyAccDirBL.GetAccDirPick() : CompanyAccDirBL.GetCashBankDirPick();

                tvAccDir.RootValue = 0;
                tvAccDir.ParentFieldName = "ParentAccountID";
                tvAccDir.KeyFieldName = "AccountID";
                tvAccDir.DataSource = m_tAccount;

                tvAccDir.Columns["AccountName"].Visible = true;
                tvAccDir.Columns["LastLevel"].Visible = false;

                //StyleFormatCondition condition = new StyleFormatCondition(DevExpress.XtraGrid.FormatConditionEnum.Equal, tvAccDir.Columns["LastLevel"], null, "Y");
                tvAccDir.Appearance.HeaderPanel.Font = new Font(tvAccDir.Appearance.HeaderPanel.Font, FontStyle.Bold);
                tvAccDir.Appearance.FooterPanel.Font = new Font(tvAccDir.Appearance.HeaderPanel.Font, FontStyle.Bold);
                tvAccDir.BestFitColumns();

                // tvAccDir.ExpandAll();
            }
            catch (Exception Except)
            {
                MessageBox.Show(Except.Message, "Error");
            }

        }
  
        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvAccDir.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvAccDir.CollapseAll();
        }


        private void cmdOK_Click(object sender, EventArgs e)
        {
            m_aAccountId = new ArrayList();

            foreach (TreeListNode ndParent in tvAccDir.Nodes)
            {
                if (ndParent.Level == 0) { GetAccountID(ndParent); }
            }
            Close();
        }


        private void GetAccountID(TreeListNode argNode)
        {
            object sstr;
            if (argNode.Checked == true)
            {
                if (argNode.Nodes.Count == 0) 
                {
                    sstr = argNode["AccountID"].ToString();
                    m_aAccountId.Add(sstr);
                }
            }
            foreach (TreeListNode ndChild in argNode.Nodes)
            {
                GetAccountID(ndChild);
            }

        }


        private void tvAccDir_AfterCheckNode(object sender, NodeEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
            Cursor.Current = Cursors.Default;
        }

        private void tvAccDir_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
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
                    if (check.Equals(state))
                        continue;

                    b = !b;
                    break;
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }
    }
}
