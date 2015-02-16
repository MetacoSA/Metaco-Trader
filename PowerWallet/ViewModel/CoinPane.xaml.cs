using PowerWallet.Messages;
using System;
using System.Collections.Generic;
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
            coins.DataContext = App.Locator.Resolve<CoinsViewModel>();
            grid.SelectionChanged += grid_SelectionChanged;
        }


        public CoinsViewModel ViewModel
        {
            get
            {
                return coins.DataContext as CoinsViewModel;
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
