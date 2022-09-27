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
    public partial class BalanceSheet : Window
    {
 
        public BalanceSheet()
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
                    mDataGridBGroup.ItemsSource= ledgerService.FindBalanceSheet(mDTPDate.SelectedDate.Value);
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

        private void mDTPDate_LostFocus(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mDataGridBGroup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string code = "";
            if (mDataGridBGroup.CurrentCell.Column.DisplayIndex <= 1)
            {
                //Asset Part
                code=(mDataGridBGroup.SelectedItem as CBalanceSheet).AssetCode;
            }
            else
            {
                //Liabilities part
                code = (mDataGridBGroup.SelectedItem as CBalanceSheet).LiabilityCode;
            }

            if (code != "")
            {
                BalanceSheetDetails bsd = new BalanceSheetDetails(mDTPDate.SelectedDate.Value, code);
                bsd.Show();
            }
        }
    }
}
