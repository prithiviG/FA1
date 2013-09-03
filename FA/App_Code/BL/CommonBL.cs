using System;
using System.Data;
using FA.DataLayer;

namespace FA.BusinessLayer
{
    class CommonBL
    {
        #region Methods

        internal static DataTable Get_Bank()
        {
            return CommonDL.Get_Bank();
        }

        internal static DataTable Get_CostCentre()
        {
            return CommonDL.Get_CostCentre();
        }
        #endregion

        internal static void Move_BillDetails()
        {
            CommonDL.Move_BillDetails();
        }

        internal static DataTable Get_Client()
        {
            return CommonDL.Get_Client();
        }

        internal static DataTable Get_AllProject()
        {
            return CommonDL.Get_AllProject();
        }

        internal static DataTable Get_AllBuyer()
        {
            return CommonDL.Get_AllBuyer();
        }

        internal static DataTable Get_AllFlat()
        {
            return CommonDL.Get_AllFlat();
        }

        internal static DataTable Get_AllVendor()
        {
            return CommonDL.Get_AllVendor();
        }

        internal static DataTable Get_Resource_Det()
        {
            return CommonDL.Get_Resource_Det();
        }

        public static int GetTaxSubLedger(int argQualId, int argiStateId, decimal argPercent, int argServiceTypeId)
        {
            return CommonDL.GetTaxSubLedger(argQualId, argiStateId, argPercent, argServiceTypeId);
        }

        internal static DataTable Get_TaxLedger()
        {
            return CommonDL.Get_TaxLedger();
        }

        internal static DataTable Get_BuyerReceiptInfo_Det()
        {
            return CommonDL.Get_BuyerReceiptInfo_Det();
        }

        internal static DataTable Get_FiscalYear_Det(int arg_iFYearId, int arg_iEntryId)
        {
            return CommonDL.Get_FiscalYear_Det(arg_iFYearId, arg_iEntryId);
        }

        internal static DataTable Get_SubLedger_AccountType()
        {
            return CommonDL.Get_SubLedger_AccountType();
        }
        internal static bool Check_Posting_Lock_FA(DateTime arg_dBillDate)
        {
            return CommonDL.Check_Posting_Lock_FA(arg_dBillDate);
        }
        internal static bool Check_Advance_Vendor()
        {
           return CommonDL.Check_Advance_Vendor();
        }

        internal static DataTable Get_Expense_Account()
        {
            return CommonDL.Get_Expense_Account();
        }

        internal static int Get_HO_CostCentre()
        {
            return CommonDL.Get_HO_CostCentre();
        }

        internal static DataTable Get_Pending_Bills(int arg_iAccId, string arg_sType)
        {
            return CommonDL.Get_Pending_Bills(arg_iAccId, arg_sType);
            
        }

        internal static DataTable Get_Pending_Bills_WriteOff(string arg_sPayType)
        {
            return CommonDL.Get_Pending_Bills_WriteOff(arg_sPayType);
        }

        internal static DataTable Get_WriteOff_Account()
        {
            return CommonDL.Get_WriteOff_Account();
        }
    }
}
