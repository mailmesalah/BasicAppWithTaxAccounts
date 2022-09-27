using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServerServiceInterface
{    
    [ServiceContract]
    public interface IJournalVoucher
    {
        [OperationContract]
        bool CreateBill(CJournalVoucher oJournalVoucher);
        [OperationContract]
        CJournalVoucher ReadBill(string billNo,string financialCode);
        [OperationContract]
        bool UpdateBill(CJournalVoucher oJournalVoucher);
        [OperationContract]
        bool DeleteBill(string billNo,string financialCode);        
        [OperationContract]
        int ReadNextBillNo(string financialCode);

        [OperationContract]
        List<CJournalVoucherReportSummary> FindJournalVouchersSummary(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode);
        [OperationContract]
        List<CJournalVoucherReportDetailed> FindJournalVouchersDetailed(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode);
    }


    
    [DataContract]
    public class CJournalVoucher
    {
        int id;
        string billNo;
        DateTime billDateTime = new DateTime();
        string financialCode;
        List<CJournalVoucherDetails> details= new List<CJournalVoucherDetails>();
 
        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string BillNo
        {
            get { return billNo; }
            set { billNo = value; }
        }

        [DataMember]
        public DateTime BillDateTime
        {
            get { return billDateTime; }
            set { billDateTime = value; }
        }
        
        [DataMember]
        public string FinancialCode
        {
            get { return financialCode; }
            set { financialCode = value; }
        }

        [DataMember]
        public List<CJournalVoucherDetails> Details
        {
            get { return details; }
            set { details = value; }
        }
    }


    [DataContract]
    public class CJournalVoucherDetails
    {
        int serialNo;
        string ledgerCode;
        string ledger;
        string narration;
        decimal debit;
        decimal credit;

        [DataMember]
        public int SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }

        [DataMember]
        public string LedgerCode
        {
            get { return ledgerCode; }
            set { ledgerCode = value; }
        }

        [DataMember]
        public string Ledger
        {
            get { return ledger; }
            set { ledger = value; }
        }

        [DataMember]
        public string Narration
        {
            get { return narration; }
            set { narration = value; }
        }

        [DataMember]
        public decimal Debit
        {
            get { return debit; }
            set { debit = value; }
        }

        [DataMember]
        public decimal Credit
        {
            get { return credit; }
            set { credit = value; }
        }
    }


    [DataContract]
    public class CJournalVoucherReportSummary
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string financialCode;
        decimal? totalDebit;
        decimal? totalCredit;

        [DataMember]
        public string BillNo
        {
            get { return billNo; }
            set { billNo = value; }
        }

        [DataMember]
        public DateTime? BillDateTime
        {
            get { return billDateTime; }
            set { billDateTime = value; }
        }

        [DataMember]
        public string FinancialCode
        {
            get { return financialCode; }
            set { financialCode = value; }
        }

        [DataMember]
        public decimal? TotalDebit
        {
            get { return totalDebit; }
            set { totalDebit = value; }
        }

        [DataMember]
        public decimal? TotalCredit
        {
            get { return totalCredit; }
            set { totalCredit = value; }
        }
    }


    [DataContract]
    public class CJournalVoucherReportDetailed
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string financialCode;
        int? serialNo;
        string ledgerCode;
        string ledger;
        string narration;
        decimal? debit;
        decimal? credit;

        [DataMember]
        public string BillNo
        {
            get { return billNo; }
            set { billNo = value; }
        }

        [DataMember]
        public DateTime? BillDateTime
        {
            get { return billDateTime; }
            set { billDateTime = value; }
        }

        [DataMember]
        public string FinancialCode
        {
            get { return financialCode; }
            set { financialCode = value; }
        }

        [DataMember]
        public int? SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }

        [DataMember]
        public string LedgerCode
        {
            get { return ledgerCode; }
            set { ledgerCode = value; }
        }

        [DataMember]
        public string Ledger
        {
            get { return ledger; }
            set { ledger = value; }
        }

        [DataMember]
        public string Narration
        {
            get { return narration; }
            set { narration = value; }
        }

        [DataMember]
        public decimal? Debit
        {
            get { return debit; }
            set { debit = value; }
        }

        [DataMember]
        public decimal? Credit
        {
            get { return credit; }
            set { credit = value; }
        }

    }
}
