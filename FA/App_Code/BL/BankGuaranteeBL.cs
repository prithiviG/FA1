using System;
using System.Collections.Generic;
using System.Data;

namespace FA
{
    class BankGuaranteeBL
    {

        internal static int BGRegisterId { get; set; }
        internal static int BGRenewalId { get; set; }
        internal static int BGCommId { get; set; }
        internal static int BGCancelId { get; set; }
        internal static int BankId { get; set; }
        internal static DateTime BGDate { get; set; }
        internal static string BGNo { get; set; }
        internal static string BGType { get; set; }
        internal static DateTime BGFrom { get; set; }
        internal static DateTime BGTo { get; set; }
        internal static int ClaimDays { get; set; }
        internal static DateTime ValidTill { get; set; }
        internal static int ClientId { get; set; }
        internal static int ProjectId { get; set; }
        internal static string RefNo { get; set; }
        internal static Decimal BGAmount { get; set; }
        internal static string Remarks { get; set; }
        internal static Boolean IsCancel{ get; set; }
        internal static DateTime CancelDate { get; set; }

        internal static DataTable Get_GuaranteeType()
        {
            return BankGuaranteeDL.Get_GuaranteeType();
        }

        internal static bool Update_BankGuarantee()
        {
            return BankGuaranteeDL.Update_BankGuarantee();

        }

        internal static DataTable Get_BGRegister(int arg_iBankId)
        {
            return BankGuaranteeDL.Get_BGRegister(arg_iBankId);
        }


        internal static bool Delete_BG(int arg_iBGId)
        {
            return BankGuaranteeDL.Delete_BG(arg_iBGId);
        }

        internal static DataTable Get_RenewInfo(int arg_iBGId)
        {
            return BankGuaranteeDL.Get_RenewInfo(arg_iBGId);
        }

        internal static DataTable Get_CommissionInfo(int arg_iBGId)
        {
            return BankGuaranteeDL.Get_CommissionInfo(arg_iBGId);
        }

        internal static DataTable Get_CancelInfo(int arg_iBGId)
        {
            return BankGuaranteeDL.Get_CancelInfo(arg_iBGId);
        }


        internal static bool Update_BGRenewal()
        {
            return BankGuaranteeDL.Update_BGRenewal();
        }

        internal static void Get_RenewalMaxDate(int arg_iBGId, ref DateTime arg_dFDate)
        {
             BankGuaranteeDL.Get_RenewalMaxDate(arg_iBGId,ref arg_dFDate);
        }

        internal static bool Update_BGCommission()
        {
            return BankGuaranteeDL.Update_BGCommission();
        }

        internal static bool Delete_BGRenewal(int arg_iRenewId, int arg_iBGId)
        {
            return BankGuaranteeDL.Delete_BGRenewal(arg_iRenewId, arg_iBGId);
        }

        internal static bool Check_BGRenewalDet(int arg_iRenewalId)
        {
            return BankGuaranteeDL.Check_BGRenewalDet(arg_iRenewalId);
        }

        internal static bool Check_BGCommissionDet(int arg_iRenewId)
        {
            return BankGuaranteeDL.Check_BGCommissionDet(arg_iRenewId);
        }

        internal static bool Delete_BGCommission(int arg_iCommId, int arg_iBGId)
        {
            return BankGuaranteeDL.Delete_BGCommission(arg_iCommId, arg_iBGId);
        }
    }
}
