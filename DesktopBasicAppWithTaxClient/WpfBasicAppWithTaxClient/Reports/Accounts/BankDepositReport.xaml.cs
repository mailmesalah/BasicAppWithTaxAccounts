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
    public partial class BankDepositReport : Window
    {

        //For managing summary and detailed reports
        private const int SUMMARY = 0;
        private const int DETAILED = 1;
        private static int Report = SUMMARY;

        public BankDepositReport()
        {
            InitializeComponent();

            //Methods
            loadFinancialCodes();
            loadStatus();
            loadLedgers();
            loadBankLedgers();            
        }

        //Member methods
        private void loadBankLedgers()
        {
            try
            {
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();

                    mComboBank.ItemsSource = ledgerService.ReadAllBankRegisters();
                    mComboBank.DisplayMemberPath = "Ledger";
                    mComboBank.SelectedValuePath = "LedgerCode";
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
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();
                       
                    mComboLedger.ItemsSource = ledgerService.ReadAllLedgers();
                    mComboLedger.DisplayMemberPath = "Ledger";
                    mComboLedger.SelectedValuePath = "LedgerCode";
                }                
            }
            catch
            {
             
            }
        }

        private void loadStatus()
        {
            try
            {
                mComboStatus.Items.Clear();
                mComboStatus.Items.Add("Pending");
                mComboStatus.Items.Add("Accepted");
                mComboStatus.Items.Add("Declined");

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
                string status="";
                if (mComboStatus.SelectedItem != null)
                {
                    status = mComboStatus.SelectedValue.ToString();
                }
                string ledgerCode="";
                if (mComboLedger.SelectedItem != null && (mComboLedger.SelectedItem as CLedger).Ledger.Equals(mComboLedger.Text))
                {
                    ledgerCode = mComboLedger.SelectedValue.ToString();
                }
                string ledger=mComboLedger.Text.Trim();
                string narration = mTextBoxNarration.Text.Trim();

                string bankCode = "";
                if (mComboBank.SelectedItem != null && (mComboBank.SelectedItem as CLedgerRegister).Ledger.Equals(mComboBank.Text))
                {
                    bankCode = mComboBank.SelectedValue.ToString();
                }
                string bank = mComboBank.Text.Trim();

                string financialCode="";
                if (mComboFinancialYear.SelectedItem != null)
                {
                    financialCode = mComboFinancialYear.Text.Trim();
                }
                

                using (ChannelFactory<IBankDeposit> bankDepositProxy = new ChannelFactory<ServerServiceInterface.IBankDeposit>("BankDepositEndpoint"))
                {
                    bankDepositProxy.Open();
                    IBankDeposit bankDepositService = bankDepositProxy.CreateChannel();
                    if (Report == SUMMARY)
                    {
                        //Restructuring Colums                        
                        mDataGrid.Columns.Clear();

                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Date", Binding = new System.Windows.Data.Binding("BillDateTime"), Width = 120, IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Bill No", Binding = new System.Windows.Data.Binding("BillNo"), Width = 100, IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Bank", Binding = new System.Windows.Data.Binding("Bank"), Width= new DataGridLength(350,DataGridLengthUnitType.Star), IsReadOnly = true });
                        System.Windows.Data.Binding b = new System.Windows.Data.Binding("TotalAmount");b.StringFormat = "N2";                        
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Amount", Binding = b, Width = 120, CellStyle = (Style)mDataGrid.Resources["ColRightAlign"], IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Financial Year", Binding = new System.Windows.Data.Binding("FinancialCode"), Width = 120, IsReadOnly = true });

                        mDataGrid.ItemsSource = bankDepositService.FindBankDepositsSummary(mDTPStartDate.SelectedDate.Value,mDTPEndDate.SelectedDate.Value,billNo,bankCode,bank,ledgerCode,ledger,status,narration,financialCode);
                    }
                    else
                    {
                        //Restructuring Colums                        
                        mDataGrid.Columns.Clear();
                        
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Date", Binding = new System.Windows.Data.Binding("BillDateTime"), Width = 120, IsReadOnly=true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Bill No", Binding = new System.Windows.Data.Binding("BillNo"), Width = 100, IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Bank", Binding = new System.Windows.Data.Binding("Bank"), Width = 200, IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Sl No", Binding = new System.Windows.Data.Binding("SerialNo"), Width = 80, IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Ledger", Binding = new System.Windows.Data.Binding("Ledger"), Width = 200, IsReadOnly = true });
                        System.Windows.Data.Binding b = new System.Windows.Data.Binding("Amount"); b.StringFormat = "N2";
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Amount", Binding = b, Width = 120, CellStyle = (Style)mDataGrid.Resources["ColRightAlign"], IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Narration", Binding = new System.Windows.Data.Binding("Narration"), Width = 120, IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Status", Binding = new System.Windows.Data.Binding("Status"), Width = 100, IsReadOnly = true });
                        mDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Financial Year", Binding = new System.Windows.Data.Binding("FinancialCode"), Width = 100, IsReadOnly = true });

                        mDataGrid.ItemsSource = bankDepositService.FindBankDepositsDetailed(mDTPStartDate.SelectedDate.Value, mDTPEndDate.SelectedDate.Value, billNo, bankCode, bank, ledgerCode, ledger, status, narration, financialCode);
                    }
                    
                }
            }
            catch(Exception e)
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
                if (Report == SUMMARY)
                {
                    CBankDepositReportSummary cbd = mDataGrid.SelectedItem as CBankDepositReportSummary;
                    if (cbd.BillNo != "" && cbd.BillDateTime != null)
                    {
                        BankDeposits bd = new BankDeposits(cbd.BillNo, (DateTime)cbd.BillDateTime);
                        bd.Show();
                    }
                }
                else
                {
                    CBankDepositReportDetailed cbd = mDataGrid.SelectedItem as CBankDepositReportDetailed;
                    if (cbd.BillNo != "" && cbd.BillDateTime != null)
                    {
                        BankDeposits bd = new BankDeposits(cbd.BillNo, (DateTime)cbd.BillDateTime);
                        bd.Show();
                    }
                }
            }
        }

        private void mButtonShowSummary_Click(object sender, RoutedEventArgs e)
        {
            Report = SUMMARY;
            showDataFromDatabase();
        }

        private void mButtonShowDetailed_Click(object sender, RoutedEventArgs e)
        {
            Report = DETAILED;
            showDataFromDatabase();
        }

        private void mComboStatus_SelectionChanged(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mComboBank_SelectionChanged(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mComboFinancialYear_SelectionChanged(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }
    }
}
