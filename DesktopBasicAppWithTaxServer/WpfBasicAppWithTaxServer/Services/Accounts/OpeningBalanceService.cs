using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfServerApp;
using WpfServerApp.General;
using WpfServerApp.Services;

namespace WpfServerApp.Services.Accounts
{
    public class OpeningBalanceService : IOpeningBalance
    {
        

        public bool CreateBill(COpeningBalance oOpeningBalance)
        {
            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                
                using (var dataB = new Database9002Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {

                        LedgerService ls = new LedgerService();
                        BillNoService bs = new BillNoService();


                        int cbillNo = bs.ReadNextOpeningBalanceBillNo(oOpeningBalance.FinancialCode);
                        bs.UpdateOpeningBalanceBillNo(oOpeningBalance.FinancialCode,cbillNo+1);

                        //Saving to Ledger Transaction
                        int ltBillNo = bs.ReadNextLedgerTransactionBillNo(oOpeningBalance.FinancialCode);
                        bs.UpdateLedgerTransactionBillNo(oOpeningBalance.FinancialCode, ltBillNo + 1);

                        for (int i = 0; i < oOpeningBalance.Details.Count; i++)
                        {
                            opening_balances op = new opening_balances();

                            op.bill_no = cbillNo.ToString();
                            op.bill_date_time = oOpeningBalance.BillDateTime;
                            op.serial_no = oOpeningBalance.Details.ElementAt(i).SerialNo;
                            op.ledger_code= oOpeningBalance.Details.ElementAt(i).LedgerCode;
                            op.ledger = oOpeningBalance.Details.ElementAt(i).Ledger;
                            op.narration = oOpeningBalance.Details.ElementAt(i).Narration;
                            op.debit = oOpeningBalance.Details.ElementAt(i).Debit;
                            op.credit = oOpeningBalance.Details.ElementAt(i).Credit;
                            op.financial_code = oOpeningBalance.FinancialCode;                            

                            dataB.opening_balances.Add(op);
                            

                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo.ToString();
                            lt1.bill_date_time = oOpeningBalance.BillDateTime;
                            lt1.bill_type = "OP";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oOpeningBalance.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oOpeningBalance.Details.ElementAt(i).Ledger;
                            lt1.narration = oOpeningBalance.Details.ElementAt(i).Narration;
                            lt1.debit = oOpeningBalance.Details.ElementAt(i).Debit;
                            lt1.credit = oOpeningBalance.Details.ElementAt(i).Credit;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oOpeningBalance.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oOpeningBalance.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oOpeningBalance.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = oOpeningBalance.Details.ElementAt(i).Debit > 0 ? "Receivable" : "Payable";
                            lt1.ref_bill_no = cbillNo.ToString();
                            lt1.ref_bill_date_time = oOpeningBalance.BillDateTime;
                            lt1.financial_code = oOpeningBalance.FinancialCode;

                            //Second entry
                            ledger_transactions lt2 = new ledger_transactions();                            

                            lt2.bill_no = ltBillNo.ToString();
                            lt2.bill_date_time = oOpeningBalance.BillDateTime;
                            lt2.bill_type = "OP";
                            lt2.serial_no = i+2;
                            lt2.ledger_code = oOpeningBalance.Details.ElementAt(i).Debit>0? UniqueLedgers.LedgerCode["Receivable"] : UniqueLedgers.LedgerCode["Payable"];
                            lt2.ledger = oOpeningBalance.Details.ElementAt(i).Debit > 0 ? "Receivable": "Payable";
                            lt2.narration = oOpeningBalance.Details.ElementAt(i).Narration;
                            lt2.debit = oOpeningBalance.Details.ElementAt(i).Credit;
                            lt2.credit = oOpeningBalance.Details.ElementAt(i).Debit;
                            lt2.a_group_code = ls.ReadAGroupCodeOf(oOpeningBalance.Details.ElementAt(i).Debit > 0 ? UniqueLedgers.LedgerCode["Receivable"] : UniqueLedgers.LedgerCode["Payable"]);
                            lt2.b_group_code = ls.ReadBGroupCodeOf(oOpeningBalance.Details.ElementAt(i).Debit > 0 ? UniqueLedgers.LedgerCode["Receivable"] : UniqueLedgers.LedgerCode["Payable"]);
                            lt2.c_group_code = ls.ReadCGroupCodeOf(oOpeningBalance.Details.ElementAt(i).Debit > 0 ? UniqueLedgers.LedgerCode["Receivable"] : UniqueLedgers.LedgerCode["Payable"]);
                            lt2.co_ledger = oOpeningBalance.Details.ElementAt(i).Ledger;
                            lt2.ref_bill_no = cbillNo.ToString();
                            lt2.ref_bill_date_time = oOpeningBalance.BillDateTime;
                            lt2.financial_code = oOpeningBalance.FinancialCode;

                            dataB.ledger_transactions.Add(lt1);
                            dataB.ledger_transactions.Add(lt2);
                        }

                        dataB.SaveChanges();
                        //Success
                        returnValue = true;

                        dataBTransaction.Commit();
                    }
                    catch
                    {
                        dataBTransaction.Rollback();
                    }
                    finally
                    {

                    }
                }                
            }

            return returnValue;
        }

        public bool DeleteBill(string billNo,string financialCode)
        {
            bool returnValue = true;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9002Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        //Delete from Cash Payment
                        var op = dataB.opening_balances.Select(c => c).Where(x=>x.bill_no==billNo&&x.financial_code==financialCode);
                        dataB.opening_balances.RemoveRange(op);
                                                                        

                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == billNo && x.financial_code == financialCode&&x.bill_type=="OP");
                        dataB.ledger_transactions.RemoveRange(lt);

                        dataB.SaveChanges();                        
                        dataBTransaction.Commit();
                    }
                    catch
                    {
                        returnValue = false;
                        dataBTransaction.Rollback();
                    }
                    finally
                    {

                    }
                }
            }
            return returnValue;
        }

        public COpeningBalance ReadBill(string billNo,string financialCode)
        {
            COpeningBalance cop = null;

            using (var dataB = new Database9002Entities())
            {
                var ops = dataB.opening_balances.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode).OrderBy(y=>y.serial_no);
                
                if (ops.Count() > 0)
                {
                    cop = new COpeningBalance();

                    var op = ops.FirstOrDefault();
                    cop.Id = op.id;
                    cop.BillNo = op.bill_no;
                    cop.BillDateTime = op.bill_date_time;
                    cop.FinancialCode = op.financial_code;

                    foreach (var item in ops)
                    {
                        cop.Details.Add(new COpeningBalanceDetails() { SerialNo=item.serial_no,LedgerCode=item.ledger_code,Ledger=item.ledger, Narration=item.narration, Debit=item.debit, Credit=item.credit });
                    }
                }
                
            }

            return cop;
        }

        public int ReadNextBillNo(string financialCode)
        {
            
            BillNoService bns = new BillNoService();
            return bns.ReadNextOpeningBalanceBillNo(financialCode);
            
        }

        public bool UpdateBill(COpeningBalance oOpeningBalance)
        {
            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9002Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        //Remove already entered data from Cash Payment
                        var opp = dataB.opening_balances.Select(c => c).Where(x => x.bill_no == oOpeningBalance.BillNo&& x.financial_code==oOpeningBalance.FinancialCode);
                        dataB.opening_balances.RemoveRange(opp);
                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == oOpeningBalance.BillNo && x.financial_code == oOpeningBalance.FinancialCode && x.bill_type == "OP");
                        string ltBillNo = "";
                        foreach (var item in lt)
                        {
                            ltBillNo = item.bill_no;
                            break;
                        }
                        dataB.ledger_transactions.RemoveRange(lt);

                        //Add newly updated data
                        LedgerService ls = new LedgerService();
                        
                        for (int i = 0; i < oOpeningBalance.Details.Count; i++)
                        {
                            opening_balances op = new opening_balances();

                            op.bill_no = oOpeningBalance.BillNo;
                            op.bill_date_time = oOpeningBalance.BillDateTime;
                            op.serial_no = oOpeningBalance.Details.ElementAt(i).SerialNo;
                            op.ledger_code = oOpeningBalance.Details.ElementAt(i).LedgerCode;
                            op.ledger = oOpeningBalance.Details.ElementAt(i).Ledger;
                            op.narration = oOpeningBalance.Details.ElementAt(i).Narration;
                            op.debit = oOpeningBalance.Details.ElementAt(i).Debit;
                            op.credit = oOpeningBalance.Details.ElementAt(i).Credit;
                            op.financial_code = oOpeningBalance.FinancialCode;

                            dataB.opening_balances.Add(op);


                            //Saving to Ledger Transaction
                        

                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo;
                            lt1.bill_date_time = oOpeningBalance.BillDateTime;
                            lt1.bill_type = "OP";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oOpeningBalance.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oOpeningBalance.Details.ElementAt(i).Ledger;
                            lt1.narration = oOpeningBalance.Details.ElementAt(i).Narration;
                            lt1.debit = oOpeningBalance.Details.ElementAt(i).Debit;
                            lt1.credit = oOpeningBalance.Details.ElementAt(i).Credit;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oOpeningBalance.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oOpeningBalance.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oOpeningBalance.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = oOpeningBalance.Details.ElementAt(i).Debit > 0 ? "Receivable" : "Payable";
                            lt1.ref_bill_no = oOpeningBalance.BillNo;
                            lt1.ref_bill_date_time = oOpeningBalance.BillDateTime;
                            lt1.financial_code = oOpeningBalance.FinancialCode;

                            //Second entry
                            ledger_transactions lt2 = new ledger_transactions();

                            lt2.bill_no = ltBillNo;
                            lt2.bill_date_time = oOpeningBalance.BillDateTime;
                            lt2.bill_type = "OP";
                            lt2.serial_no = i+2;
                            lt2.ledger_code = oOpeningBalance.Details.ElementAt(i).Debit > 0 ? UniqueLedgers.LedgerCode["Receivable"] : UniqueLedgers.LedgerCode["Payable"];
                            lt2.ledger = oOpeningBalance.Details.ElementAt(i).Debit > 0 ? "Receivable" : "Payable";
                            lt2.narration = oOpeningBalance.Details.ElementAt(i).Narration;
                            lt2.debit = oOpeningBalance.Details.ElementAt(i).Credit;
                            lt2.credit = oOpeningBalance.Details.ElementAt(i).Debit;
                            lt2.a_group_code = ls.ReadAGroupCodeOf(oOpeningBalance.Details.ElementAt(i).Debit > 0 ? UniqueLedgers.LedgerCode["Receivable"] : UniqueLedgers.LedgerCode["Payable"]);
                            lt2.b_group_code = ls.ReadBGroupCodeOf(oOpeningBalance.Details.ElementAt(i).Debit > 0 ? UniqueLedgers.LedgerCode["Receivable"] : UniqueLedgers.LedgerCode["Payable"]);
                            lt2.c_group_code = ls.ReadCGroupCodeOf(oOpeningBalance.Details.ElementAt(i).Debit > 0 ? UniqueLedgers.LedgerCode["Receivable"] : UniqueLedgers.LedgerCode["Payable"]);
                            lt2.co_ledger = oOpeningBalance.Details.ElementAt(i).Ledger;
                            lt2.ref_bill_no = oOpeningBalance.BillNo;
                            lt2.ref_bill_date_time = oOpeningBalance.BillDateTime;
                            lt2.financial_code = oOpeningBalance.FinancialCode;

                            dataB.ledger_transactions.Add(lt1);
                            dataB.ledger_transactions.Add(lt2);
                        }

                        dataB.SaveChanges();
                        //Success
                        returnValue = true;

                        dataBTransaction.Commit();
                    }
                    catch(Exception e)
                    {                        
                        dataBTransaction.Rollback();
                    }
                    finally
                    {

                    }
                }
            }
            return returnValue;
        }

        //Reports
        public List<COpeningBalanceReportDetailed> FindOpeningBalancesDetailed(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode)
        {
            List<COpeningBalanceReportDetailed> report = new List<COpeningBalanceReportDetailed>();

            using (var dataB = new Database9002Entities())
            {
                string startD = startDate.Year + "-" + startDate.Month + "-" + startDate.Day;
                string endD = endDate.Year + "-" + endDate.Month + "-" + endDate.Day;

                string billNoQuery = billNo.Trim().Equals("") ? "" : " && (bd.bill_no='" + billNo.Trim() + "') ";
                string ledgerCodeQuery = ledgerCode.Trim().Equals("") ? "" : " && (bd.ledger_code='" + ledgerCode.Trim() + "') ";
                string ledgerQuery = ledger.Trim().Equals("") ? "" : " && (bd.ledger Like '%" + ledger.Trim() + "%') ";
                string narrationQuery = narration.Trim().Equals("") ? "" : " && (bd.narration Like '%" + narration.Trim() + "%') ";
                string financialCodeQuery = financialCode.Trim().Equals("") ? "" : " && (bd.financial_code='" + financialCode.Trim() + "') ";

                string subQ = billNoQuery + ledgerCodeQuery + ledgerQuery + narrationQuery + financialCodeQuery;

                var resData = dataB.Database.SqlQuery<COpeningBalanceReportDetailed>("Select bd.bill_date_time As BillDateTime, bd.bill_no As BillNo, bd.serial_no As SerialNo, bd.ledger_code As LedgerCode, bd.ledger As Ledger, bd.narration as Narration, bd.financial_code As FinancialCode, bd.debit As Debit, bd.credit As Debit From opening_balances bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + " Order By bd.bill_date_time,bd.bill_no,bd.serial_no ");

                decimal? credits = 0;
                decimal? debits = 0;
                foreach (var item in resData)
                {
                    debits = debits + item.Debit;
                    credits = credits + item.Credit;
                    report.Add(item);
                }
                //Total
                report.Add(new COpeningBalanceReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "" });
                report.Add(new COpeningBalanceReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "Total", Debit = debits, Credit = credits });

            }


            return report;
        }

        public List<COpeningBalanceReportSummary> FindOpeningBalancesSummary(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode)
        {
            List<COpeningBalanceReportSummary> report = new List<COpeningBalanceReportSummary>();

            using (var dataB = new Database9002Entities())
            {
                string startD = startDate.Year + "-" + startDate.Month + "-" + startDate.Day;
                string endD = endDate.Year + "-" + endDate.Month + "-" + endDate.Day;

                string billNoQuery = billNo.Trim().Equals("") ? "" : " && (bd.bill_no='" + billNo.Trim() + "') ";
                string ledgerCodeQuery = ledgerCode.Trim().Equals("") ? "" : " && (bd.ledger_code='" + ledgerCode.Trim() + "') ";
                string ledgerQuery = ledger.Trim().Equals("") ? "" : " && (bd.ledger Like '%" + ledger.Trim() + "%') ";
                string narrationQuery = narration.Trim().Equals("") ? "" : " && (bd.narration Like '%" + narration.Trim() + "%') ";
                string financialCodeQuery = financialCode.Trim().Equals("") ? "" : " && (bd.financial_code='" + financialCode.Trim() + "') ";

                string subQ = billNoQuery + ledgerCodeQuery + ledgerQuery + narrationQuery + financialCodeQuery;

                var resData = dataB.Database.SqlQuery<COpeningBalanceReportSummary>("Select Sum(bd.debit) As TotalDebit,Sum(bd.credit) As TotalCredit,bd.bill_date_time As BillDateTime,bd.bill_no As BillNo, bd.financial_code As FinancialCode From opening_balances bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + "Group By bd.bill_date_time,bd.bill_no,bd.financial_code Order By bd.bill_date_time,bd.bill_no");

                decimal? debit = 0;
                decimal? credit = 0;

                foreach (var item in resData)
                {
                    credit = credit + item.TotalCredit;
                    debit = debit + item.TotalDebit;
                    report.Add(item);
                }
                //Total
                report.Add(new COpeningBalanceReportSummary() { BillDateTime = null, BillNo = "" });
                report.Add(new COpeningBalanceReportSummary() { BillDateTime = null, BillNo = "Total", TotalDebit = debit, TotalCredit = credit });

            }


            return report;
        }
    }
}
