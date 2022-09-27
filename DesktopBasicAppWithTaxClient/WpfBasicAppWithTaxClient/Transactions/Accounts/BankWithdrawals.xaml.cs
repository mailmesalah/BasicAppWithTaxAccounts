using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using WpfClientApp.General;

namespace WpfClientApp.Transactions.Accounts
{
    /// <summary>
    /// Interaction logic for BankWithdrawals.xaml
    /// </summary>
    public partial class BankWithdrawals : Window
    {
        
        CBankWithdrawal mBankWithdrawal = new CBankWithdrawal();
        ObservableCollection<CBankWithdrawalDetails> mGridContent = new ObservableCollection<CBankWithdrawalDetails>();
        String mBankWithdrawalID = "";

        //Linking Report to Transaction
        public BankWithdrawals(string billNo, DateTime billDate)
        {
            InitializeComponent();
            loadInitialDetails();

            mTextBoxBillNo.Text = billNo;
            mDTPDate.SelectedDate = billDate;
            showDataFromDatabase();
        }

        public BankWithdrawals()
        {
            InitializeComponent();
            loadInitialDetails();
                        
        }

        //Member methods
        private void loadInitialDetails()
        {            
            getLedgers();
            getBankLedgers();
            newBill();
        }

        private void newBill()
        {
            mBankWithdrawalID = "";
            mBankWithdrawal = new CBankWithdrawal();
            mTextBoxBillNo.Text = getLastBillNo();
            loadFinancialCodes();
            mDTPDate.SelectedDate = DateTime.Now;
            mComboFinancialYear.Text = CommonMethods.getFinancialCode(DateTime.Now);
            mGridContent.Clear();
            mDataGridContent.ItemsSource = mGridContent;
            clearEditBoxes();
            mComboBankLedgers.Focus();
        }

        private void clearEditBoxes(){
            mLabelSerialNo.Content = mDataGridContent.Items.Count+1;
            mComboLedgers.Text = "";
            mTextBoxNarration.Text = "";
            mTextBoxAmount.Text = "";            
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
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        private string getLastBillNo()
        {
            string billNo = "";
            try {
                using (ChannelFactory<IBankWithdrawal> BankWithdrawalProxy = new ChannelFactory<ServerServiceInterface.IBankWithdrawal>("BankWithdrawalEndpoint"))
                {
                    BankWithdrawalProxy.Open();
                    IBankWithdrawal BankWithdrawalservice = BankWithdrawalProxy.CreateChannel();
                    billNo=BankWithdrawalservice.ReadNextBillNo(CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)).ToString();                    
                }
            }
            catch
            {

            }
            return billNo;
        }

        private void getLedgers()
        {
            try {
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();                    
                    List<CLedger> ledgers = ledgerService.ReadAllLedgers();
                    mComboLedgers.ItemsSource = ledgers;
                    mComboLedgers.DisplayMemberPath = "Ledger";
                    mComboLedgers.SelectedValuePath = "LedgerCode";
                }
            }
            catch
            {

            }        
        }

        private void getBankLedgers()
        {
            try
            {
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();
                    List<CLedger> ledgers = ledgerService.ReadBankLedgers();
                    mComboBankLedgers.ItemsSource = ledgers;
                    mComboBankLedgers.DisplayMemberPath = "Ledger";
                    mComboBankLedgers.SelectedValuePath = "LedgerCode";
                }
            }
            catch
            {

            }
        }

        private void addDataToGrid()
        {
            if (mComboLedgers.SelectedIndex == -1)
            {
                mComboLedgers.Focus();
                return;
            }

            decimal amount = 0;
            try
            {
                
                amount = decimal.Parse(mTextBoxAmount.Text);

                if (amount <= 0)
                {
                    mTextBoxAmount.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxAmount.Focus();
                return;
            }
            
            int serialNo = int.Parse(mLabelSerialNo.Content.ToString());
            if (serialNo <= mDataGridContent.Items.Count)
            {
                //Edit
                int index = mDataGridContent.SelectedIndex;
                mGridContent.Remove(mDataGridContent.SelectedItem as CBankWithdrawalDetails);
                mGridContent.Insert(index, new CBankWithdrawalDetails() { SerialNo = serialNo, Ledger = mComboLedgers.Text.ToString(), LedgerCode = mComboLedgers.SelectedValue.ToString(), Narration = mTextBoxNarration.Text.ToString().Trim(), Amount = amount, Status="Pending" });
            }
            else
            {
                //Add
                CBankWithdrawalDetails crd = new CBankWithdrawalDetails() { SerialNo = serialNo, Ledger = mComboLedgers.Text.ToString(), LedgerCode = mComboLedgers.SelectedValue.ToString(), Narration = mTextBoxNarration.Text.ToString().Trim(), Amount = amount, Status="Pending" };                
                mGridContent.Add(crd);
            }
            
            clearEditBoxes();
            mDataGridContent.ScrollIntoView(mDataGridContent.Items.GetItemAt(mDataGridContent.Items.Count-1));
            mComboLedgers.Focus();
        }

        private void selectDataToEditBoxes()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                CBankWithdrawalDetails crd=(CBankWithdrawalDetails)mDataGridContent.Items.GetItemAt(mDataGridContent.SelectedIndex);
                mLabelSerialNo.Content = crd.SerialNo;
                mComboLedgers.Text = crd.Ledger;
                mTextBoxNarration.Text = crd.Narration;
                mTextBoxAmount.Text = crd.Amount.ToString();
            }
        }

        private void removeFromGrid()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                mGridContent.Remove(mDataGridContent.SelectedItem as CBankWithdrawalDetails);

                //Reseting the Serial Nos
                for(int i = 0; i < mGridContent.Count; i++)
                {
                    mGridContent.ElementAt(i).SerialNo = i + 1;                    
                }
                mDataGridContent.Items.Refresh();
                clearEditBoxes();
            }
            
        }

        private void showDataFromDatabase()
        {
            try
            {
                using (ChannelFactory<IBankWithdrawal> BankWithdrawalProxy = new ChannelFactory<ServerServiceInterface.IBankWithdrawal>("BankWithdrawalEndpoint"))
                {
                    BankWithdrawalProxy.Open();
                    IBankWithdrawal BankWithdrawalservice = BankWithdrawalProxy.CreateChannel();

                    CBankWithdrawal ccr= BankWithdrawalservice.ReadBill(mTextBoxBillNo.Text.Trim(), CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));
                    
                    if (ccr != null)
                    {
                        mBankWithdrawalID = ccr.Id.ToString();
                        mComboBankLedgers.Text = ccr.Bank;
                        mDTPDate.SelectedDate = ccr.BillDateTime;
                        mGridContent.Clear();
                        foreach (var item in ccr.Details)
                        {
                            mGridContent.Add(item);
                        }
                        mDataGridContent.Items.Refresh();
                    }
                    
                }
            }
            catch
            {

            }
        }
     
        private void saveDataToDatabase()
        {
            try
            {
                if (mComboBankLedgers.SelectedItem == null)
                {
                    mComboBankLedgers.Focus();
                    return;
                }

                if (mDataGridContent.Items.Count==0)
                {
                    mComboLedgers.Focus();
                    return;
                }
              
                using (ChannelFactory<IBankWithdrawal> BankWithdrawalProxy = new ChannelFactory<ServerServiceInterface.IBankWithdrawal>("BankWithdrawalEndpoint"))
                {
                    BankWithdrawalProxy.Open();
                    IBankWithdrawal BankWithdrawalservice = BankWithdrawalProxy.CreateChannel();
                    
                    CBankWithdrawal ccr = new CBankWithdrawal();
                    ccr.BillNo = mTextBoxBillNo.Text.Trim();
                    ccr.BillDateTime = mDTPDate.SelectedDate.Value;
                    ccr.Bank = mComboBankLedgers.Text.ToString();
                    ccr.BankCode = mComboBankLedgers.SelectedValue.ToString();
                    ccr.FinancialCode = CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);                                    
                    foreach (var item in mGridContent)
                    {
                        ccr.Details.Add(item);
                    }

                    bool success = false;
                    if (mBankWithdrawalID != "")
                    { 
                        success = BankWithdrawalservice.UpdateBill(ccr);
                    }
                    else
                    {                    
                        success = BankWithdrawalservice.CreateBill(ccr);
                    }

                    if (success)
                    {
                        newBill();
                    }                    
                }
            }
            catch(Exception e)
            {
                
            }
        }

        private void deleteDataFromDatabase()
        {
            try
            {
                using (ChannelFactory<IBankWithdrawal> BankWithdrawalProxy = new ChannelFactory<ServerServiceInterface.IBankWithdrawal>("BankWithdrawalEndpoint"))
                {
                    BankWithdrawalProxy.Open();
                    IBankWithdrawal BankWithdrawalservice = BankWithdrawalProxy.CreateChannel();
                    
                    bool success= BankWithdrawalservice.DeleteBill(mTextBoxBillNo.Text.Trim(), CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));

                    if (success)
                    {
                        newBill();
                    }                   
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
       
        private void mButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            addDataToGrid();
        }

        private void mDTPDate_LostFocus(object sender, RoutedEventArgs e)
        {
            mComboFinancialYear.Text= CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);
        }

        private void mButtonNew_Click(object sender, RoutedEventArgs e)
        {
            newBill();
        }

        private void mButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteDataFromDatabase();
        }

        private void mComboFinancialYear_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mComboFinancialYear.SelectedIndex > -1)
                {
                    int year = int.Parse(mComboFinancialYear.SelectedItem.ToString());
                    if (!mComboFinancialYear.SelectedItem.ToString().Equals(CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)))
                    {
                        mDTPDate.SelectedDate = new DateTime(year, CommonMethods.FinancialStartDate.Month, CommonMethods.FinancialStartDate.Day);
                    }
                }
            }
            catch
            {

            }
        }        

        private void mDataGridContent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectDataToEditBoxes();
        }

        private void mButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            removeFromGrid();
        }        

        private void mTextBoxBillNo_LostFocus(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mButtonSave_Click(object sender, RoutedEventArgs e)
        {
            saveDataToDatabase();
        }
    }
}
