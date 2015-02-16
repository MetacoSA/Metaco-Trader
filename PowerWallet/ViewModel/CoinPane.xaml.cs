using PowerWallet.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerWallet.ViewModel
{
    /// <summary>
    /// Interaction logic for CoinPane.xaml
    /// </summary>
    public partial class CoinPane : UserControl
    {
        public CoinPane()
        {
            InitializeComponent();
            ViewModel = App.Locator.Resolve<CoinsViewModel>();
            grid.SelectionChanged += grid_SelectionChanged;
            coins.CreateCommandBinding(NavigationCommands.Refresh, new Binding("Search"));
        }


        public CoinsViewModel ViewModel
        {
            get
            {
                return coins.DataContext as CoinsViewModel;
            }
            set
            {
                coins.DataContext = value;
            }
        }

        void grid_SelectionChanged(object sender, Xceed.Wpf.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            if (grid.SelectedItems.Count == 1)
            {
                
                var coin = grid.SelectedItems[0] as CoinViewModel;
                if (coin != null)
                {
                    App.Locator.Messenger.Send(new ExposePropertiesMessage(coin));
                }
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            
        }

    }
}
