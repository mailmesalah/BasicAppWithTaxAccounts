using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServerServiceInterface
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ICashReceipt
    {
        [OperationContract]
        bool CreateBill(CCashReceipt oCashReceipt);
        [OperationContract]
        CCashReceipt ReadBill(string billNo,string financialCode);
        [OperationContract]
        bool UpdateBill(CCashReceipt oCashReceipt);
        [OperationContract]
        bool DeleteBill(string billNo,string financialCode);        
        [OperationContract]
        int ReadNextBillNo(string financialCode);

        [OperationContract]
        List<CCashReceiptReportSummary> FindCashReceiptsSummary(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode);
        [OperationContract]
        List<CCashReceiptReportDetailed> FindCashReceiptsDetailed(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode);
    }


    
    [DataContract]
    public class CCashReceipt
    {
        int id;
        string billNo;
        DateTime billDateTime = new DateTime();
        string financialCode;
        List<CCashReceiptDetails> details= new List<CCashReceiptDetails>();
 
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
        public List<CCashReceiptDetails> Details
        {
            get { return details; }
            set { details = value; }
        }
    }


    [DataContract]
    public class CCashReceiptDetails
    {
        int serialNo;
        string ledgerCode;
        string ledger;
        string narration;
        decimal amount;

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
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
    }


    [DataContract]
    public class CCashReceiptReportSummary
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string financialCode;
        decimal? totalAmount;

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
        public decimal? TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }
    }


    [DataContract]
    public class CCashReceiptReportDetailed
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string financialCode;
        int? serialNo;
        string ledgerCode;
        string ledger;
        string narration;
        decimal? amount;

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
        public decimal? Amount
        {
            get { return amount; }
            set { amount = value; }
        }

    }
}
