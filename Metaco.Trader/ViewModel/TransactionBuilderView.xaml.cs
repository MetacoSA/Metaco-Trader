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

namespace Metaco.Trader.ViewModel
{
    /// <summary>
    /// Interaction logic for TransactionBuilderView.xaml
    /// </summary>
    public partial class TransactionBuilderView : UserControl
    {
        public TransactionBuilderView()
        {
            InitializeComponent();
            ViewModel = App.Locator.Resolve<TransactionBuilderViewModel>();
            grid.SelectionChanged += grid_SelectionChanged;
        }

        void grid_SelectionChanged(object sender, Xceed.Wpf.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            CoinPane.CoinSelectionChanged(grid);
        }
        public TransactionBuilderViewModel ViewModel
        {
            get
            {
                return (TransactionBuilderViewModel)this.GetValue(ViewModelProperty);
            }
            set
            {
                this.SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
                                                                                                  typeof(TransactionBuilderViewModel),
                                                                                                  typeof(TransactionBuilderView),
                                                                                                  new PropertyMetadata(ViewModelChanged));

        private static void ViewModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TransactionBuilderView view = obj as TransactionBuilderView;
            view.root.DataContext = args.NewValue;
        }
    }
}
