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
    /// Interaction logic for CashPayments.xaml
    /// </summary>
    public partial class CashPayments : Window
    {
        
        CCashPayment mCashPayment = new CCashPayment();
        ObservableCollection<CCashPaymentDetails> mGridContent = new ObservableCollection<CCashPaymentDetails>();
        String mCashPaymentID = "";

        //Linking Report to Transaction
        public CashPayments(string billNo, DateTime billDate)
        {
            InitializeComponent();
            loadInitialDetails();

            mTextBoxBillNo.Text = billNo;
            mDTPDate.SelectedDate = billDate;
            showDataFromDatabase();
        }

        public CashPayments()
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
            mCashPaymentID = "";
            mCashPayment = new CCashPayment();
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
                using (ChannelFactory<ICashPayment> CashPaymentProxy = new ChannelFactory<ServerServiceInterface.ICashPayment>("CashPaymentEndpoint"))
                {
                    CashPaymentProxy.Open();
                    ICashPayment CashPaymentservice = CashPaymentProxy.CreateChannel();
                    billNo=CashPaymentservice.ReadNextBillNo(CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)).ToString();                    
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
                mGridContent.Remove(mDataGridContent.SelectedItem as CCashPaymentDetails);
                mGridContent.Insert(index, new CCashPaymentDetails() { SerialNo = serialNo, Ledger = mComboLedgers.Text.ToString(), LedgerCode = mComboLedgers.SelectedValue.ToString(), Narration = mTextBoxNarration.Text.ToString().Trim(), Amount = amount });
            }
            else
            {
                //Add
                CCashPaymentDetails crd = new CCashPaymentDetails() { SerialNo = serialNo, Ledger = mComboLedgers.Text.ToString(), LedgerCode = mComboLedgers.SelectedValue.ToString(), Narration = mTextBoxNarration.Text.ToString().Trim(), Amount = amount };                
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
                CCashPaymentDetails crd=(CCashPaymentDetails)mDataGridContent.Items.GetItemAt(mDataGridContent.SelectedIndex);
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
                mGridContent.Remove(mDataGridContent.SelectedItem as CCashPaymentDetails);

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
                using (ChannelFactory<ICashPayment> CashPaymentProxy = new ChannelFactory<ServerServiceInterface.ICashPayment>("CashPaymentEndpoint"))
                {
                    CashPaymentProxy.Open();
                    ICashPayment CashPaymentservice = CashPaymentProxy.CreateChannel();

                    CCashPayment ccr= CashPaymentservice.ReadBill(mTextBoxBillNo.Text.Trim(), CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));
                    
                    if (ccr != null)
                    {
                        mCashPaymentID = ccr.Id.ToString();
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
              
                using (ChannelFactory<ICashPayment> CashPaymentProxy = new ChannelFactory<ServerServiceInterface.ICashPayment>("CashPaymentEndpoint"))
                {
                    CashPaymentProxy.Open();
                    ICashPayment CashPaymentservice = CashPaymentProxy.CreateChannel();

                    CCashPayment ccr = new CCashPayment();
                    ccr.BillNo = mTextBoxBillNo.Text.Trim();
                    ccr.BillDateTime = mDTPDate.SelectedDate.Value;
                    ccr.FinancialCode = CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);                                    
                    foreach (var item in mGridContent)
                    {
                        ccr.Details.Add(item);
                    }

                    bool success = false;
                    if (mCashPaymentID != "")
                    { 
                        success = CashPaymentservice.UpdateBill(ccr);
                    }
                    else
                    {                    
                        success = CashPaymentservice.CreateBill(ccr);
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
                using (ChannelFactory<ICashPayment> CashPaymentProxy = new ChannelFactory<ServerServiceInterface.ICashPayment>("CashPaymentEndpoint"))
                {
                    CashPaymentProxy.Open();
                    ICashPayment CashPaymentservice = CashPaymentProxy.CreateChannel();
                    
                    bool success= CashPaymentservice.DeleteBill(mTextBoxBillNo.Text.Trim(), CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));

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
