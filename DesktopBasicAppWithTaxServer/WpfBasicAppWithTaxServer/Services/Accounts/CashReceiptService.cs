using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfServerApp;
using WpfServerApp.General;
using WpfServerApp.Services;

namespace WpfServerApp.Services.Accounts
{

    public class CashReceiptService : ICashReceipt
    {
        

        public bool CreateBill(CCashReceipt oCashReceipt)
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


                        int cbillNo = bs.ReadNextCashReceiptBillNo(oCashReceipt.FinancialCode);
                        bs.UpdateCashReceiptBillNo(oCashReceipt.FinancialCode,cbillNo+1);
                        //Saving to Ledger Transaction
                        int ltBillNo = bs.ReadNextLedgerTransactionBillNo(oCashReceipt.FinancialCode);
                        bs.UpdateLedgerTransactionBillNo(oCashReceipt.FinancialCode, ltBillNo + 1);

                        for (int i = 0; i < oCashReceipt.Details.Count; i++)
                        {
                            cash_receipts cr = new cash_receipts();

                            cr.bill_no = cbillNo.ToString();
                            cr.bill_date_time = oCashReceipt.BillDateTime;
                            cr.serial_no = oCashReceipt.Details.ElementAt(i).SerialNo;
                            cr.ledger_code= oCashReceipt.Details.ElementAt(i).LedgerCode;
                            cr.ledger = oCashReceipt.Details.ElementAt(i).Ledger;
                            cr.narration = oCashReceipt.Details.ElementAt(i).Narration;
                            cr.amount = oCashReceipt.Details.ElementAt(i).Amount;
                            cr.financial_code = oCashReceipt.FinancialCode;

                            dataB.cash_receipts.Add(cr);
                            
                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo.ToString();
                            lt1.bill_date_time = oCashReceipt.BillDateTime;
                            lt1.bill_type = "CR";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oCashReceipt.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oCashReceipt.Details.ElementAt(i).Ledger;
                            lt1.narration = oCashReceipt.Details.ElementAt(i).Narration;
                            lt1.debit = 0;
                            lt1.credit = oCashReceipt.Details.ElementAt(i).Amount;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oCashReceipt.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oCashReceipt.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oCashReceipt.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = "Cash";
                            lt1.ref_bill_no = cbillNo.ToString();
                            lt1.ref_bill_date_time = oCashReceipt.BillDateTime;
                            lt1.financial_code = oCashReceipt.FinancialCode;

                            //Second entry
                            ledger_transactions lt2 = new ledger_transactions();

                            lt2.bill_no = ltBillNo.ToString();
                            lt2.bill_date_time = oCashReceipt.BillDateTime;
                            lt2.bill_type = "CR";
                            lt2.serial_no = i+2;
                            lt2.ledger_code = UniqueLedgers.LedgerCode["Cash"];
                            lt2.ledger = "Cash";
                            lt2.narration = oCashReceipt.Details.ElementAt(i).Narration;
                            lt2.debit = oCashReceipt.Details.ElementAt(i).Amount;
                            lt2.credit = 0;
                            lt2.a_group_code = ls.ReadAGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.b_group_code = ls.ReadBGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.c_group_code = ls.ReadCGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.co_ledger = oCashReceipt.Details.ElementAt(i).Ledger;
                            lt2.ref_bill_no = cbillNo.ToString();
                            lt2.ref_bill_date_time = oCashReceipt.BillDateTime;
                            lt2.financial_code = oCashReceipt.FinancialCode;

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
                        var cr = dataB.cash_receipts.Select(c => c).Where(x=>x.bill_no==billNo&&x.financial_code==financialCode);
                        dataB.cash_receipts.RemoveRange(cr);
                                                                        

                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == billNo && x.financial_code == financialCode&&x.bill_type=="CR");
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

        public CCashReceipt ReadBill(string billNo,string financialCode)
        {
            CCashReceipt ccr = null;

            using (var dataB = new Database9002Entities())
            {
                var crs = dataB.cash_receipts.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode).OrderBy(y=>y.serial_no);
                
                if (crs.Count() > 0)
                {
                    ccr = new CCashReceipt();

                    var cr = crs.FirstOrDefault();
                    ccr.Id = cr.id;
                    ccr.BillNo = cr.bill_no;
                    ccr.BillDateTime = cr.bill_date_time;
                    ccr.FinancialCode = cr.financial_code;

                    foreach (var item in crs)
                    {
                        ccr.Details.Add(new CCashReceiptDetails() { SerialNo=item.serial_no,LedgerCode=item.ledger_code,Ledger=item.ledger, Narration=item.narration, Amount=item.amount });
                    }
                }
                
            }

            return ccr;
        }

        public int ReadNextBillNo(string financialCode)
        {
            
            BillNoService bns = new BillNoService();
            return bns.ReadNextCashReceiptBillNo(financialCode);
            
        }

        public bool UpdateBill(CCashReceipt oCashReceipt)
        {
            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9002Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        //Remove already entered data from Cash Receipt
                        var crr = dataB.cash_receipts.Select(c => c).Where(x => x.bill_no == oCashReceipt.BillNo&& x.financial_code==oCashReceipt.FinancialCode);
                        dataB.cash_receipts.RemoveRange(crr);
                        //Delete from Ledger Transaction
                        var lt = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == oCashReceipt.BillNo && x.financial_code == oCashReceipt.FinancialCode && x.bill_type == "CR");
                        string ltBillNo = "";
                        foreach (var item in lt)
                        {
                            ltBillNo = item.bill_no;
                            break;
                        }
                        dataB.ledger_transactions.RemoveRange(lt);

                        //Add newly updated data
                        LedgerService ls = new LedgerService();
                        
                        for (int i = 0; i < oCashReceipt.Details.Count; i++)
                        {
                            cash_receipts cr = new cash_receipts();

                            cr.bill_no = oCashReceipt.BillNo;
                            cr.bill_date_time = oCashReceipt.BillDateTime;
                            cr.serial_no = oCashReceipt.Details.ElementAt(i).SerialNo;
                            cr.ledger_code = oCashReceipt.Details.ElementAt(i).LedgerCode;
                            cr.ledger = oCashReceipt.Details.ElementAt(i).Ledger;
                            cr.narration = oCashReceipt.Details.ElementAt(i).Narration;
                            cr.amount = oCashReceipt.Details.ElementAt(i).Amount;
                            cr.financial_code = oCashReceipt.FinancialCode;

                            dataB.cash_receipts.Add(cr);


                            //Saving to Ledger Transaction
                        

                            //First entry
                            ledger_transactions lt1 = new ledger_transactions();

                            lt1.bill_no = ltBillNo;
                            lt1.bill_date_time = oCashReceipt.BillDateTime;
                            lt1.bill_type = "CR";
                            lt1.serial_no = i+1;
                            lt1.ledger_code = oCashReceipt.Details.ElementAt(i).LedgerCode;
                            lt1.ledger = oCashReceipt.Details.ElementAt(i).Ledger;
                            lt1.narration = oCashReceipt.Details.ElementAt(i).Narration;
                            lt1.debit = 0;
                            lt1.credit = oCashReceipt.Details.ElementAt(i).Amount;
                            lt1.a_group_code = ls.ReadAGroupCodeOf(oCashReceipt.Details.ElementAt(i).LedgerCode);
                            lt1.b_group_code = ls.ReadBGroupCodeOf(oCashReceipt.Details.ElementAt(i).LedgerCode);
                            lt1.c_group_code = ls.ReadCGroupCodeOf(oCashReceipt.Details.ElementAt(i).LedgerCode);
                            lt1.co_ledger = "Cash";
                            lt1.ref_bill_no = oCashReceipt.BillNo;
                            lt1.ref_bill_date_time = oCashReceipt.BillDateTime;
                            lt1.financial_code = oCashReceipt.FinancialCode;

                            //Second entry
                            ledger_transactions lt2 = new ledger_transactions();

                            lt2.bill_no = ltBillNo;
                            lt2.bill_date_time = oCashReceipt.BillDateTime;
                            lt2.bill_type = "CR";
                            lt2.serial_no = i+2;
                            lt2.ledger_code = UniqueLedgers.LedgerCode["Cash"];
                            lt2.ledger = "Cash";
                            lt2.narration = oCashReceipt.Details.ElementAt(i).Narration;
                            lt2.debit = oCashReceipt.Details.ElementAt(i).Amount;
                            lt2.credit = 0;
                            lt2.a_group_code = ls.ReadAGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.b_group_code = ls.ReadBGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.c_group_code = ls.ReadCGroupCodeOf(UniqueLedgers.LedgerCode["Cash"]);
                            lt2.co_ledger = oCashReceipt.Details.ElementAt(i).Ledger;
                            lt2.ref_bill_no = oCashReceipt.BillNo;
                            lt2.ref_bill_date_time = oCashReceipt.BillDateTime;
                            lt2.financial_code = oCashReceipt.FinancialCode;

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
        public List<CCashReceiptReportDetailed> FindCashReceiptsDetailed(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode)
        {
            List<CCashReceiptReportDetailed> report = new List<CCashReceiptReportDetailed>();

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

                var resData = dataB.Database.SqlQuery<CCashReceiptReportDetailed>("Select bd.bill_date_time As BillDateTime, bd.bill_no As BillNo, bd.serial_no As SerialNo, bd.ledger_code As LedgerCode, bd.ledger As Ledger, bd.narration as Narration, bd.financial_code As FinancialCode, bd.amount As Amount From cash_receipts bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + " Order By bd.bill_date_time,bd.bill_no,bd.serial_no ");

                decimal? amounts = 0;
                foreach (var item in resData)
                {
                    amounts = amounts + item.Amount;
                    report.Add(item);
                }
                //Total
                report.Add(new CCashReceiptReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "" });
                report.Add(new CCashReceiptReportDetailed() { BillDateTime = null, SerialNo = null, Ledger = "Total", Amount = amounts });

            }


            return report;
        }

        public List<CCashReceiptReportSummary> FindCashReceiptsSummary(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode)
        {
            List<CCashReceiptReportSummary> report = new List<CCashReceiptReportSummary>();

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

                var resData = dataB.Database.SqlQuery<CCashReceiptReportSummary>("Select Sum(bd.amount) As TotalAmount,bd.bill_date_time As BillDateTime,bd.bill_no As BillNo, bd.financial_code As FinancialCode From cash_receipts bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + "Group By bd.bill_date_time,bd.bill_no,bd.financial_code Order By bd.bill_date_time,bd.bill_no");

                decimal? amounts = 0;

                foreach (var item in resData)
                {
                    amounts = amounts + item.TotalAmount;
                    report.Add(item);
                }
                //Total
                report.Add(new CCashReceiptReportSummary() { BillDateTime = null, BillNo = "" });
                report.Add(new CCashReceiptReportSummary() { BillDateTime = null, BillNo = "Total", TotalAmount = amounts });

            }


            return report;
        }
    }
}
