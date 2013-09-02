using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using FA.BusinessLayer;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;

namespace FA
{
    public partial class frmNarration : DevExpress.XtraEditors.XtraForm
    {
        #region Variable

                
        string sMode = "";
        bool bNarr;
        int iNarrId;

        #endregion

        #region Object


        readonly NarrationBL NarrationBL;

        #endregion

        #region Constructor

        public frmNarration()
        {
            InitializeComponent();
            NarrationBL = new NarrationBL();
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

        private void frmNarration_Load(object sender, EventArgs e)
        {
            defaultLookAndFeel1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Blue";

            Fill_Narration(Get_Narration());
            CheckPermission();
            clsStatic.SetMyGraphics();
            dgvRemarksView.Appearance.FocusedRow.BackColor = Color.Teal;
            dgvRemarksView.Appearance.FocusedRow.ForeColor = Color.White;
            dgvRemarksView.Appearance.HideSelectionRow.BackColor = Color.Teal;
            dgvRemarksView.Appearance.HideSelectionRow.ForeColor = Color.White;
        }

        private void frmfrmNarration_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                Close();
            }
        }

        #endregion

        #region Button Event

           #endregion

        #region Grid Event

        private void dgvRemarksView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            AssignData();
            iNarrId = NarrationBL.Update_Narration();
            if (NarrationBL.DBUpdate == true)
            {
                int val = 0;
                DataRow dr = dgvRemarksView.GetDataRow(dgvRemarksView.FocusedRowHandle); //("Total");

                val = iNarrId;
                dr["Narration Id"] = val;
                bNarr = false;
                sMode = "";
                iNarrId = 0;
            }
        }

        private void dgvRemarksView_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            if (dgvRemarksView.FocusedRowHandle >= 0)
            {

                iNarrId = Convert.ToInt16(dgvRemarksView.GetFocusedRowCellValue("NarrationId").ToString());
            }

            else
                iNarrId = 0;

        }

        private void dgvRemarksView_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (dgvRemarksView.FocusedRowHandle < 0)
            {
                if (BsfGlobal.FindPermission("Narration-Add") == false)
                {
                    e.Cancel = true;
                    MessageBox.Show("No Rights to Access this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                sMode = "I";
                bNarr = true;
            }
            else
            {
                if (BsfGlobal.FindPermission("Narration-Edit") == false)
                {
                    e.Cancel = true;
                    MessageBox.Show("No Rights to Access this event", "Build Super Fast ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                sMode = "U";
                bNarr = true;
            }
        }
        
        #endregion

        #region Function

        private void CheckPermission()
        {
            if (BsfGlobal.FindPermission("Narration-Add") == false) { dgvRemarksView.OptionsView.NewItemRowPosition = NewItemRowPosition.None; }
            if (BsfGlobal.FindPermission("Narration-Edit") == false) { dgvRemarksView.OptionsBehavior.Editable = false; }

        }
        private static DataTable Get_Narration()
        {
            return NarrationBL.Get_Narration();
        }

        private void Fill_Narration(DataTable dtNarration)
        {
            if (dgvRemarksView.FocusedRowHandle >= 0)
            {
                if (dtNarration.Rows.Count != 0)
                {
                    dgvRemarks.DataSource = dtNarration;
                    dgvRemarks.Focus();
                }
            }
            else
            {
                dgvRemarks.DataSource = dtNarration;
            }

            dgvRemarksView.Columns["Description"].BestFit();
            dgvRemarksView.Columns["NarrationId"].Visible = false;
           
        }
                
        public void AssignData()
        {
            
            NarrationBL.Mode = sMode;
            if (bNarr == true)
                NarrationBL.Description = dgvRemarksView.GetFocusedRowCellValue("Description").ToString();

            if (bNarr == true && NarrationBL.Mode != "I")
            {
                iNarrId = Convert.ToInt16(dgvRemarksView.GetFocusedRowCellValue("NarrationId").ToString());
                NarrationBL.DescId = iNarrId;
            }
            else
                NarrationBL.DescId = iNarrId;
        }
        #endregion
    }
}
