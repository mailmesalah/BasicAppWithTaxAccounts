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

namespace WpfClientApp.Reports.Accounts.Helper
{
    /// <summary>
    /// </summary>
    public partial class IncomeStatementDetails : Window
    {

        DateTime mEndDate;
        DateTime mstartDate;
        public IncomeStatementDetails(DateTime startDate, DateTime endDate, string ledgerCode)
        {
            InitializeComponent();

            mstartDate = startDate;
            mEndDate = endDate;
            showDataFromDatabase(ledgerCode);
        }

        
        private void showDataFromDatabase(string ledgerCode)
        {
            try
            {
                using (ChannelFactory<ILedger> LedgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    LedgerProxy.Open();
                    ILedger ledgerService = LedgerProxy.CreateChannel();
                    mDataGridGroup.ItemsSource= ledgerService.FindIncomeStatementDetails(mstartDate, mEndDate,ledgerCode);
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

        private void mDataGridGroup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string code = (mDataGridGroup.SelectedItem as CIncomeStatementDetails).LedgerCode;
            string type = (mDataGridGroup.SelectedItem as CIncomeStatementDetails).LedgerType;

            if (code != null && type.EndsWith("Group"))
            {
                showDataFromDatabase(code);
            }
        }
    }
}
