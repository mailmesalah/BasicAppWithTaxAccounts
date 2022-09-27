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

namespace WpfClientApp.Reports.Accounts
{
    /// <summary>
    /// </summary>
    public partial class TrialBalance : Window
    {
        private List<CTrialBalance> mDataContents = new List<CTrialBalance>();                
        private DataGrid mDataGridCGroup = new DataGrid();

        public List<CTrialBalance> BGroupData
        {
            get
            {
                string fCode = mComboFinancialYear.Text.ToString();
                return getTrialBalanceOfBGroup(fCode, CommonMethods.getFinancialStartDate(fCode), CommonMethods.getFinancialEndDate(fCode));
            } 
           
        }

        public List<CTrialBalance> CGroupData
        {
            get
            {
                string fCode = mComboFinancialYear.Text.ToString();
                return getTrialBalanceOfCGroup(fCode, CommonMethods.getFinancialStartDate(fCode), CommonMethods.getFinancialEndDate(fCode));
            }

        }

        public TrialBalance()
        {
            InitializeComponent();

            //methods
            loadFinancialCodes();
            mDataGridBGroup.ItemsSource = mDataContents;            
        }

        //Member methods
        
        private List<CTrialBalance> getTrialBalanceOfBGroup(string financialCode,DateTime startDate,DateTime endDate)
        {
            List<CTrialBalance> mData = new List<CTrialBalance>();        
            try
            {
                string gCode = "";                
                if (mDataGridBGroup.SelectedItem != null)
                {                    
                    gCode = (mDataGridBGroup.SelectedItem as CTrialBalance).LedgerCode;                                        
                }

                if (gCode == "")
                {
                    return mData;
                }

                using (ChannelFactory<ILedger> LedgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    LedgerProxy.Open();
                    ILedger ledgerService = LedgerProxy.CreateChannel();
                    string fCode = mComboFinancialYear.Text.ToString();
                    
                    mData = ledgerService.FindTrialBalanceOfBGroup(gCode, fCode, CommonMethods.getFinancialStartDate(fCode), CommonMethods.getFinancialEndDate(fCode));              
                }
            }
            catch
            {

            }
            return mData;
        }

        private List<CTrialBalance> getTrialBalanceOfCGroup(string financialCode, DateTime startDate, DateTime endDate)
        {
            List<CTrialBalance> mData = new List<CTrialBalance>();
            try
            {
                string gCode = "";
                string gType = "";
                if (mDataGridCGroup.SelectedItem != null)
                {
                    gCode = (mDataGridCGroup.SelectedItem as CTrialBalance).LedgerCode;
                    gType= (mDataGridCGroup.SelectedItem as CTrialBalance).LedgerType;
                }

                if (gCode == "" || gType=="CAccount")
                {
                    return mData;
                }

                using (ChannelFactory<ILedger> LedgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    LedgerProxy.Open();
                    ILedger ledgerService = LedgerProxy.CreateChannel();
                    string fCode = mComboFinancialYear.Text.ToString();

                    mData = ledgerService.FindTrialBalanceOfCGroup(gCode, fCode, CommonMethods.getFinancialStartDate(fCode), CommonMethods.getFinancialEndDate(fCode));
                }
            }
            catch
            {

            }
            return mData;
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
                using (ChannelFactory<ILedger> LedgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    LedgerProxy.Open();
                    ILedger ledgerService = LedgerProxy.CreateChannel();
                    string fCode = mComboFinancialYear.Text.ToString();
                    mDataContents = ledgerService.FindTrialBalance(fCode,CommonMethods.getFinancialStartDate(fCode),CommonMethods.getFinancialEndDate(fCode));
                    mDataGridBGroup.Items.Refresh();
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
        
        private void mDataGridBGroup_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {                
                DataGrid dg = sender as DataGrid;

                if (dg.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                {
                    mDataGridCGroup = new DataGrid();
                    dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;                    
                }
                else
                {
                    mDataGridCGroup = new DataGrid();
                    dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                }
            }
            catch
            {

            }
        }

        private void mDataGridBGroup_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            try
            {
                mDataGridCGroup = e.DetailsElement as DataGrid;
            }
            catch
            {
            }            
        }

        private void mDataGridCGroup_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DataGrid dg = sender as DataGrid;
                if (dg.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                {
                    mDataGridCGroup = new DataGrid();
                    dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                }
                else
                {
                    mDataGridCGroup = new DataGrid();
                    dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                }

                e.Handled = true;
            }
            catch
            {
            }
        }

        private void mComboFinancialYear_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (mComboFinancialYear.SelectedItem != null)
            {
                showDataFromDatabase();
            }
        }
    }
}
