using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
            ViewModel = App.Locator.Resolve<SearchViewModel>();
            searchBox.ToolTip =
                "Formats:\r\n"  +
                "blockId[-raw|-json]\r\n" +
                "txid[-raw|-json]\r\n" +
                "script|address|base58|pubkeys\r\n" +
                "blockheader\r\n" +
                "transaction";
        }

        public SearchViewModel ViewModel
        {
            get
            {
                return (SearchViewModel)this.GetValue(ViewModelProperty);
            }
            set
            {
                this.SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
                                                                                                  typeof(SearchViewModel),
                                                                                                  typeof(SearchView),
                                                                                                  new PropertyMetadata(ViewModelChanged));

        private static void ViewModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SearchView view = obj as SearchView;
            view.ViewModelChanged(args);
        }

        private void ViewModelChanged(DependencyPropertyChangedEventArgs args)
        {
            root.DataContext = args.NewValue;
            if(ViewModel != null)
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            UpdateContent();
            if (args.OldValue != null)
                ((INotifyPropertyChanged)args.OldValue).PropertyChanged -= ViewModel_PropertyChanged;
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
            {
                UpdateContent();
            }
        }

        private void UpdateContent()
        {
            if (ViewModel == null)
            {
                editor.Text = null;
            }
            else
            {
                editor.Text = ViewModel.Content;
            }
        }
    }
}
