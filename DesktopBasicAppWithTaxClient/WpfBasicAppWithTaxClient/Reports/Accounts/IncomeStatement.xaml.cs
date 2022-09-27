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
using WpfClientApp.Reports.Accounts.Helper;

namespace WpfClientApp.Reports.Accounts
{
    /// <summary>
    /// </summary>
    public partial class IncomeStatement : Window
    {
 
        public IncomeStatement()
        {
            InitializeComponent();

            showDataFromDatabase();
        }

        
        private void showDataFromDatabase()
        {
            try
            {
                using (ChannelFactory<ILedger> LedgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    LedgerProxy.Open();
                    ILedger ledgerService = LedgerProxy.CreateChannel();
                    mDataGridBGroup.ItemsSource= ledgerService.FindIncomeStatement(mDTPStartDate.SelectedDate.Value,mDTPEndDate.SelectedDate.Value);
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

        private void mDTPStartDate_LostFocus(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mDTPEndDate_LostFocus(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mDataGridBGroup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string code = (mDataGridBGroup.SelectedItem as CIncomeStatement).LedgerCode; 
                        
            if (code != "")
            {
                IncomeStatementDetails isd = new IncomeStatementDetails(mDTPStartDate.SelectedDate.Value, mDTPEndDate.SelectedDate.Value, code);
                isd.Show();
            }
        }
    }
}
