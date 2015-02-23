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
    /// Interaction logic for AddressDesigner.xaml
    /// </summary>
    public partial class AddressDesigner : UserControl
    {
        public AddressDesigner()
        {
            InitializeComponent();
            ViewModel = App.Locator.Resolve<AddressDesignerViewModel>();
        }

        public AddressDesignerViewModel ViewModel
        {
            get
            {
                return (AddressDesignerViewModel)this.GetValue(ViewModelProperty);
            }
            set
            {
                this.SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
                                                                                                  typeof(AddressDesignerViewModel),
                                                                                                  typeof(AddressDesigner),
                                                                                                  new PropertyMetadata(ViewModelChanged));

        private static void ViewModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AddressDesigner view = obj as AddressDesigner;
            view.root.DataContext = args.NewValue;
        }
    }
}
