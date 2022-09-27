using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfServerApp;
using WpfServerApp.General;
using WpfServerApp.Services;

namespace WpfServerApp.Services.Accounts
{
    public class JournalVoucherService : IJournalVoucher
    {
        

        public bool CreateBill(CJournalVoucher oJournalVoucher)
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


                        int cbillNo = bs.ReadNextJournalVoucherBillNo(oJournalVoucher.FinancialCode);
                        bs.UpdateJournalVoucherBillNo(oJournalVoucher.FinancialCode,cbillNo+1);

                        //Saving to Ledger Transaction
                        int ltBillNo = bs.ReadNextLedgerTransactionBillNo(oJournalVoucher.FinancialCode);
                        bs.UpdateLedgerTransactionBillNo(oJournalVoucher.FinancialCode, ltBillNo + 1);

                        for (int i = 0; i < oJournalVoucher.Details.Count; i++)
                        {
                            journal_vouchers jv = new journal_vouchers();

                            jv.bill_no = cbillNo.ToString();
                            jv.bill_date_time = oJournalVoucher.BillDateTime;
                            jv.serial_no = oJournalVoucher.Details.ElementAt(i).SerialNo;
                            jv.ledger_code= oJournalVoucher.Details.ElementAt(i).LedgerCode;
                            jv.ledger = oJournalVoucher.Details.ElementAt(i).Ledger;
                            jv.narration = oJournalVoucher.Details.ElementAt(i).Narration;
                            jv.debit = oJournalVoucher.Details.ElementAt(i).Debit;
                            jv.credit = oJournalVoucher.Details.ElementAt(i).Credit;
                            jv.financial_code = oJournalVoucher.FinancialCode;                            

                            dataB.journal_vouchers.Add(jv);
                            

                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo.ToString();
                            lt1.bill_date_time = oJournalVoucher.BillDateTime;
                            lt1.bill_type = "JV";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oJournalVoucher.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oJournalVoucher.Details.ElementAt(i).Ledger;
                            lt1.narration = oJournalVoucher.Details.ElementAt(i).Narration;
                            lt1.debit = oJournalVoucher.Details.ElementAt(i).Debit;
                            lt1.credit = oJournalVoucher.Details.ElementAt(i).Credit;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oJournalVoucher.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oJournalVoucher.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oJournalVoucher.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = "Journal Voucher";
                            lt1.ref_bill_no = cbillNo.ToString();
                            lt1.ref_bill_date_time = oJournalVoucher.BillDateTime;
                            lt1.financial_code = oJournalVoucher.FinancialCode;
                            
                            dataB.ledger_transactions.Add(lt1);
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
                        var jv = dataB.journal_vouchers.Select(c => c).Where(x=>x.bill_no==billNo&&x.financial_code==financialCode);
                        dataB.journal_vouchers.RemoveRange(jv);
                                                                        

                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == billNo && x.financial_code == financialCode&&x.bill_type=="JV");
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

        public CJournalVoucher ReadBill(string billNo,string financialCode)
        {
            CJournalVoucher cjv = null;

            using (var dataB = new Database9002Entities())
            {
                var jvs = dataB.journal_vouchers.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode).OrderBy(y=>y.serial_no);
                
                if (jvs.Count() > 0)
                {
                    cjv = new CJournalVoucher();

                    var jv = jvs.FirstOrDefault();
                    cjv.Id = jv.id;
                    cjv.BillNo = jv.bill_no;
                    cjv.BillDateTime = jv.bill_date_time;
                    cjv.FinancialCode = jv.financial_code;
                    
                    foreach (var item in jvs)
                    {
                        cjv.Details.Add(new CJournalVoucherDetails() { SerialNo=item.serial_no,LedgerCode=item.ledger_code,Ledger=item.ledger, Narration=item.narration, Debit=item.debit,Credit=item.credit });
                    }
                }
                
            }

            return cjv;
        }

        public int ReadNextBillNo(string financialCode)
        {
            
            BillNoService bns = new BillNoService();
            return bns.ReadNextJournalVoucherBillNo(financialCode);
            
        }

        public bool UpdateBill(CJournalVoucher oJournalVoucher)
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
                        var jvv = dataB.journal_vouchers.Select(c => c).Where(x => x.bill_no == oJournalVoucher.BillNo&& x.financial_code==oJournalVoucher.FinancialCode);
                        dataB.journal_vouchers.RemoveRange(jvv);
                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == oJournalVoucher.BillNo && x.financial_code == oJournalVoucher.FinancialCode && x.bill_type == "JV");
                        string ltBillNo = "";
                        foreach (var item in lt)
                        {
                            ltBillNo = item.bill_no;
                            break;
                        }
                        dataB.ledger_transactions.RemoveRange(lt);

                        //Add newly updated data
                        LedgerService ls = new LedgerService();
                        
                        for (int i = 0; i < oJournalVoucher.Details.Count; i++)
                        {
                            journal_vouchers jv = new journal_vouchers();

                            jv.bill_no = oJournalVoucher.BillNo;
                            jv.bill_date_time = oJournalVoucher.BillDateTime;
                            jv.serial_no = oJournalVoucher.Details.ElementAt(i).SerialNo;
                            jv.ledger_code = oJournalVoucher.Details.ElementAt(i).LedgerCode;
                            jv.ledger = oJournalVoucher.Details.ElementAt(i).Ledger;
                            jv.narration = oJournalVoucher.Details.ElementAt(i).Narration;
                            jv.debit = oJournalVoucher.Details.ElementAt(i).Debit;
                            jv.credit = oJournalVoucher.Details.ElementAt(i).Credit;
                            jv.financial_code = oJournalVoucher.FinancialCode;

                            dataB.journal_vouchers.Add(jv);


                            //Saving to Ledger Transaction
                        

                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo;
                            lt1.bill_date_time = oJournalVoucher.BillDateTime;
                            lt1.bill_type = "JV";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oJournalVoucher.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oJournalVoucher.Details.ElementAt(i).Ledger;
                            lt1.narration = oJournalVoucher.Details.ElementAt(i).Narration;
                            lt1.debit = oJournalVoucher.Details.ElementAt(i).Debit;
                            lt1.credit = oJournalVoucher.Details.ElementAt(i).Debit;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oJournalVoucher.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oJournalVoucher.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oJournalVoucher.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = "Journal Voucher";
                            lt1.ref_bill_no = oJournalVoucher.BillNo;
                            lt1.ref_bill_date_time = oJournalVoucher.BillDateTime;
                            lt1.financial_code = oJournalVoucher.FinancialCode;
                            
                            dataB.ledger_transactions.Add(lt1);                            
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
        public List<CJournalVoucherReportDetailed> FindJournalVouchersDetailed(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode)
        {
            List<CJournalVoucherReportDetailed> report = new List<CJournalVoucherReportDetailed>();

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

                var resData = dataB.Database.SqlQuery<CJournalVoucherReportDetailed>("Select bd.bill_date_time As BillDateTime, bd.bill_no As BillNo, bd.serial_no As SerialNo, bd.ledger_code As LedgerCode, bd.ledger As Ledger, bd.narration as Narration, bd.financial_code As FinancialCode, bd.debit As Debit, bd.credit As Debit From journal_vouchers bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + " Order By bd.bill_date_time,bd.bill_no,bd.serial_no ");

                decimal? credits = 0;
                decimal? debits = 0;
                foreach (var item in resData)
                {
                    debits = debits + item.Debit;
                    credits = credits + item.Credit;
                    report.Add(item);
                }
                //Total
                report.Add(new CJournalVoucherReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "" });
                report.Add(new CJournalVoucherReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "Total", Debit = debits, Credit = credits });

            }


            return report;
        }

        public List<CJournalVoucherReportSummary> FindJournalVouchersSummary(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode)
        {
            List<CJournalVoucherReportSummary> report = new List<CJournalVoucherReportSummary>();

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

                var resData = dataB.Database.SqlQuery<CJournalVoucherReportSummary>("Select Sum(bd.debit) As TotalDebit,Sum(bd.credit) As TotalCredit,bd.bill_date_time As BillDateTime,bd.bill_no As BillNo, bd.financial_code As FinancialCode From journal_vouchers bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + "Group By bd.bill_date_time,bd.bill_no,bd.financial_code Order By bd.bill_date_time,bd.bill_no");

                decimal? debit = 0;
                decimal? credit = 0;

                foreach (var item in resData)
                {
                    credit = credit + item.TotalCredit;
                    debit = debit + item.TotalDebit;
                    report.Add(item);
                }
                //Total
                report.Add(new CJournalVoucherReportSummary() { BillDateTime = null, BillNo = "" });
                report.Add(new CJournalVoucherReportSummary() { BillDateTime = null, BillNo = "Total", TotalDebit = debit, TotalCredit = credit });

            }


            return report;
        }

    }
}
