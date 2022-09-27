using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfServerApp;
using WpfServerApp.General;
using WpfServerApp.Services;

namespace WpfServerApp.Services.Accounts
{
    public class CashPaymentService : ICashPayment
    {
        

        public bool CreateBill(CCashPayment oCashPayment)
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


                        int cbillNo = bs.ReadNextCashPaymentBillNo(oCashPayment.FinancialCode);
                        bs.UpdateCashPaymentBillNo(oCashPayment.FinancialCode,cbillNo+1);

                        //Saving to Ledger Transaction
                        int ltBillNo = bs.ReadNextLedgerTransactionBillNo(oCashPayment.FinancialCode);
                        bs.UpdateLedgerTransactionBillNo(oCashPayment.FinancialCode, ltBillNo + 1);

                        for (int i = 0; i < oCashPayment.Details.Count; i++)
                        {
                            cash_payments cp = new cash_payments();

                            cp.bill_no = cbillNo.ToString();
                            cp.bill_date_time = oCashPayment.BillDateTime;
                            cp.serial_no = oCashPayment.Details.ElementAt(i).SerialNo;
                            cp.ledger_code= oCashPayment.Details.ElementAt(i).LedgerCode;
                            cp.ledger = oCashPayment.Details.ElementAt(i).Ledger;
                            cp.narration = oCashPayment.Details.ElementAt(i).Narration;
                            cp.amount = oCashPayment.Details.ElementAt(i).Amount;
                            cp.financial_code = oCashPayment.FinancialCode;

                            dataB.cash_payments.Add(cp);
                            

                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo.ToString();
                            lt1.bill_date_time = oCashPayment.BillDateTime;
                            lt1.bill_type = "CP";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oCashPayment.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oCashPayment.Details.ElementAt(i).Ledger;
                            lt1.narration = oCashPayment.Details.ElementAt(i).Narration;
                            lt1.debit = oCashPayment.Details.ElementAt(i).Amount;
                            lt1.credit = 0;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oCashPayment.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oCashPayment.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oCashPayment.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = "Cash";
                            lt1.ref_bill_no = cbillNo.ToString();
                            lt1.ref_bill_date_time = oCashPayment.BillDateTime;
                            lt1.financial_code = oCashPayment.FinancialCode;

                            //Second entry
                            ledger_transactions lt2 = new ledger_transactions();

                            lt2.bill_no = ltBillNo.ToString();
                            lt2.bill_date_time = oCashPayment.BillDateTime;
                            lt2.bill_type = "CP";
                            lt2.serial_no = i+2;
                            lt2.ledger_code = UniqueLedgers.LedgerCode["Cash"];
                            lt2.ledger = "Cash";
                            lt2.narration = oCashPayment.Details.ElementAt(i).Narration;
                            lt2.debit = 0;
                            lt2.credit = oCashPayment.Details.ElementAt(i).Amount;
                            lt2.a_group_code = ls.ReadAGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.b_group_code = ls.ReadBGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.c_group_code = ls.ReadCGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.co_ledger = oCashPayment.Details.ElementAt(i).Ledger;
                            lt2.ref_bill_no = cbillNo.ToString();
                            lt2.ref_bill_date_time = oCashPayment.BillDateTime;
                            lt2.financial_code = oCashPayment.FinancialCode;

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
                        var cr = dataB.cash_payments.Select(c => c).Where(x=>x.bill_no==billNo&&x.financial_code==financialCode);
                        dataB.cash_payments.RemoveRange(cr);
                                                                        

                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == billNo && x.financial_code == financialCode&&x.bill_type=="CP");
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

        public CCashPayment ReadBill(string billNo,string financialCode)
        {
            CCashPayment ccp = null;

            using (var dataB = new Database9002Entities())
            {
                var cps = dataB.cash_payments.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode).OrderBy(y=>y.serial_no);
                
                if (cps.Count() > 0)
                {
                    ccp = new CCashPayment();

                    var cp = cps.FirstOrDefault();
                    ccp.Id = cp.id;
                    ccp.BillNo = cp.bill_no;
                    ccp.BillDateTime = cp.bill_date_time;
                    ccp.FinancialCode = cp.financial_code;

                    foreach (var item in cps)
                    {
                        ccp.Details.Add(new CCashPaymentDetails() { SerialNo=item.serial_no,LedgerCode=item.ledger_code,Ledger=item.ledger, Narration=item.narration, Amount=item.amount });
                    }
                }
                
            }

            return ccp;
        }

        public int ReadNextBillNo(string financialCode)
        {
            
            BillNoService bns = new BillNoService();
            return bns.ReadNextCashPaymentBillNo(financialCode);
            
        }

        public bool UpdateBill(CCashPayment oCashPayment)
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
                        var cpp = dataB.cash_payments.Select(c => c).Where(x => x.bill_no == oCashPayment.BillNo&& x.financial_code==oCashPayment.FinancialCode);
                        dataB.cash_payments.RemoveRange(cpp);
                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == oCashPayment.BillNo && x.financial_code == oCashPayment.FinancialCode && x.bill_type == "CP");
                        string ltBillNo = "";
                        foreach (var item in lt)
                        {
                            ltBillNo = item.bill_no;
                            break;
                        }
                        dataB.ledger_transactions.RemoveRange(lt);

                        //Add newly updated data
                        LedgerService ls = new LedgerService();
                        
                        for (int i = 0; i < oCashPayment.Details.Count; i++)
                        {
                            cash_payments cp = new cash_payments();

                            cp.bill_no = oCashPayment.BillNo;
                            cp.bill_date_time = oCashPayment.BillDateTime;
                            cp.serial_no = oCashPayment.Details.ElementAt(i).SerialNo;
                            cp.ledger_code = oCashPayment.Details.ElementAt(i).LedgerCode;
                            cp.ledger = oCashPayment.Details.ElementAt(i).Ledger;
                            cp.narration = oCashPayment.Details.ElementAt(i).Narration;
                            cp.amount = oCashPayment.Details.ElementAt(i).Amount;
                            cp.financial_code = oCashPayment.FinancialCode;

                            dataB.cash_payments.Add(cp);


                            //Saving to Ledger Transaction
                        

                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo;
                            lt1.bill_date_time = oCashPayment.BillDateTime;
                            lt1.bill_type = "CP";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oCashPayment.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oCashPayment.Details.ElementAt(i).Ledger;
                            lt1.narration = oCashPayment.Details.ElementAt(i).Narration;
                            lt1.debit = oCashPayment.Details.ElementAt(i).Amount;
                            lt1.credit = 0;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oCashPayment.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oCashPayment.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oCashPayment.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = "Cash";
                            lt1.ref_bill_no = oCashPayment.BillNo;
                            lt1.ref_bill_date_time = oCashPayment.BillDateTime;
                            lt1.financial_code = oCashPayment.FinancialCode;

                            //Second entry
                            ledger_transactions lt2 = new ledger_transactions();

                            lt2.bill_no = ltBillNo;
                            lt2.bill_date_time = oCashPayment.BillDateTime;
                            lt2.bill_type = "CP";
                            lt2.serial_no = i+2;
                            lt2.ledger_code = UniqueLedgers.LedgerCode["Cash"];
                            lt2.ledger = "Cash";
                            lt2.narration = oCashPayment.Details.ElementAt(i).Narration;
                            lt2.debit = 0;
                            lt2.credit = oCashPayment.Details.ElementAt(i).Amount;
                            lt2.a_group_code = ls.ReadAGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.b_group_code = ls.ReadBGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.c_group_code = ls.ReadCGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.co_ledger = oCashPayment.Details.ElementAt(i).Ledger;
                            lt2.ref_bill_no = oCashPayment.BillNo;
                            lt2.ref_bill_date_time = oCashPayment.BillDateTime;
                            lt2.financial_code = oCashPayment.FinancialCode;

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
        public List<CCashPaymentReportDetailed> FindCashPaymentsDetailed(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode)
        {
            List<CCashPaymentReportDetailed> report = new List<CCashPaymentReportDetailed>();

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

                var resData = dataB.Database.SqlQuery<CCashPaymentReportDetailed>("Select bd.bill_date_time As BillDateTime, bd.bill_no As BillNo, bd.serial_no As SerialNo, bd.ledger_code As LedgerCode, bd.ledger As Ledger, bd.narration as Narration, bd.financial_code As FinancialCode, bd.amount As Amount From cash_payments bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + " Order By bd.bill_date_time,bd.bill_no,bd.serial_no ");

                decimal? amounts = 0;
                foreach (var item in resData)
                {
                    amounts = amounts + item.Amount;
                    report.Add(item);
                }
                //Total
                report.Add(new CCashPaymentReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "" });
                report.Add(new CCashPaymentReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "Total", Amount = amounts });

            }


            return report;
        }

        public List<CCashPaymentReportSummary> FindCashPaymentsSummary(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode)
        {
            List<CCashPaymentReportSummary> report = new List<CCashPaymentReportSummary>();

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

                var resData = dataB.Database.SqlQuery<CCashPaymentReportSummary>("Select Sum(bd.amount) As TotalAmount,bd.bill_date_time As BillDateTime,bd.bill_no As BillNo, bd.financial_code As FinancialCode From cash_payments bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + "Group By bd.bill_date_time,bd.bill_no,bd.financial_code Order By bd.bill_date_time,bd.bill_no");

                decimal? amounts = 0;

                foreach (var item in resData)
                {
                    amounts = amounts + item.TotalAmount;
                    report.Add(item);
                }
                //Total
                report.Add(new CCashPaymentReportSummary() { BillDateTime = null, BillNo = "" });
                report.Add(new CCashPaymentReportSummary() { BillDateTime = null, BillNo = "Total", TotalAmount = amounts });

            }


            return report;
        }
    }
}
