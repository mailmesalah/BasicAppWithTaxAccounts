using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfServerApp;
using WpfServerApp.General;
using WpfServerApp.Services;

namespace WpfServerApp.Services.Accounts
{

    public class BankWithdrawalService : IBankWithdrawal
    {
        

        public bool CreateBill(CBankWithdrawal oBankWithdrawal)
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


                        int cbillNo = bs.ReadNextBankWithdrawalBillNo(oBankWithdrawal.FinancialCode);
                        bs.UpdateBankWithdrawalBillNo(oBankWithdrawal.FinancialCode,cbillNo+1);
                        //Saving to Ledger Transaction
                        int ltBillNo = bs.ReadNextLedgerTransactionBillNo(oBankWithdrawal.FinancialCode);
                        bs.UpdateLedgerTransactionBillNo(oBankWithdrawal.FinancialCode, ltBillNo + 1);

                        for (int i = 0; i < oBankWithdrawal.Details.Count; i++)
                        {
                            bank_withdrawals bw = new bank_withdrawals();

                            bw.bill_no = cbillNo.ToString();
                            bw.bill_date_time = oBankWithdrawal.BillDateTime;
                            bw.serial_no = oBankWithdrawal.Details.ElementAt(i).SerialNo;
                            bw.ledger_code= oBankWithdrawal.Details.ElementAt(i).LedgerCode;
                            bw.ledger = oBankWithdrawal.Details.ElementAt(i).Ledger;
                            bw.narration = oBankWithdrawal.Details.ElementAt(i).Narration;
                            bw.amount = oBankWithdrawal.Details.ElementAt(i).Amount;
                            bw.financial_code = oBankWithdrawal.FinancialCode;
                            bw.status = oBankWithdrawal.Details.ElementAt(i).Status;
                            bw.bank = oBankWithdrawal.Bank;
                            bw.bank_code = oBankWithdrawal.BankCode;

                            dataB.bank_withdrawals.Add(bw);
                            
                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo.ToString();
                            lt1.bill_date_time = oBankWithdrawal.BillDateTime;
                            lt1.bill_type = "BW";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oBankWithdrawal.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oBankWithdrawal.Details.ElementAt(i).Ledger;
                            lt1.narration = oBankWithdrawal.Details.ElementAt(i).Narration;
                            lt1.debit = oBankWithdrawal.Details.ElementAt(i).Amount;
                            lt1.credit = 0;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oBankWithdrawal.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oBankWithdrawal.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oBankWithdrawal.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = oBankWithdrawal.Bank;
                            lt1.ref_bill_no = cbillNo.ToString();
                            lt1.ref_bill_date_time = oBankWithdrawal.BillDateTime;
                            lt1.financial_code = oBankWithdrawal.FinancialCode;

                            //Second entry
                            ledger_transactions lt2 = new ledger_transactions();

                            lt2.bill_no = ltBillNo.ToString();
                            lt2.bill_date_time = oBankWithdrawal.BillDateTime;
                            lt2.bill_type = "BW";
                            lt2.serial_no =i+2;
                            lt2.ledger_code = oBankWithdrawal.BankCode;
                            lt2.ledger = oBankWithdrawal.Bank;
                            lt2.narration = oBankWithdrawal.Details.ElementAt(i).Narration;
                            lt2.debit = 0;
                            lt2.credit = oBankWithdrawal.Details.ElementAt(i).Amount;
                            lt2.a_group_code = ls.ReadAGroupCodeOf(oBankWithdrawal.BankCode);
                            lt2.b_group_code = ls.ReadBGroupCodeOf(oBankWithdrawal.BankCode);
                            lt2.c_group_code = ls.ReadCGroupCodeOf(oBankWithdrawal.BankCode);
                            lt2.co_ledger = oBankWithdrawal.Details.ElementAt(i).Ledger;
                            lt2.ref_bill_no = cbillNo.ToString();
                            lt2.ref_bill_date_time = oBankWithdrawal.BillDateTime;
                            lt2.financial_code = oBankWithdrawal.FinancialCode;

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
                        //Delete from Cash Receipt
                        var bd = dataB.bank_withdrawals.Select(c => c).Where(x=>x.bill_no==billNo&&x.financial_code==financialCode);
                        dataB.bank_withdrawals.RemoveRange(bd);
                                                                        

                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no== billNo && x.financial_code == financialCode&&x.bill_type=="BW");
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

        public CBankWithdrawal ReadBill(string billNo,string financialCode)
        {
            CBankWithdrawal bwd = null;

            using (var dataB = new Database9002Entities())
            {
                var bws = dataB.bank_withdrawals.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode).OrderBy(y=>y.serial_no);
                
                if (bws.Count() > 0)
                {
                    bwd = new CBankWithdrawal();

                    var bw = bws.FirstOrDefault();
                    bwd.Id = bw.id;
                    bwd.BillNo = bw.bill_no;
                    bwd.BillDateTime = bw.bill_date_time;
                    bwd.FinancialCode = bw.financial_code;
                    bwd.Bank = bw.bank;
                    bwd.BankCode = bw.bank_code;
                    foreach (var item in bws)
                    {
                        bwd.Details.Add(new CBankWithdrawalDetails() { SerialNo=item.serial_no,LedgerCode=item.ledger_code,Ledger=item.ledger, Narration=item.narration, Amount=item.amount, Status=item.status });
                    }
                }
                
            }

            return bwd;
        }

        public int ReadNextBillNo(string financialCode)
        {
            
            BillNoService bns = new BillNoService();
            return bns.ReadNextBankWithdrawalBillNo(financialCode);
            
        }

        public bool UpdateBill(CBankWithdrawal oBankWithdrawal)
        {
            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9002Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {                        
                        var bws = dataB.bank_withdrawals.Select(c => c).Where(x => x.bill_no == oBankWithdrawal.BillNo&& x.financial_code==oBankWithdrawal.FinancialCode);
                        dataB.bank_withdrawals.RemoveRange(bws);                        
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == oBankWithdrawal.BillNo && x.financial_code == oBankWithdrawal.FinancialCode && x.bill_type == "BW");
                        string ltBillNo = "";
                        foreach (var item in lt)
                        {
                            ltBillNo = item.bill_no;
                            break;
                        }
                        dataB.ledger_transactions.RemoveRange(lt);

                        LedgerService ls = new LedgerService();
                        
                        for (int i = 0; i < oBankWithdrawal.Details.Count; i++)
                        {
                            bank_withdrawals bw = new bank_withdrawals();

                            bw.bill_no = oBankWithdrawal.BillNo;
                            bw.bill_date_time = oBankWithdrawal.BillDateTime;
                            bw.serial_no = oBankWithdrawal.Details.ElementAt(i).SerialNo;
                            bw.ledger_code = oBankWithdrawal.Details.ElementAt(i).LedgerCode;
                            bw.ledger = oBankWithdrawal.Details.ElementAt(i).Ledger;
                            bw.narration = oBankWithdrawal.Details.ElementAt(i).Narration;
                            bw.amount = oBankWithdrawal.Details.ElementAt(i).Amount;
                            bw.financial_code = oBankWithdrawal.FinancialCode;
                            bw.status = oBankWithdrawal.Details.ElementAt(i).Status;
                            bw.bank = oBankWithdrawal.Bank;
                            bw.bank_code = oBankWithdrawal.BankCode;

                            dataB.bank_withdrawals.Add(bw);
                            
                            //Saving to Ledger Transaction
                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo;
                            lt1.bill_date_time = oBankWithdrawal.BillDateTime;
                            lt1.bill_type = "BW";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oBankWithdrawal.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oBankWithdrawal.Details.ElementAt(i).Ledger;
                            lt1.narration = oBankWithdrawal.Details.ElementAt(i).Narration;
                            lt1.debit = oBankWithdrawal.Details.ElementAt(i).Amount;
                            lt1.credit = 0;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oBankWithdrawal.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oBankWithdrawal.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oBankWithdrawal.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = oBankWithdrawal.Bank;
                            lt1.ref_bill_no = oBankWithdrawal.BillNo;
                            lt1.ref_bill_date_time = oBankWithdrawal.BillDateTime;
                            lt1.financial_code = oBankWithdrawal.FinancialCode;

                            //Second entry
                            ledger_transactions lt2 = new ledger_transactions();

                            lt2.bill_no = ltBillNo;
                            lt2.bill_date_time = oBankWithdrawal.BillDateTime;
                            lt2.bill_type = "BW";
                            lt2.serial_no = i+2;
                            lt2.ledger_code = oBankWithdrawal.BankCode;
                            lt2.ledger = oBankWithdrawal.Bank;
                            lt2.narration = oBankWithdrawal.Details.ElementAt(i).Narration;
                            lt2.debit = 0;
                            lt2.credit = oBankWithdrawal.Details.ElementAt(i).Amount;
                            lt2.a_group_code = ls.ReadAGroupCodeOf(oBankWithdrawal.BankCode);
                            lt2.b_group_code = ls.ReadBGroupCodeOf(oBankWithdrawal.BankCode);
                            lt2.c_group_code = ls.ReadCGroupCodeOf(oBankWithdrawal.BankCode);
                            lt2.co_ledger = oBankWithdrawal.Details.ElementAt(i).Ledger;
                            lt2.ref_bill_no = oBankWithdrawal.BillNo;
                            lt2.ref_bill_date_time = oBankWithdrawal.BillDateTime;
                            lt2.financial_code = oBankWithdrawal.FinancialCode;

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
        public List<CBankWithdrawalReportDetailed> FindBankWithdrawalsDetailed(DateTime startDate, DateTime endDate, string billNo, string bankCode, string bank, string ledgerCode, string ledger, string status, string narration, string financialCode)
        {
            List<CBankWithdrawalReportDetailed> report = new List<CBankWithdrawalReportDetailed>();

            using (var dataB = new Database9002Entities())
            {
                string startD = startDate.Year + "-" + startDate.Month + "-" + startDate.Day;
                string endD = endDate.Year + "-" + endDate.Month + "-" + endDate.Day;

                string billNoQuery = billNo.Trim().Equals("") ? "" : " && (bd.bill_no='" + billNo.Trim() + "') ";
                string ledgerCodeQuery = ledgerCode.Trim().Equals("") ? "" : " && (bd.ledger_code='" + ledgerCode.Trim() + "') ";
                string ledgerQuery = ledger.Trim().Equals("") ? "" : " && (bd.ledger Like '%" + ledger.Trim() + "%') ";
                string bankCodeQuery = bankCode.Trim().Equals("") ? "" : " && (bd.bank_code='" + bankCode.Trim() + "') ";
                string bankQuery = bank.Trim().Equals("") ? "" : " && (bd.bank Like '%" + bank.Trim() + "%') ";
                string statusQuery = status.Trim().Equals("") ? "" : " && (bd.status='" + status.Trim() + "') ";
                string narrationQuery = narration.Trim().Equals("") ? "" : " && (bd.narration Like '%" + narration.Trim() + "%') ";
                string financialCodeQuery = financialCode.Trim().Equals("") ? "" : " && (bd.financial_code='" + financialCode.Trim() + "') ";

                string subQ = billNoQuery + ledgerCodeQuery + ledgerQuery + bankCodeQuery + bankQuery + statusQuery + narrationQuery + financialCodeQuery;

                var resData = dataB.Database.SqlQuery<CBankWithdrawalReportDetailed>("Select bd.bill_date_time As BillDateTime, bd.bill_no As BillNo, bd.serial_no As SerialNo, bd.bank_code As BankCode, bd.bank As Bank, bd.ledger_code As LedgerCode, bd.ledger As Ledger, bd.narration as Narration, bd.financial_code As FinancialCode, bd.amount As Amount, bd.status As Status From bank_deposits bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + " Order By bd.bill_date_time,bd.bill_no,bd.serial_no ");

                decimal? amounts = 0;
                foreach (var item in resData)
                {
                    amounts = amounts + item.Amount;
                    report.Add(item);
                }
                //Total
                report.Add(new CBankWithdrawalReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "" });
                report.Add(new CBankWithdrawalReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "Total", Amount = amounts });

            }


            return report;
        }

        public List<CBankWithdrawalReportSummary> FindBankWithdrawalsSummary(DateTime startDate, DateTime endDate, string billNo, string bankCode, string bank, string ledgerCode, string ledger, string status, string narration, string financialCode)
        {
            List<CBankWithdrawalReportSummary> report = new List<CBankWithdrawalReportSummary>();

            using (var dataB = new Database9002Entities())
            {
                string startD = startDate.Year + "-" + startDate.Month + "-" + startDate.Day;
                string endD = endDate.Year + "-" + endDate.Month + "-" + endDate.Day;

                string billNoQuery = billNo.Trim().Equals("") ? "" : " && (bd.bill_no='" + billNo.Trim() + "') ";
                string ledgerCodeQuery = ledgerCode.Trim().Equals("") ? "" : " && (bd.ledger_code='" + ledgerCode.Trim() + "') ";
                string ledgerQuery = ledger.Trim().Equals("") ? "" : " && (bd.ledger Like '%" + ledger.Trim() + "%') ";
                string bankCodeQuery = bankCode.Trim().Equals("") ? "" : " && (bd.bank_code='" + bankCode.Trim() + "') ";
                string bankQuery = bank.Trim().Equals("") ? "" : " && (bd.bank Like '%" + bank.Trim() + "%') ";
                string statusQuery = status.Trim().Equals("") ? "" : " && (bd.status='" + status.Trim() + "') ";
                string narrationQuery = narration.Trim().Equals("") ? "" : " && (bd.narration Like '%" + narration.Trim() + "%') ";
                string financialCodeQuery = financialCode.Trim().Equals("") ? "" : " && (bd.financial_code='" + financialCode.Trim() + "') ";

                string subQ = billNoQuery + ledgerCodeQuery + ledgerQuery + bankCodeQuery + bankQuery + statusQuery + narrationQuery + financialCodeQuery;

                var resData = dataB.Database.SqlQuery<CBankWithdrawalReportSummary>("Select Sum(bd.amount) As TotalAmount,bd.bill_date_time As BillDateTime,bd.bill_no As BillNo,bd.bank As bank, bd.bank_code As BankCode, bd.financial_code As FinancialCode From bank_deposits bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + "Group By bd.bill_date_time,bd.bill_no,bd.bank,bd.bank_code,bd.financial_code Order By bd.bill_date_time,bd.bill_no");

                decimal? amounts = 0;

                foreach (var item in resData)
                {
                    amounts = amounts + item.TotalAmount;
                    report.Add(item);
                }
                //Total
                report.Add(new CBankWithdrawalReportSummary() { BillDateTime = null, Bank = "" });
                report.Add(new CBankWithdrawalReportSummary() { BillDateTime = null, Bank = "Total", TotalAmount = amounts });

            }


            return report;
        }
    }
}
