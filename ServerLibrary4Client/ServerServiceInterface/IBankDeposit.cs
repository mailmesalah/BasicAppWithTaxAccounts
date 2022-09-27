using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServerServiceInterface
{    
    [ServiceContract]
    public interface IBankDeposit
    {
        [OperationContract]
        bool CreateBill(CBankDeposit oBankDeposit);
        [OperationContract]
        CBankDeposit ReadBill(string billNo,string financialCode);
        [OperationContract]
        bool UpdateBill(CBankDeposit oBankDeposit);
        [OperationContract]
        bool DeleteBill(string billNo,string financialCode);        
        [OperationContract]
        int ReadNextBillNo(string financialCode);

        [OperationContract]
        List<CBankDepositReportSummary> FindBankDepositsSummary(DateTime startDate, DateTime endDate, string billNo, string bankCode, string bank, string ledgerCode, string ledger, string status, string narration, string financialCode);
        [OperationContract]
        List<CBankDepositReportDetailed> FindBankDepositsDetailed(DateTime startDate, DateTime endDate, string billNo, string bankCode, string bank, string ledgerCode, string ledger, string status, string narration, string financialCode);

    }


    
    [DataContract]
    public class CBankDeposit
    {
        int id;
        string billNo;
        DateTime billDateTime = new DateTime();
        string financialCode;
        string bankCode;
        string bank;
        List<CBankDepositDetails> details= new List<CBankDepositDetails>();
 
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
        public string BankCode
        {
            get { return bankCode; }
            set { bankCode = value; }
        }

        [DataMember]
        public string Bank
        {
            get { return bank; }
            set { bank = value; }
        }

        [DataMember]
        public List<CBankDepositDetails> Details
        {
            get { return details; }
            set { details = value; }
        }
    }


    [DataContract]
    public class CBankDepositDetails
    {
        int serialNo;
        string ledgerCode;
        string ledger;
        string narration;
        decimal amount;
        string status;        

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

        [DataMember]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }        
    }


    [DataContract]
    public class CBankDepositReportSummary
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string financialCode;
        string bankCode;
        string bank;
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
        public string BankCode
        {
            get { return bankCode; }
            set { bankCode = value; }
        }

        [DataMember]
        public string Bank
        {
            get { return bank; }
            set { bank = value; }
        }

        [DataMember]
        public decimal? TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }
    }


    [DataContract]
    public class CBankDepositReportDetailed
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string financialCode;
        string bankCode;
        string bank;
        int? serialNo;
        string ledgerCode;
        string ledger;
        string narration;
        decimal? amount;
        string status;

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
        public string BankCode
        {
            get { return bankCode; }
            set { bankCode = value; }
        }

        [DataMember]
        public string Bank
        {
            get { return bank; }
            set { bank = value; }
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

        [DataMember]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
