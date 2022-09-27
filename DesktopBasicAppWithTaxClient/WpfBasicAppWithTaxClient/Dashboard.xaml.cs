using System.Windows;
using WpfClientApp.Registers;
using WpfClientApp.Registers.Accounts;
using WpfClientApp.Reports;
using WpfClientApp.Reports.Accounts;
using WpfClientApp.Transactions;
using WpfClientApp.Transactions.Accounts;

namespace WpfClientApp
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void CashReceipts_Click(object sender, RoutedEventArgs e)
        {
            CashReceipts cr = new CashReceipts();
            cr.Show();
        }

        private void CashPayments_Click(object sender, RoutedEventArgs e)
        {
            CashPayments cp = new CashPayments();
            cp.Show();
        }

        private void BankDeposits_Click(object sender, RoutedEventArgs e)
        {
            BankDeposits bd = new BankDeposits();
            bd.Show();
        }

        private void BankWithdrawals_Click(object sender, RoutedEventArgs e)
        {
            BankWithdrawals bw = new BankWithdrawals();
            bw.Show();
        }

        private void JournalVouchers_Click(object sender, RoutedEventArgs e)
        {
            JournalVouchers jv = new JournalVouchers();
            jv.Show();
        }

        private void OpeningBalances_Click(object sender, RoutedEventArgs e)
        {
            OpeningBalances ob = new OpeningBalances();
            ob.Show();
        }

        private void ProductRegister_Click(object sender, RoutedEventArgs e)
        {
            ProductRegister pr = new ProductRegister();
            pr.Show();
        }        

        private void PurchaseInterstate_Click(object sender, RoutedEventArgs e)
        {
            PurchaseInterstate pi = new PurchaseInterstate();
            pi.Show();
        }

        private void PurchaseWholesale_Click(object sender, RoutedEventArgs e)
        {
            PurchaseWholesale pw = new PurchaseWholesale();
            pw.Show();
        }

        private void SalesInterstate_Click(object sender, RoutedEventArgs e)
        {
            SalesInterstate si = new SalesInterstate();
            si.Show();
        }

        private void SalesWholesale_Click(object sender, RoutedEventArgs e)
        {
            SalesWholesale sw = new SalesWholesale();
            sw.Show();
        }

        private void SalesLocal_Click(object sender, RoutedEventArgs e)
        {
            SalesLocal sl = new SalesLocal();
            sl.Show();
        }

        private void PurchaseReturnInterstate_Click(object sender, RoutedEventArgs e)
        {
            PurchaseReturnInterstate pri = new PurchaseReturnInterstate();
            pri.Show();
        }

        private void PurchaseReturnWholesale_Click(object sender, RoutedEventArgs e)
        {
            PurchaseReturnWholesale prw = new PurchaseReturnWholesale();
            prw.Show();
        }

        private void SalesReturnInterstate_Click(object sender, RoutedEventArgs e)
        {
            SalesReturnInterstate sri = new SalesReturnInterstate();
            sri.Show();
        }

        private void SalesReturnWholesale_Click(object sender, RoutedEventArgs e)
        {
            SalesReturnWholesale srw = new SalesReturnWholesale();
            srw.Show();
        }

        private void SalesReturnLocal_Click(object sender, RoutedEventArgs e)
        {
            SalesReturnLocal srl = new SalesReturnLocal();
            srl.Show();
        }

        private void StockAddition_Click(object sender, RoutedEventArgs e)
        {
            StockAddition sa = new StockAddition();
            sa.Show();
        }

        private void StockDeletion_Click(object sender, RoutedEventArgs e)
        {
            StockDeletion sd = new StockDeletion();
            sd.Show();
        }

        private void LedgerRegisters_Click(object sender, RoutedEventArgs e)
        {
            LedgerRegisters lr = new LedgerRegisters();
            lr.Show();
        }

        private void SupplierRegisters_Click(object sender, RoutedEventArgs e)
        {
            SupplierRegisters sr = new SupplierRegisters();
            sr.Show();
        }

        private void CustomerRegisters_Click(object sender, RoutedEventArgs e)
        {
            CustomerRegisters cr = new CustomerRegisters();
            cr.Show();
        }

        private void EmployeeRegisters_Click(object sender, RoutedEventArgs e)
        {
            EmployeeRegisters er = new EmployeeRegisters();
            er.Show();
        }

        private void BankRegisters_Click(object sender, RoutedEventArgs e)
        {
            BankRegisters br = new BankRegisters();
            br.Show();
        }

        private void PurchaseInterstateReport_Click(object sender, RoutedEventArgs e)
        {
            PurchaseInterstateReport pir = new PurchaseInterstateReport();
            pir.Show();
        }

        private void PurchaseWholesaleReport_Click(object sender, RoutedEventArgs e)
        {
            PurchaseWholesaleReport pwr = new PurchaseWholesaleReport();
            pwr.Show();
        }

        private void SalesInterstateReport_Click(object sender, RoutedEventArgs e)
        {
            SalesInterstateReport sir = new SalesInterstateReport();
            sir.Show();
        }

        private void SalesWholesaleReport_Click(object sender, RoutedEventArgs e)
        {
            SalesWholesaleReport swr = new SalesWholesaleReport();
            swr.Show();
        }

        private void SalesLocalReport_Click(object sender, RoutedEventArgs e)
        {
            SalesLocalReport slr = new SalesLocalReport();
            slr.Show();
        }

        private void PurchaseReturnInterstateReport_Click(object sender, RoutedEventArgs e)
        {
            PurchaseReturnInterstateReport prir = new PurchaseReturnInterstateReport();
            prir.Show();
        }

        private void PurchaseReturnWholesaleReport_Click(object sender, RoutedEventArgs e)
        {
            PurchaseReturnWholesaleReport prwr = new PurchaseReturnWholesaleReport();
            prwr.Show();
        }

        private void SalesReturnInterstateReport_Click(object sender, RoutedEventArgs e)
        {
            SalesReturnInterstateReport srir = new SalesReturnInterstateReport();
            srir.Show();
        }

        private void SalesReturnWholesaleReport_Click(object sender, RoutedEventArgs e)
        {
            SalesReturnWholesaleReport srwr = new SalesReturnWholesaleReport();
            srwr.Show();
        }

        private void SalesReturnLocalReport_Click(object sender, RoutedEventArgs e)
        {
            SalesReturnLocalReport srlr = new SalesReturnLocalReport();
            srlr.Show();
        }

        private void StockAdditionReport_Click(object sender, RoutedEventArgs e)
        {
            StockAdditionReport sar = new StockAdditionReport();
            sar.Show();
        }

        private void StockDeletionReport_Click(object sender, RoutedEventArgs e)
        {
            StockDeletionReport sdr = new StockDeletionReport();
            sdr.Show();
        }

        private void Income_Click(object sender, RoutedEventArgs e)
        {
            IncomeStatement ist = new IncomeStatement();
            ist.Show();
        }

        private void BalanceSheet_Click(object sender, RoutedEventArgs e)
        {
            BalanceSheet bs = new BalanceSheet();
            bs.Show();
        }

        private void LedgerTransaction_Click(object sender, RoutedEventArgs e)
        {
            LedgerTransaction lt = new LedgerTransaction();
            lt.Show();
        }

        private void BankDepositReport_Click(object sender, RoutedEventArgs e)
        {
            BankDepositReport bdr = new BankDepositReport();
            bdr.Show();
        }

        private void CashReceiptReport_Click(object sender, RoutedEventArgs e)
        {
            CashReceiptReport crr = new CashReceiptReport();
            crr.Show();
        }

        private void CashPaymentReport_Click(object sender, RoutedEventArgs e)
        {
            CashPaymentReport cpr = new CashPaymentReport();
            cpr.Show();
        }

        private void BankWithdrawalReport_Click(object sender, RoutedEventArgs e)
        {
            BankWithdrawalReport bwr = new BankWithdrawalReport();
            bwr.Show();
        }

        private void JournalVoucherReport_Click(object sender, RoutedEventArgs e)
        {
            JournalVoucherReport jvr = new JournalVoucherReport();
            jvr.Show();
        }

        private void OpeningBalanceReport_Click(object sender, RoutedEventArgs e)
        {
            OpeningBalanceReport obr = new OpeningBalanceReport();
            obr.Show();
        }

        private void LedgerReport_Click(object sender, RoutedEventArgs e)
        {
            LedgerReport lr = new LedgerReport();
            lr.Show();
        }
    }
}
