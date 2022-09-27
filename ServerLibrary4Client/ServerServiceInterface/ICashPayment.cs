using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServerServiceInterface
{    
    [ServiceContract]
    public interface ICashPayment
    {
        [OperationContract]
        bool CreateBill(CCashPayment oCashPayment);
        [OperationContract]
        CCashPayment ReadBill(string billNo,string financialCode);
        [OperationContract]
        bool UpdateBill(CCashPayment oCashPayment);
        [OperationContract]
        bool DeleteBill(string billNo,string financialCode);        
        [OperationContract]
        int ReadNextBillNo(string financialCode);

        [OperationContract]
        List<CCashPaymentReportSummary> FindCashPaymentsSummary(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode);
        [OperationContract]
        List<CCashPaymentReportDetailed> FindCashPaymentsDetailed(DateTime startDate, DateTime endDate, string billNo, string ledgerCode, string ledger, string narration, string financialCode);

    }


    
    [DataContract]
    public class CCashPayment
    {
        int id;
        string billNo;
        DateTime billDateTime = new DateTime();
        string financialCode;
        List<CCashPaymentDetails> details= new List<CCashPaymentDetails>();
 
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
        public List<CCashPaymentDetails> Details
        {
            get { return details; }
            set { details = value; }
        }
    }


    [DataContract]
    public class CCashPaymentDetails
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
    public class CCashPaymentReportSummary
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
    public class CCashPaymentReportDetailed
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
