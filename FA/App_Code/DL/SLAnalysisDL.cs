using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;


using FA.BusinessLayer;

namespace FA.DataLayer
{
    class SLAnalysisDL
    {
        internal static decimal Get_SLType_OpeningBalance(int argSLTypeId, int argSLId, int arg_iCCId, string arg_sAccTypeIds)
        {
            decimal dAmt;
            string sSql = string.Empty;
            string sCond = string.Empty;
            
            if (arg_sAccTypeIds != string.Empty)
            {
                sCond = String.Format("AND AM.TypeId IN ({0})", arg_sAccTypeIds);
            }
            
            if (arg_iCCId <= 0)
            {
                sSql = String.Format("SELECT SUM(OpeningBal) OpeningBalance FROM dbo.SLAccount  " +
                                     "WHERE  SubLedgerId={1} AND CompanyId= {2} AND ParentAccountId IN (" +
                                     "SELECT SL.ParentAccountId FROM dbo.SLAccount SL " +
                                     "INNER JOIN [{3}].dbo.SubLedgerMaster SLM ON SL.SubLedgerId=SLM.SubLedgerId " +
                                     "INNER JOIN [{3}].dbo.AccountMaster AM ON AM.AccountId=SL.ParentAccountId " +
                                     "WHERE SLM.SubLedgerId={1} AND SLM.SubledgerTypeId={0} AND SL.CompanyId={2} {4}) ", 
                                     argSLTypeId, argSLId, BsfGlobal.g_lCompanyId, BsfGlobal.g_sFaDBName,sCond
                                     );
            }
            else
            {
                sSql = String.Format("SELECT SUM(OpeningBalance) OpeningBalance FROM dbo.CCAccount " +
                                     "WHERE  SubLedgerId={1} AND CompanyId= {2} AND CostCentreId={4} AND ParentAccountId IN (" +
                                     "SELECT SL.ParentAccountId FROM dbo.CCAccount SL " +
                                     "INNER JOIN [{3}].dbo.SubLedgerMaster SLM ON SL.SubLedgerId=SLM.SubLedgerId " +
                                     "INNER JOIN [{3}].dbo.AccountMaster AM ON AM.AccountId=SL.ParentAccountId " +
                                     "WHERE SLM.SubLedgerId={1} AND SLM.SubledgerTypeId={0} AND SL.CompanyId={2} AND SL.CostCentreId={4} {5}) ", 
                                     argSLTypeId, argSLId, BsfGlobal.g_lCompanyId, BsfGlobal.g_sFaDBName, arg_iCCId,sCond);
            }
            SqlDataAdapter da = new SqlDataAdapter(sSql, BsfGlobal.g_CompanyDB);
            DataTable dt = new DataTable();
            da.Fill(dt);
            da.Dispose();

            if (dt.Rows.Count > 0)
                dAmt = Convert.ToDecimal(clsStatic.IsNullCheck(dt.Rows[0]["OpeningBalance"].ToString(), datatypes.vartypenumeric));
            else
                dAmt = 0;
            dt.Dispose();
            BsfGlobal.g_CompanyDB.Close();
            return dAmt;
        }
        internal static DataTable Get_SLType_Det(DateTime arg_dAsOn)
        {
            string sSql = "";
            SqlDataAdapter sda = null;
            DataTable dt = new DataTable();
            try
            {
                BsfGlobal.OpenCompanyDB();

                if (SLAnalysisBL.CCId <= 0)
                {
                    sSql = String.Format("SELECT B.SubLedgerTypeId SLTypeId,B.SubLedgerTypeName SLTypeName, " +
                                        "CASE WHEN Debit-Credit>=0  THEN Debit-Credit ELSE 0 END Debit, " +
                                        "CASE WHEN (Debit-Credit<0 ) THEN ABS(Debit-Credit) ELSE 0 END Credit  " +
                                        "FROM (SELECT A.SubLedgerTypeId SLTypeId , SUM(Debit) Debit, SUM(credit) Credit " +
                                        "FROM (SELECT ET.SubLedgerTypeId,SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END) Debit, " +
                                        "SUM(CASE WHEN ET.TransType='C' THEN ABS(ET.Amount) ELSE 0 END) Credit " +
                                        "FROM EntryTrans ET WITH (READPAST) WHERE ET.VoucherDate<='{0}' AND ET.CompanyId={2}  GROUP BY SubLedgerTypeId " +
                                        "UNION ALL " +
                                        "SELECT SLT.SLTypeId,CASE WHEN Sum(SL.OpeningBalance)>0 THEN Sum(SL.OpeningBalance) ELSE 0 END  Debit, " +
                                        "CASE WHEN Sum(SL.OpeningBalance)<0 THEN ABS(Sum(SL.OpeningBalance)) ELSE 0 END  Credit " +
                                        "FROM SLAccount SL WITH (READPAST) INNER JOIN dbo.Account A ON SL.ParentAccountId=A.AccountId AND A.CompanyId=SL.CompanyId  " +
                                        "INNER JOIN  [{1}].dbo.AccountMaster AM ON A.AccountId=AM.AccountId " +
                                        "INNER JOIN  [{1}].dbo.SLAccountType SLT ON SLT.TypeId=AM.TypeId " +
                                        "WHERE  LastLevel='Y' AND SLTypeId<>0 AND SL.CompanyId={2} GROUP BY SLT.SLTypeId) A GROUP BY A.SubLedgerTypeId) A " +
                                        "RIGHT JOIN  [{1}].dbo.SubLedgerType B ON B.SubLedgerTypeId=A.SLTypeId " +
                                        "ORDER BY B.SubLedgerTypeName ", String.Format("{0:dd/MMM/yyyy}", arg_dAsOn), BsfGlobal.g_sFaDBName, BsfGlobal.g_lCompanyId);
                }
                else
                {
                    sSql = String.Format("SELECT B.SubLedgerTypeId SLTypeId,B.SubLedgerTypeName SLTypeName, " +
                                        "CASE WHEN Debit-Credit>=0  THEN Debit-Credit ELSE 0 END Debit, " +
                                        "CASE WHEN (Debit-Credit<0 ) THEN ABS(Debit-Credit) ELSE 0 END Credit  " +
                                        "FROM (SELECT A.SubLedgerTypeId SLTypeId , SUM(Debit) Debit, SUM(credit) Credit " +
                                        "FROM (SELECT ET.SubLedgerTypeId,SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END) Debit, " +
                                        "SUM(CASE WHEN ET.TransType='C' THEN ABS(ET.Amount) ELSE 0 END) Credit " +
                                        "FROM EntryTrans ET WITH (READPAST) WHERE ET.VoucherDate<='{0}' AND ET.CompanyId={2} AND ET.CostCentreId={3} GROUP BY SubLedgerTypeId " +
                                        "UNION ALL " +
                                        "SELECT SLT.SLTypeId,CASE WHEN Sum(SL.OpeningBalance)>0 THEN Sum(SL.OpeningBalance) ELSE 0 END  Debit, " +
                                        "CASE WHEN Sum(SL.OpeningBalance)<0 THEN ABS(Sum(SL.OpeningBalance)) ELSE 0 END  Credit " +
                                        "FROM CCAccount SL WITH (READPAST) INNER JOIN Account A ON SL.ParentAccountId=A.AccountId  AND A.CompanyId=SL.CompanyId  " +
                                        "INNER JOIN  [{1}].dbo.AccountMaster AM ON A.AccountId=AM.AccountId " +
                                        "INNER JOIN  [{1}].dbo.SLAccountType SLT ON SLT.TypeId=AM.TypeId " +
                                        "WHERE  LastLevel='Y' AND SLTypeId<>0 AND SL.CompanyId={2} AND SL.CostCentreId={3} GROUP BY SLT.SLTypeId) A GROUP BY A.SubLedgerTypeId) A " +
                                        "RIGHT JOIN  [{1}].dbo.SubLedgerType B ON B.SubLedgerTypeId=A.SLTypeId " +
                                        "ORDER BY B.SubLedgerTypeName ", String.Format("{0:dd/MMM/yyyy}", arg_dAsOn), BsfGlobal.g_sFaDBName, BsfGlobal.g_lCompanyId, SLAnalysisBL.CCId);
                }

                sda = new SqlDataAdapter(sSql, BsfGlobal.g_CompanyDB);
                sda.Fill(dt);

            }
            catch (Exception ex)
            {
                BsfGlobal.CustomException(ex.Message, ex.StackTrace); ;
            }
            finally
            {
                BsfGlobal.g_CompanyDB.Close();
            }
            return dt;
        }

        internal static DataTable Get_SLType_TransDet(int arg_iSLTypeId, DateTime arg_dAsOn, string arg_sAccTypeIds)
        {
            string sSql = string.Empty;
            string sCond = string.Empty;
            SqlDataAdapter sda = null;
            DataTable dt = new DataTable();
            try
            {
                if (arg_sAccTypeIds != string.Empty)
                {
                    sCond = String.Format("AND AM.TypeId IN ({0})", arg_sAccTypeIds);
                }

                BsfGlobal.OpenCompanyDB();

                if (SLAnalysisBL.CCId <= 0)
                {
                    sSql = String.Format("SELECT A.SLTypeId,A.SubLedgerId SLId,B.SubLedgerName SLName, CASE WHEN Debit-Credit>=0  THEN Debit-Credit ELSE 0 END Debit," +
                                        "CASE WHEN (Debit-Credit<0 ) THEN ABS(Debit-Credit) ELSE 0 END Credit FROM ( " +
                                        "SELECT A.SubLedgerTypeId SLTypeId ,A.SubLedgerId, SUM(Debit) Debit, SUM(credit) Credit FROM ( " +
                                        "SELECT ET.SubLedgerTypeId,ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END) Debit,  " +
                                        "SUM(CASE WHEN ET.TransType='C' THEN ABS(ET.Amount) ELSE 0 END) Credit FROM EntryTrans ET WITH (READPAST) " +
                                        "INNER JOIN [{2}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                        "WHERE ET.SubLedgerTypeId={0} AND ET.VoucherDate<='{1}' AND ET.CompanyId={3} {4}  GROUP BY SubLedgerTypeId,SubLedgerId " +
                                        "UNION ALL  " +
                                        "SELECT SLT.SLTypeId,SLM.SubLedgerId,CASE WHEN Sum(SL.OpeningBalance)>0 THEN Sum(SL.OpeningBalance) ELSE 0 END  Debit,  " +
                                        "CASE WHEN Sum(SL.OpeningBalance)<0 THEN ABS(Sum(SL.OpeningBalance)) ELSE 0 END  Credit  " +
                                        "FROM SLAccount SL INNER JOIN Account A ON SL.ParentAccountId=A.AccountId  AND A.CompanyId=SL.CompanyId   " +
                                        "INNER JOIN  [{2}].dbo.AccountMaster AM ON A.AccountId=AM.AccountId  " +
                                        "INNER JOIN  [{2}].dbo.SLAccountType SLT ON SLT.TypeId=AM.TypeId " +
                                        "INNER JOIN  [{2}].dbo.SubLedgerMaster SLM ON SLM.SubLedgerTypeId=SLT.SLTypeId " +
                                        "AND SLM.SubLedgerId=SL.SubLedgerId " +
                                        "WHERE  LastLevel='Y' AND SLTypeId={0} AND SL.CompanyId={3} {4} GROUP BY SLT.SLTypeId, SLM.SubLedgerId) A " +
                                        "GROUP BY A.SubLedgerTypeId,A.SubLedgerId) A INNER JOIN  " +
                                        "[{2}].dbo.SubLedgerMaster B ON B.SubLedgerId=A.SubLedgerId ORDER BY B.SubLedgerName  ",
                                        arg_iSLTypeId,
                                        String.Format("{0:dd/MMM/yyyy}", arg_dAsOn), 
                                        BsfGlobal.g_sFaDBName, 
                                        BsfGlobal.g_lCompanyId,
                                        sCond
                                        );
                }
                else
                {
                    sSql = String.Format("SELECT A.SLTypeId,A.SubLedgerId SLId,B.SubLedgerName SLName, CASE WHEN Debit-Credit>=0  THEN Debit-Credit ELSE 0 END Debit," +
                                     "CASE WHEN (Debit-Credit<0 ) THEN ABS(Debit-Credit) ELSE 0 END Credit FROM ( " +
                                     "SELECT A.SubLedgerTypeId SLTypeId ,A.SubLedgerId, SUM(Debit) Debit, SUM(credit) Credit FROM ( " +
                                     "SELECT ET.SubLedgerTypeId,ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END) Debit,  " +
                                     "SUM(CASE WHEN ET.TransType='C' THEN ABS(ET.Amount) ELSE 0 END) Credit FROM EntryTrans ET WITH (READPAST) " +
                                     "INNER JOIN [{1}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                     "WHERE ET.VoucherDate<='{0}' AND ET.CompanyId={2} AND ET.CostCentreId={3} AND ET.SubLedgerTypeId={4} {5} " +
                                     "GROUP BY SubLedgerTypeId,SubLedgerId " +
                                     "UNION ALL  " +
                                     "SELECT SLT.SLTypeId,SLM.SubLedgerId,CASE WHEN Sum(SL.OpeningBalance)>0 THEN Sum(SL.OpeningBalance) ELSE 0 END  Debit,  " +
                                     "CASE WHEN Sum(SL.OpeningBalance)<0 THEN ABS(Sum(SL.OpeningBalance)) ELSE 0 END  Credit  " +
                                     "FROM CCAccount SL INNER JOIN Account A ON SL.ParentAccountId=A.AccountId  AND A.CompanyId=SL.CompanyId   " +
                                     "INNER JOIN  [{1}].dbo.AccountMaster AM ON A.AccountId=AM.AccountId  " +
                                     "INNER JOIN  [{1}].dbo.SLAccountType SLT ON SLT.TypeId=AM.TypeId " +
                                     "INNER JOIN  [{1}].dbo.SubLedgerMaster SLM ON SLM.SubLedgerTypeId=SLT.SLTypeId AND SLM.SubLedgerId=SL.SubLedgerId " +
                                     "WHERE  LastLevel='Y' AND SLTypeId={4} AND SL.CompanyId={2} AND SL.CostCentreId={3} {5} GROUP BY SLT.SLTypeId, SLM.SubLedgerId) A " +
                                     "GROUP BY A.SubLedgerTypeId,A.SubLedgerId) A INNER JOIN  " +
                                     "[{1}].dbo.SubLedgerMaster B ON B.SubLedgerId=A.SubLedgerId ORDER BY B.SubLedgerName  ", 
                                     String.Format("{0:dd/MMM/yyyy}", arg_dAsOn),
                                     BsfGlobal.g_sFaDBName, 
                                     BsfGlobal.g_lCompanyId,
                                     SLAnalysisBL.CCId, arg_iSLTypeId,sCond);
                }

                sda = new SqlDataAdapter(sSql, BsfGlobal.g_CompanyDB);
                sda.Fill(dt);

            }
            catch (Exception ex)
            {
                BsfGlobal.CustomException(ex.Message, ex.StackTrace); ;
            }
            finally
            {
                BsfGlobal.g_CompanyDB.Close();
            }
            return dt;
        }

        internal static DataTable Get_SLType_BillDet(int arg_iSLTypeId, int arg_iSLId, DateTime arg_dAsOn, string arg_sAccTypeIds)
        {
            string sSql = string.Empty;
            string sCond = string.Empty;
            SqlDataAdapter sda = null;
            DataTable dt = new DataTable();
            try
            {
                if (arg_sAccTypeIds != string.Empty) sCond = String.Format("AND AM.TypeId IN ({0})", arg_sAccTypeIds);

                BsfGlobal.OpenCompanyDB();

                sSql = String.Format("SELECT ET.AccountId, ET.SubLedgerTypeId SLTypeId,ET.VoucherDate,ET.VoucherNo,ET.SubLedgerId SLId,ET.RelatedSLId,AM1.AccountName," +
                                     "AM.AccountName RelatedAccount,SubLedger=SLM.SubLedgerName,RelatedSubLedger=SLM1.SubLedgerName, RefType, " +
                                     "EM.ChequeNo,EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre, " +
                                     "CASE WHEN ET.TransType='D' THEN Abs(ET.Amount) ELSE 0 END Debit, " +
                                     "CASE WHEN ET.TransType='C' THEN Abs(ET.Amount) ELSE 0 END Credit FROM EntryTrans ET WITH (READPAST) " +
                                     "LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                     "LEFT JOIN [{1}].dbo.AccountMaster AM WITH (READPAST) ON ET.AccountId=AM.AccountId " +
                                     "LEFT JOIN [{1}].dbo.AccountMaster AM1 WITH (READPAST) ON ET.RelatedAccountId=AM1.AccountId " +
                                     "LEFT JOIN [{1}].dbo.SubLedgerMaster SLM WITH (READPAST) ON ET.SubLedgerId=SLM.SubLedgerId " +
                                     "LEFT JOIN [{1}].dbo.SubLedgerMaster SLM1 WITH (READPAST) ON ET.RelatedSLId=SLM1.SubLedgerId " +
                                     "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " +
                                     "WHERE ET.VoucherDate<='{0}' AND ET.CompanyId={2} AND ET.SubLedgerTypeId={3} AND ET.SubLedgerId={4} {5} ", 
                                     String.Format("{0:dd/MMM/yyyy}", arg_dAsOn), 
                                     BsfGlobal.g_sFaDBName, 
                                     BsfGlobal.g_lCompanyId,
                                     arg_iSLTypeId,
                                     arg_iSLId,
                                     sCond
                                     );

                if (SLAnalysisBL.CCId > 0) sSql = String.Format("{0} AND ET.CostCentreId={1}", sSql, SLAnalysisBL.CCId);

                sSql = sSql + " ORDER BY ET.VoucherDate,ET.RefId,ET.VoucherNo ";

                sda = new SqlDataAdapter(sSql, BsfGlobal.g_CompanyDB);
                sda.Fill(dt);

            }
            catch (Exception ex)
            {
                BsfGlobal.CustomException(ex.Message, ex.StackTrace); ;
            }
            finally
            {
                BsfGlobal.g_CompanyDB.Close();
            }
            return dt;
        }

        internal static decimal Get_VendorType_Balance(int arg_iSLTypeId, int arg_iSLAccId, int arg_iCCId, string arg_sAccTypeIds)
        {
            decimal dAmt;
            string sSql = "";
            string sCond = string.Empty;

            if (arg_sAccTypeIds != string.Empty) sCond = String.Format("AND AM.TypeId IN ({0})", arg_sAccTypeIds);

            if (arg_iSLTypeId == 1)
            {
                if (arg_iCCId > 0)
                {
                    sSql = String.Format("SELECT Balance=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                         "WHERE RefType IN ('PV') AND FromOB=1 AND SubLedgerId={1} AND CostCentreId={2} {3} " +
                                         "UNION ALL " +
                                         "SELECT Balance=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                         "WHERE RefType IN ('PO') AND FromOB=1 AND SubLedgerId={1} AND CostCentreId={2} {3}",
                                         BsfGlobal.g_sFaDBName,
                                         arg_iSLAccId,
                                         arg_iCCId,
                                         sCond
                                         );
                }
                else
                {
                    sSql = String.Format("SELECT Balance=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                         "WHERE RefType IN ('PV') AND FromOB=1 AND SubLedgerId={1} {2} " +
                                         "UNION ALL " +
                                         "SELECT Balance=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                         "WHERE RefType IN ('PO') AND FromOB=1 AND SubLedgerId={1} {2}",
                                         BsfGlobal.g_sFaDBName,
                                         arg_iSLAccId,
                                         sCond
                                         );
                }
            }
            else if (arg_iSLTypeId == 2)
            {
                if (arg_iCCId > 0)
                {
                    sSql = String.Format("SELECT Balance=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                         "WHERE RefType IN ('HB','WB') AND FromOB=1 AND SubLedgerId={1} AND CostCentreId={2} {3} " +
                                         "UNION ALL " +
                                         "SELECT Balance=SUM(OpeningBalance) FROM CCAccount WHERE ParentAccountId IN (" +
                                         "SELECT AccountId FROM [{0}].dbo.AccountMaster AM WHERE TypeId=10 {3} ) AND  SubLedgerId={1} AND CostCentreId={2} " +
                                         "UNION ALL " +
                                         "SELECT Balance=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                         "WHERE RefType IN ('HO','WO') AND FromOB=1 AND SubLedgerId={1} AND CostCentreId={2} {3}",
                                         BsfGlobal.g_sFaDBName,
                                         arg_iSLAccId,
                                         arg_iCCId,
                                         sCond
                                         );
                }
                else
                {
                    sSql = String.Format("SELECT Balance=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +    
                                         "WHERE RefType IN ('HB','WB') AND FromOB=1 AND SubLedgerId={1} {2} " +
                                         "UNION ALL " +
                                         "SELECT Balance=SUM(OpeningBalance) FROM SLAccount WHERE ParentAccountId IN (" +
                                         "SELECT AccountId FROM [{0}].dbo.AccountMaster AM WHERE TypeId=10 {2}) AND  SubLedgerId={1} " +
                                         "UNION ALL " +
                                         "SELECT Balance=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                         "WHERE RefType IN ('HO','WO') AND FromOB=1 AND SubLedgerId={1} {2}",
                                         BsfGlobal.g_sFaDBName,
                                         arg_iSLAccId,
                                         sCond
                                         );
                }
            }
            else if (arg_iSLTypeId == 3)
            {
                if (arg_iCCId > 0)
                {
                    sSql = String.Format("SELECT Balance=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                         "WHERE RefType IN ('SB') AND FromOB=1 AND SubLedgerId={1} AND CostCentreId={2} {3}  " +
                                         "UNION ALL " +
                                         "SELECT Balance=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                         "WHERE RefType IN ('SO') AND FromOB=1 AND SubLedgerId={1} AND CostCentreId={2} {3} ",
                                         BsfGlobal.g_sFaDBName,
                                         arg_iSLAccId,
                                         arg_iCCId,
                                         sCond
                                         );
                }
                else
                {
                    sSql = String.Format("SELECT Balance=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                         "WHERE RefType IN ('SB') AND FromOB=1 AND SubLedgerId={1} {2} " +
                                         "UNION ALL " +
                                         "SELECT Balance=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                         "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                         "WHERE RefType IN ('SO') AND FromOB=1 AND SubLedgerId={1} {2}",
                                         BsfGlobal.g_sFaDBName,
                                         arg_iSLAccId,
                                         sCond
                                         );
                }
            }

            sSql = String.Format("SELECT OpeningBalance=SUM(Balance) FROM ({0}) A", sSql);
            SqlDataAdapter da = new SqlDataAdapter(sSql, BsfGlobal.g_CompanyDB);
            DataTable dt = new DataTable();
            da.Fill(dt);
            da.Dispose();

            if (dt.Rows.Count > 0)
                dAmt = Convert.ToDecimal(clsStatic.IsNullCheck(dt.Rows[0]["OpeningBalance"].ToString(), datatypes.vartypenumeric));
            else
                dAmt = 0;
            dt.Dispose();
            BsfGlobal.g_CompanyDB.Close();
            return dAmt;
        }

        internal static DataSet Get_VendorType_Details(int arg_sVTypeId, DateTime arg_dAsOn, string arg_sAccTypeIds)
        {
            string sSql = "";
            string sCond = string.Empty;
            SqlDataAdapter sda = null;
            DataSet ds = new DataSet();
            try
            {
                if (arg_sAccTypeIds != string.Empty) sCond = String.Format("AND AM.TypeId IN ({0})", arg_sAccTypeIds);

                BsfGlobal.OpenCompanyDB();

                if (SLAnalysisBL.CCId <= 0)
                {
                    if (arg_sVTypeId == 1)
                    {
                        sSql = String.Format("SELECT SubLedgerId,Amount=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE BR.RefType IN ('PV') AND FromOB=1 {4} GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('PO') AND FromOB=1 {4} GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('PV') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4} GROUP BY SubLedgerId  " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN AD.Amount ELSE -AD.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE BR.RefType IN ('PV') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4} GROUP BY ET.SubLedgerId ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             sCond
                                             );
                    }
                    else if (arg_sVTypeId == 2)
                    {
                        sSql = String.Format("SELECT SubLedgerId,Amount=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('HB','WB') AND FromOB=1 {4} GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('HO','WO') AND FromOB=1 {4} GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,SUM(OpeningBalance) FROM SLAccount WHERE ParentAccountId IN (" +
                                             "SELECT AccountId FROM [{0}].dbo.AccountMaster AM WHERE TypeId=10 {4} ) GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('HB','WB') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4} GROUP BY SubLedgerId  " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN AD.Amount ELSE -AD.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND BR.RefType IN ('HB','WB') AND ET.RefType='O' " +
                                             "AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4} GROUP BY ET.SubLedgerId ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             sCond
                                             );
                    }
                    else if (arg_sVTypeId == 3)
                    {
                        sSql = String.Format("SELECT SubLedgerId,Amount=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('SB') AND FromOB=1 {4}  GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('SO') AND FromOB=1 {4}  GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('SB') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4}  GROUP BY SubLedgerId  " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN AD.Amount ELSE -AD.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE BR.RefType IN ('SB') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4}  GROUP BY ET.SubLedgerId ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             sCond
                                             );
                    }
                    else if (arg_sVTypeId == 4)
                    {
                        sSql = String.Format("SELECT ET.SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.RefType='S' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4}  GROUP BY ET.SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN AD.Amount ELSE -AD.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4}  " +
                                             "AND ET.RefType='O' AND BR.RefType='S' GROUP BY ET.SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN EntryMaster EM ON EM.EntryId=ET.RefId AND EM.JournalType=ET.RefType " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.RefType <>'O' AND ET.SubLedgerTypeId=1 AND ET.VoucherDate<='{2:dd-MMM-yyyy}' {4}  GROUP BY ET.SubLedgerId",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             sCond
                                             );
                    }
                }
                else
                {
                    if (arg_sVTypeId == 1)
                    {
                        sSql = String.Format("SELECT SubLedgerId,Amount=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR "+
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('PV') AND CostCentreId={4} AND FromOB=1 {5}  GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('PO') AND CostCentreId={4} AND FromOB=1 {5}  GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('PV') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND ET.CostCentreId={4}  {5} GROUP BY SubLedgerId  " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN AD.Amount ELSE -AD.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE BR.RefType IN ('PV') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND BR.CostCentreId={4}  {5} GROUP BY ET.SubLedgerId ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             SLAnalysisBL.CCId,
                                             sCond
                                             );
                    }
                    else if (arg_sVTypeId == 2)
                    {
                        sSql = String.Format("SELECT SubLedgerId,Amount=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('HB','WB') AND CostCentreId={4} AND FromOB=1 {5} GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('HO','WO') AND CostCentreId={4} AND FromOB=1 {5}  GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,SUM(OpeningBalance) FROM CCAccount WHERE ParentAccountId IN (" +
                                             "SELECT AccountId FROM [{0}].dbo.AccountMaster AM WHERE TypeId=10 {5} ) AND CostCentreId={4} GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('HB','WB') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND ET.CostCentreId={4} {5}  GROUP BY SubLedgerId  " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN AD.Amount ELSE -AD.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND BR.RefType IN ('HB','WB') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND BR.CostCentreId={4} {5}  GROUP BY ET.SubLedgerId ",
                                              BsfGlobal.g_sFaDBName,
                                              BsfGlobal.g_dStartDate,
                                              arg_dAsOn,
                                              BsfGlobal.g_lYearId,
                                              SLAnalysisBL.CCId,
                                              sCond
                                              );
                    }
                    else if (arg_sVTypeId == 3)
                    {
                        sSql = String.Format("SELECT SubLedgerId,Amount=-1*SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('SB') AND CostCentreId={4} AND FromOB=1 {5}  GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(BillAmount) FROM [{0}].dbo.BillRegister BR " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE RefType IN ('SO') AND CostCentreId={4} AND FromOB=1 {5}  GROUP BY SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('SB') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND ET.CostCentreId={4} {5}   GROUP BY SubLedgerId  " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN AD.Amount ELSE -AD.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE BR.RefType IN ('SB') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND BR.CostCentreId={4} {5}  GROUP BY ET.SubLedgerId ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             SLAnalysisBL.CCId,
                                             sCond
                                              );
                    }
                    else if (arg_sVTypeId == 4)
                    {
                        sSql = String.Format("SELECT ET.SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.RefType='S' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND ET.CostCentreId={4} {5}  GROUP BY ET.SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,Amount=SUM(CASE WHEN ET.TransType='D' THEN AD.Amount ELSE -AD.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE ET.RefType='O' AND BR.RefType='S' AND ET.SubLedgerTypeId=1 AND ET.VoucherDate<='{1:dd-MMM-yyyy}' AND ET.CostCentreId={4} {5}  GROUP BY ET.SubLedgerId " +
                                             "UNION ALL " +
                                             "SELECT ET.SubLedgerId,SUM(CASE WHEN ET.TransType='D' THEN ET.Amount ELSE -ET.Amount END) FROM EntryTrans ET " +
                                             "INNER JOIN EntryMaster EM ON EM.EntryId=ET.RefId AND EM.JournalType=ET.RefType " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE ET.RefType <>'O' AND ET.SubLedgerTypeId=1  AND ET.CostCentreId={4} {5}  GROUP BY ET.SubLedgerId",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             SLAnalysisBL.CCId,
                                             sCond
                                             );
                    }
                }

                sSql = String.Format("SELECT SLTypeId,SLId,SLName, Debit=CASE WHEN Amount>0 THEN Amount ELSE 0 END,Credit=CASE WHEN Amount<0 THEN ABS(Amount) ELSE 0 END FROM (" +
                                     "SELECT SLTypeId=1, SLId=A.SubLedgerId,SLName=B.SubLedgerName,Amount=SUM(Amount) FROM ({0}) A INNER JOIN [{1}].dbo.SubLedgerMaster B ON A.SubledgerId=B.SubLedgerId " +
                                     "WHERE B.SubLedgerTypeId=1 GROUP BY A.SubLedgerId,B.SubLedgerName) A  ORDER BY A.SLName", sSql, BsfGlobal.g_sFaDBName);
                sda = new SqlDataAdapter(sSql, BsfGlobal.g_CompanyDB);
                sda.Fill(ds, "VTypeInfo");

                if (SLAnalysisBL.CCId <= 0)
                {
                    if (arg_sVTypeId == 1)
                    {
                        sSql = String.Format("SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                             "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                             "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                             "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                             "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " + 
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('PV') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' " +
                                             "UNION ALL " +
                                             "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType," +
                                             "Debit=CASE WHEN ET.TransType='D' THEN AD.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN AD.Amount ELSE 0 END, " +
                                             "EM.ChequeNo,EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  FROM EntryTrans ET " +
                                             "LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                             "LEFT JOIN [" + BsfGlobal.g_sWorkFlowDBName + "].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                             "WHERE BR.RefType IN ('PV') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId
                                             );
                    }
                    else if (arg_sVTypeId == 2)
                    {
                        sSql = String.Format("SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                             "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                             "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                             "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                             "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +  
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('HB','WB') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' " +
                                             "UNION ALL " +
                                             "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType," +
                                             "Debit=CASE WHEN ET.TransType='D' THEN AD.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN AD.Amount ELSE 0 END, " +
                                             "EM.ChequeNo,EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  FROM EntryTrans ET " +
                                             "LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                             "LEFT JOIN [" + BsfGlobal.g_sWorkFlowDBName + "].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " +
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " + 
                                             "WHERE BR.RefType IN ('HB','WB') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId
                                             );
                    }
                    else if (arg_sVTypeId == 3)
                    {
                        sSql = String.Format("SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                            "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN [" + BsfGlobal.g_sWorkFlowDBName + "].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " +
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +  
                                            "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('SB') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' " +
                                            "UNION ALL " +
                                            "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType," +
                                            "Debit=CASE WHEN ET.TransType='D' THEN AD.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN AD.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " +
                                            "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                            "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                            "WHERE BR.RefType IN ('SB') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' ",
                                            BsfGlobal.g_sFaDBName,
                                            BsfGlobal.g_dStartDate,
                                            arg_dAsOn,
                                            BsfGlobal.g_lYearId
                                            );
                    }
                    else if (arg_sVTypeId == 4)
                    {
                        sSql = String.Format("SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                            "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " + 
                                            "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('S') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' " +
                                            "UNION ALL " +
                                            "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                            "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " + 
                                            "WHERE ET.RefType <>'O' AND ET.SubLedgerTypeId=1 AND ET.VoucherDate<='{2:dd-MMM-yyyy}' " +
                                            "UNION ALL " +
                                            "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType," +
                                            "Debit=CASE WHEN ET.TransType='D' THEN AD.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN AD.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                            "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                            "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " + 
                                            "WHERE BR.RefType IN ('S') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' ",
                                            BsfGlobal.g_sFaDBName,
                                            BsfGlobal.g_dStartDate,
                                            arg_dAsOn,
                                            BsfGlobal.g_lYearId
                                            );
                    }
                }
                else
                {
                    if (arg_sVTypeId == 1)
                    {
                        sSql = String.Format("SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                             "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                             "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                             "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                             "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " + 
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('PV') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND ET.CostCentreId={4} " +
                                             "UNION ALL " +
                                             "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType," +
                                             "Debit=CASE WHEN ET.TransType='D' THEN AD.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN AD.Amount ELSE 0 END, EM.ChequeNo, " +
                                             "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                             "LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                             "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " +
                                             "FROM EntryTrans ET INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE BR.RefType IN ('PV') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND BR.CostCentreId={4} ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             SLAnalysisBL.CCId
                                             );
                    }
                    else if (arg_sVTypeId == 2)
                    {
                        sSql = String.Format("SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                             "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                             "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                             "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                             "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " + 
                                             "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('HB','WB') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND ET.CostCentreId={4} " +
                                             "UNION ALL " +
                                             "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType," +
                                             "Debit=CASE WHEN ET.TransType='D' THEN AD.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN AD.Amount ELSE 0 END, EM.ChequeNo, " +
                                             "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                             "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                             "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                             "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                             "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                             "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                             "WHERE BR.RefType IN ('HB','WB') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND BR.CostCentreId={4} ",
                                             BsfGlobal.g_sFaDBName,
                                             BsfGlobal.g_dStartDate,
                                             arg_dAsOn,
                                             BsfGlobal.g_lYearId,
                                             SLAnalysisBL.CCId
                                             );
                    }
                    else if (arg_sVTypeId == 3)
                    {
                        sSql = String.Format("SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                            "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " + 
                                            "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('SB') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND ET.CostCentreId={4} " +
                                            "UNION ALL " +
                                            "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType," +
                                            "Debit=CASE WHEN ET.TransType='D' THEN AD.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN AD.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                            "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                            "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                            "WHERE BR.RefType IN ('SB') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND BR.CostCentreId={4} ",
                                            BsfGlobal.g_sFaDBName,
                                            BsfGlobal.g_dStartDate,
                                            arg_dAsOn,
                                            BsfGlobal.g_lYearId,
                                            SLAnalysisBL.CCId
                                            );
                    }
                    else if (arg_sVTypeId == 4)
                    {
                        sSql = String.Format("SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                            "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " + 
                                            "WHERE ET.SubLedgerTypeId=1 AND ET.RefType IN ('S') AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND ET.CostCentreId={4} " +
                                            "UNION ALL " +
                                            "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType, " +
                                            "Debit=CASE WHEN ET.TransType='D' THEN ET.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN ET.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=ET.AccountId " +
                                            "WHERE ET.RefType <>'O' AND ET.SubLedgerTypeId=1  AND ET.CostCentreId={4} AND ET.VoucherDate<='{2:dd-MMM-yyyy}' " +
                                            "UNION ALL " +
                                            "SELECT ET.VoucherDate,ET.VoucherNo,ET.SubLedgerTypeId,ET.SubLedgerId,ET.RelatedSLId,ET.AccountId,ET.RelatedAccountId,ET.RefType," +
                                            "Debit=CASE WHEN ET.TransType='D' THEN AD.Amount ELSE 0 END, Credit=CASE WHEN ET.TransType='C' THEN AD.Amount ELSE 0 END, EM.ChequeNo, " +
                                            "EM.ChequeDate,EM.Narration,CC.CostCentreName CostCentre  " +
                                            "FROM EntryTrans ET LEFT JOIN EntryMaster EM WITH (READPAST) ON ET.RefId=EM.EntryId AND RefType=JournalType " +
                                            "LEFT JOIN ["+BsfGlobal.g_sWorkFlowDBName+"].dbo.CostCentre CC ON CC.CostCentreId=ET.CostCentreId " + 
                                            "INNER JOIN [{0}].dbo.Adjustment AD ON ET.RefId=AD.EntryId AND AD.FYearId={3} " +
                                            "INNER JOIN [{0}].dbo.BillRegister BR ON AD.BillRegisterId=BR.BillRegisterId " +
                                            "INNER JOIN [{0}].dbo.AccountMaster AM ON AM.AccountId=BR.AccountId " +
                                            "WHERE BR.RefType IN ('S') AND ET.RefType='O' AND ET.VoucherDate<='{2:dd-MMM-yyyy}' AND BR.CostCentreId={4} ",
                                            BsfGlobal.g_sFaDBName,
                                            BsfGlobal.g_dStartDate,
                                            arg_dAsOn,
                                            BsfGlobal.g_lYearId,
                                             SLAnalysisBL.CCId
                                            );
                    }
                }

                sSql = String.Format("SELECT SLTypeId=1, A.VoucherDate,A.VoucherNo,SLId=A.SubLedgerId,A.RelatedSLId,A.AccountId,AM.AccountName," +
                                     "RelatedAccount=AM1.AccountName,SubLedger=SLM.SubLedgerName,RelatedSubLedger=SLM1.SubLedgerName, A.RefType, " +
                                     "A.Debit,A.Credit,A.ChequeNo,A.ChequeDate,A.Narration,A.CostCentre " +
                                     "FROM ({0}) A INNER JOIN [{1}].dbo.AccountMaster AM ON AM.AccountId=A.RelatedAccountId " +
                                     "INNER JOIN [{1}].dbo.AccountMaster AM1 ON AM1.AccountId=A.AccountId " +
                                     "INNER JOIN [{1}].dbo.SubLedgerMaster SLM ON SLM.SubLedgerId=A.SubLedgerId " +
                                     "LEFT JOIN [{1}].dbo.SubLedgerMaster SLM1 ON SLM1.SubLedgerId=A.RelatedSLId ", 
                                     sSql, BsfGlobal.g_sFaDBName
                                     );
                sda = new SqlDataAdapter(sSql, BsfGlobal.g_CompanyDB);
                sda.Fill(ds, "VTypeDet");

                BsfGlobal.g_CompanyDB.Close();
            }
            catch (Exception ex)
            {
                BsfGlobal.CustomException(ex.Message, ex.StackTrace); ;
            }
            return ds;
        }
    }
}
