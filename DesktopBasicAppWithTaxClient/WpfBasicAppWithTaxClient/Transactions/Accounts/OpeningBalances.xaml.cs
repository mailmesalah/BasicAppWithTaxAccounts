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
    /// Interaction logic for OpeningBalances.xaml
    /// </summary>
    public partial class OpeningBalances : Window
    {
        
        COpeningBalance mOpeningBalance = new COpeningBalance();
        ObservableCollection<COpeningBalanceDetails> mGridContent = new ObservableCollection<COpeningBalanceDetails>();
        String mOpeningBalanceID = "";

        //Linking Report to Transaction
        public OpeningBalances(string billNo, DateTime billDate)
        {
            InitializeComponent();
            loadInitialDetails();

            mTextBoxBillNo.Text = billNo;
            mDTPDate.SelectedDate = billDate;
            showDataFromDatabase();
        }

        public OpeningBalances()
        {
            InitializeComponent();
            loadInitialDetails();
                        
        }

        //Member methods
        private void loadInitialDetails()
        {            
            getLedgers();
            newBill();
        }

        private void newBill()
        {
            mOpeningBalanceID = "";
            mOpeningBalance = new COpeningBalance();
            mTextBoxBillNo.Text = getLastBillNo();
            loadFinancialCodes();
            mDTPDate.SelectedDate = DateTime.Now;
            mComboFinancialYear.Text = CommonMethods.getFinancialCode(DateTime.Now);
            mGridContent.Clear();
            mDataGridContent.ItemsSource = mGridContent;
            clearEditBoxes();
            mComboLedgers.Focus();
        }

        private void clearEditBoxes(){
            mLabelSerialNo.Content = mDataGridContent.Items.Count+1;
            mComboLedgers.Text = "";
            mTextBoxNarration.Text = "";
            mTextBoxDebit.Text = "";
            mTextBoxCredit.Text = "";            
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
                using (ChannelFactory<IOpeningBalance> OpeningBalanceProxy = new ChannelFactory<ServerServiceInterface.IOpeningBalance>("OpeningBalanceEndpoint"))
                {
                    OpeningBalanceProxy.Open();
                    IOpeningBalance OpeningBalanceservice = OpeningBalanceProxy.CreateChannel();
                    billNo=OpeningBalanceservice.ReadNextBillNo(CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)).ToString();                    
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
                    List<CLedger> ledgers = ledgerService.ReadLedgersWithoutCash();
                    mComboLedgers.ItemsSource = ledgers;
                    mComboLedgers.DisplayMemberPath = "Ledger";
                    mComboLedgers.SelectedValuePath = "LedgerCode";
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

            decimal debit = 0;
            try
            {
                
                debit = decimal.Parse(mTextBoxDebit.Text);

                if (debit < 0)
                {
                    mTextBoxDebit.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxDebit.Focus();
                return;
            }

            decimal credit;
            try
            {

                credit = decimal.Parse(mTextBoxCredit.Text);

                if (credit < 0)
                {
                    mTextBoxCredit.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxCredit.Focus();
                return;
            }
            

            int serialNo = int.Parse(mLabelSerialNo.Content.ToString());
            if (serialNo <= mDataGridContent.Items.Count)
            {
                //Edit
                int index = mDataGridContent.SelectedIndex;
                mGridContent.Remove(mDataGridContent.SelectedItem as COpeningBalanceDetails);
                mGridContent.Insert(index, new COpeningBalanceDetails() { SerialNo = serialNo, Ledger = mComboLedgers.Text.ToString(), LedgerCode = mComboLedgers.SelectedValue.ToString(), Narration = mTextBoxNarration.Text.ToString().Trim(), Debit = debit, Credit=credit });
            }
            else
            {
                //Add
                COpeningBalanceDetails crd = new COpeningBalanceDetails() { SerialNo = serialNo, Ledger = mComboLedgers.Text.ToString(), LedgerCode = mComboLedgers.SelectedValue.ToString(), Narration = mTextBoxNarration.Text.ToString().Trim(), Debit = debit, Credit=credit };                
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
                COpeningBalanceDetails crd=(COpeningBalanceDetails)mDataGridContent.Items.GetItemAt(mDataGridContent.SelectedIndex);
                mLabelSerialNo.Content = crd.SerialNo;
                mComboLedgers.Text = crd.Ledger;
                mTextBoxNarration.Text = crd.Narration;
                mTextBoxDebit.Text = crd.Debit.ToString();
                mTextBoxCredit.Text = crd.Credit.ToString();
            }
        }

        private void removeFromGrid()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                mGridContent.Remove(mDataGridContent.SelectedItem as COpeningBalanceDetails);

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
                using (ChannelFactory<IOpeningBalance> OpeningBalanceProxy = new ChannelFactory<ServerServiceInterface.IOpeningBalance>("OpeningBalanceEndpoint"))
                {
                    OpeningBalanceProxy.Open();
                    IOpeningBalance OpeningBalanceservice = OpeningBalanceProxy.CreateChannel();

                    COpeningBalance ccr= OpeningBalanceservice.ReadBill(mTextBoxBillNo.Text.Trim(), CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));
                    
                    if (ccr != null)
                    {
                        mOpeningBalanceID = ccr.Id.ToString();
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
                if (mDataGridContent.Items.Count==0)
                {
                    mComboLedgers.Focus();
                    return;
                }

                using (ChannelFactory<IOpeningBalance> OpeningBalanceProxy = new ChannelFactory<ServerServiceInterface.IOpeningBalance>("OpeningBalanceEndpoint"))
                {
                    OpeningBalanceProxy.Open();
                    IOpeningBalance OpeningBalanceservice = OpeningBalanceProxy.CreateChannel();

                    COpeningBalance ccr = new COpeningBalance();
                    ccr.BillNo = mTextBoxBillNo.Text.Trim();
                    ccr.BillDateTime = mDTPDate.SelectedDate.Value;
                    ccr.FinancialCode = CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);                                    
                    foreach (var item in mGridContent)
                    {
                        ccr.Details.Add(item);
                    }

                    bool success = false;
                    if (mOpeningBalanceID != "")
                    { 
                        success = OpeningBalanceservice.UpdateBill(ccr);
                    }
                    else
                    {                    
                        success = OpeningBalanceservice.CreateBill(ccr);
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
                using (ChannelFactory<IOpeningBalance> OpeningBalanceProxy = new ChannelFactory<ServerServiceInterface.IOpeningBalance>("OpeningBalanceEndpoint"))
                {
                    OpeningBalanceProxy.Open();
                    IOpeningBalance OpeningBalanceservice = OpeningBalanceProxy.CreateChannel();
                    
                    bool success= OpeningBalanceservice.DeleteBill(mTextBoxBillNo.Text.Trim(), CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));

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

        private void mTextBoxDebit_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            decimal debit;
            try
            {
                debit=decimal.Parse(mTextBoxDebit.Text);
                if (debit > 0)
                {
                    mTextBoxCredit.IsEnabled = false;
                    mTextBoxCredit.Text = "0";
                }
                else
                {
                    mTextBoxCredit.IsEnabled = true;
                }
            }
            catch
            {
                mTextBoxCredit.IsEnabled = true;
            }
            
        }

        private void mTextBoxCredit_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            decimal credit;
            try
            {
                credit = decimal.Parse(mTextBoxCredit.Text);
                if (credit > 0)
                {
                    mTextBoxDebit.IsEnabled = false;
                    mTextBoxDebit.Text = "0";
                }
                else
                {
                    mTextBoxDebit.IsEnabled = true;
                }
            }
            catch
            {
                mTextBoxDebit.IsEnabled = true;
            }
        }
    }
}
