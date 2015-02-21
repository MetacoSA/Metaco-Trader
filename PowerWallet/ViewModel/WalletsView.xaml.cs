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
    /// Interaction logic for WalletsView.xaml
    /// </summary>
    public partial class WalletsView : UserControl
    {
        public WalletsView()
        {
            InitializeComponent();
            ViewModel = App.Locator.Resolve<WalletsViewModel>();
        }

        public WalletsViewModel ViewModel
        {
            get
            {
                return (WalletsViewModel)this.GetValue(ViewModelProperty);
            }
            set
            {
                this.SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
                                                                                                  typeof(WalletsViewModel),
                                                                                                  typeof(WalletsView),
                                                                                                  new PropertyMetadata(ViewModelChanged));

        private static void ViewModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            WalletsView view = obj as WalletsView;
            view.root.DataContext = args.NewValue;
        }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var wallet = tree.SelectedItem as WalletViewModel;
            if (wallet != null)
            {
                wallet.Select();
            }
            var keyset = tree.SelectedItem as KeySetViewModel;
            if (keyset != null)
            {
                keyset.Select();
            }
        }

        private void AddKeySet_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var wallet = (WalletViewModel)e.Parameter;
            var win = (MainWindow)App.Current.MainWindow;
            win.Show(new NewKeySetWindow()
            {
                DataContext = wallet.CreateNewKeysetCommand()
            });
        }
    }
}
