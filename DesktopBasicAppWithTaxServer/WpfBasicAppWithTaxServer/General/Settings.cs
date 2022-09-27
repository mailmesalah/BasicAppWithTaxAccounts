using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfServerApp.General
{
    class Settings
    {
        public static Dictionary<string, string> BillTypes = new Dictionary<string, string>();

        public static void loadBillTypes()
        {
            BillTypes["CR"] = "Cash Receipts";
            BillTypes["CP"] = "Cash Payments";
            BillTypes["BW"] = "Bank Withdrawals";
            BillTypes["BD"] = "Bank Deposits";
            BillTypes["RE"] = "Reconciliation";            
            BillTypes["JV"] = "Journal Voucher";
            BillTypes["OB"] = "Opening Balance";

            BillTypes["PI"] = "Purchase Interstate";
            BillTypes["PW"] = "Purchase Wholesale";
            BillTypes["SI"] = "Sales Interstate";
            BillTypes["SW"] = "Sales Wholesale";
            BillTypes["SL"] = "Sales Retail";
            BillTypes["PIR"] = "Purchase Interstate Return";
            BillTypes["PWR"] = "Purchase Wholesale Return";
            BillTypes["SIR"] = "Sales Interstate Return";
            BillTypes["SWR"] = "Sales Wholsale Return";
            BillTypes["SLR"] = "Sales Retail Return";
        }
    }
}
