using GalaSoft.MvvmLight.Messaging;
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
    /// Interaction logic for AddressesView.xaml
    /// </summary>
    public partial class AddressesView : UserControl
    {
        public AddressesView()
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
                                                                                                  typeof(AddressesView),
                                                                                                  new PropertyMetadata(ViewModelChanged));

        private static void ViewModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AddressesView view = obj as AddressesView;
            view.root.DataContext = args.NewValue;
        }

        private void grid_SelectionChanged(object sender, Xceed.Wpf.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            var address = grid.SelectedItem as AddressViewModel;
            if (address != null)
                App.Locator.Resolve<IMessenger>().Send(new ExposePropertiesMessage(address));
        }

    }
}
