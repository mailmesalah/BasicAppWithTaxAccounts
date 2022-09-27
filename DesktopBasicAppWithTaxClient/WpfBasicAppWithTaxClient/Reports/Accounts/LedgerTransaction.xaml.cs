using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WpfClientApp.General;
using WpfClientApp.Transactions;
using WpfClientApp.Transactions.Accounts;

namespace WpfClientApp.Reports.Accounts
{
    /// <summary>
    /// </summary>
    public partial class LedgerTransaction : Window
    {

        public LedgerTransaction()
        {
            InitializeComponent();

            //Methods
            loadFinancialCodes();
            loadBillTypes();
            loadLedgers();
            loadGroupLedgers();            
        }

        //Member methods
        private void loadGroupLedgers()
        {
            try
            {
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();

                    mComboGroup.ItemsSource = ledgerService.ReadAllGroupLedgers();
                    mComboGroup.DisplayMemberPath = "Ledger";
                    mComboGroup.SelectedValuePath = "LedgerCode";
                }
            }
            catch
            {

            }
        }

        private void loadLedgers()
        {
            try
            {
                string gCode = "";
                if (mComboGroup.SelectedItem != null)
                {
                    gCode = mComboGroup.SelectedValue.ToString();
                }

                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();
                       
                    mComboLedger.ItemsSource = ledgerService.ReadAllLedgersOfGroup(gCode);
                    mComboLedger.DisplayMemberPath = "Ledger";
                    mComboLedger.SelectedValuePath = "LedgerCode";
                }                
            }
            catch
            {
             
            }
        }

        private void loadBillTypes()
        {
            try
            {
             
                using (ChannelFactory<IBillNo> billNoProxy = new ChannelFactory<ServerServiceInterface.IBillNo>("BillNoEndpoint"))
                {
                    billNoProxy.Open();
                    IBillNo billNoService = billNoProxy.CreateChannel();

                    mComboBillType.ItemsSource = billNoService.ReadBillTypes();
                    mComboBillType.DisplayMemberPath = "Value";
                    mComboBillType.SelectedValuePath = "Key";
                }
            }
            catch
            {

            }            
        }

        private void loadFinancialCodes()
        {
            try
            {
                using (ChannelFactory<IBillNo> billNoProxy = new ChannelFactory<ServerServiceInterface.IBillNo>("BillNoEndpoint"))
                {
                    billNoProxy.Open();
                    IBillNo billNoService = billNoProxy.CreateChannel();

                    List<String> fcodes = billNoService.ReadAllFinancialCodes();
                    mComboFinancialYear.ItemsSource = fcodes;                    
                }

                mComboFinancialYear.Text = CommonMethods.getFinancialCode(DateTime.Now);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }
        
        private void showDataFromDatabase()
        {
            try
            {
                string billNo=mTextBoxBillNo.Text.Trim();
                string billType="";
                if (mComboBillType.SelectedItem != null)
                {
                    billType = mComboBillType.SelectedValue.ToString();
                }
                string ledgerCode="";
                if (mComboLedger.SelectedItem != null && (mComboLedger.SelectedItem as CLedger).Ledger.Equals(mComboLedger.Text))
                {
                    ledgerCode = mComboLedger.SelectedValue.ToString();
                }
                string ledger=mComboLedger.Text.Trim();
                string narration = mTextBoxNarration.Text.Trim();
                string aGroupCode="";
                if (mComboGroup.SelectedItem != null && (mComboGroup.SelectedItem as CLedger).LedgerType.Equals("AGroup"))
                {
                    aGroupCode = mComboGroup.SelectedValue.ToString();
                }
                string bGroupCode="";
                if (mComboGroup.SelectedItem != null && (mComboGroup.SelectedItem as CLedger).LedgerType.Equals("BGroup"))
                {
                    bGroupCode = mComboGroup.SelectedValue.ToString();
                }
                string cGroupCode="";
                if (mComboGroup.SelectedItem != null && (mComboGroup.SelectedItem as CLedger).LedgerType.Equals("CGroup"))
                {
                    cGroupCode = mComboGroup.SelectedValue.ToString();
                }
                string refBillNo=mTextBoxRefBillNo.Text.Trim();
                string financialCode="";
                if (mComboFinancialYear.SelectedItem != null)
                {
                    financialCode = mComboFinancialYear.Text.Trim();
                }
                

                using (ChannelFactory<ILedger> LedgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    LedgerProxy.Open();
                    ILedger ledgerService = LedgerProxy.CreateChannel();
                    string fCode = mComboFinancialYear.Text.ToString();
                    mDataGrid.ItemsSource = ledgerService.FindLedgerTransactions(mDTPStartDate.SelectedDate.Value,mDTPEndDate.SelectedDate.Value, billNo, billType, ledgerCode, ledger, narration, aGroupCode, bGroupCode, cGroupCode, refBillNo, financialCode);                    
                }
            }
            catch
            {
                
            }
        }
     
        

        //Events
        private void mButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        
        
        private void mTextBoxBillNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mTextBoxRefBillNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mTextBoxNarration_TextChanged(object sender, TextChangedEventArgs e)
        {
            showDataFromDatabase();            
        }

        private void mDTPStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            showDataFromDatabase();            
        }

        private void mDTPEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            showDataFromDatabase();
        }        

        private void mComboLedger_SelectionChanged(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mDataGrid.SelectedItem != null)
            {
                CLedgerReport clt = mDataGrid.SelectedItem as CLedgerReport;

                switch (clt.BillType)
                {
                    case "Cash Receipts":
                        CashReceipts cr = new CashReceipts(clt.BillNo, (DateTime)clt.BillDate);
                        cr.Show();                       
                        break;
                    case "Cash Payments":
                        CashPayments cp = new CashPayments(clt.BillNo, (DateTime)clt.BillDate);
                        cp.Show();
                        break;
                    case "Bank Withdrawals":
                        BankWithdrawals bw = new BankWithdrawals(clt.BillNo, (DateTime)clt.BillDate);
                        bw.Show();
                        break;
                    case "Bank Deposits":
                        BankDeposits bd = new BankDeposits(clt.BillNo, (DateTime)clt.BillDate);
                        bd.Show();
                        break;
                    case "Reconciliation":
                        break;                    
                    case "Journal Voucher":
                        JournalVouchers jv = new JournalVouchers(clt.BillNo, (DateTime)clt.BillDate);
                        jv.Show();
                        break;
                    case "Opening Balance":
                        OpeningBalances ob = new OpeningBalances(clt.BillNo, (DateTime)clt.BillDate);
                        ob.Show();
                        break;

                    case "Purchase Interstate":
                        PurchaseInterstate pi = new PurchaseInterstate(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        pi.Show();
                        break;
                    case "Purchase Wholesale":
                        PurchaseWholesale pw = new PurchaseWholesale(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        pw.Show();
                        break;
                    case "Sales Interstate":
                        SalesInterstate si = new SalesInterstate(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        si.Show();
                        break;
                    case "Sales Wholsale":
                        SalesWholesale sw = new SalesWholesale(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        sw.Show();
                        break;
                    case "Sales Retail":
                        SalesLocal sl = new SalesLocal(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        sl.Show();
                        break;
                    case "Purchase Interstate Return":
                        PurchaseInterstate pir = new PurchaseInterstate(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        pir.Show();
                        break;
                    case "Purchase Wholesale Return":
                        PurchaseWholesale pwr = new PurchaseWholesale(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        pwr.Show();
                        break;
                    case "Sales Interstate Return":
                        SalesInterstate sir = new SalesInterstate(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        sir.Show();
                        break;
                    case "Sales Wholsale Return":
                        SalesWholesale swr = new SalesWholesale(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        swr.Show();
                        break;
                    case "Sales Retail Return":
                        SalesLocal slr = new SalesLocal(clt.RefBillNo, (DateTime)clt.RefBillDate);
                        slr.Show();
                        break;

                    default:
                        break;
                }
            }
        }

        private void mComboBillType_SelectionChanged(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mComboGroup_SelectionChanged(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mComboFinancialYear_SelectionChanged(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }
    }
}
