using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using PowerWallet.Controls;
using PowerWallet.Messages;
using PowerWallet.ViewModel;
using RapidBase.Client;
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
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace PowerWallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NiceWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            root.DataContext = App.Locator.Resolve<MainViewModel>();
            serverGrid.SelectedObject = ViewModel.Server;
            App.Locator.Messenger.Register<ExposePropertiesMessage>(this, m =>
            {
                propertyGrid.SelectedObject = m.Target.ForPropertyGrid();
            });
        }

        public MainViewModel ViewModel
        {
            get
            {
                return root.DataContext as MainViewModel;
            }
        }

        private void Search_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var txt = GetText(e.OriginalSource);
            if (txt != null)
            {
                var search = ActivateSearch();
                search.SearchedTerm = txt;
                search.Search.Execute();
            }
        }


        private SearchViewModel ActivateSearch()
        {
            if (!documents.Children.Contains(search))
            {
                documents.Children.Add(search);
            }
            search.IsActive = true;
            return ((SearchView)(search.Content)).ViewModel;
        }


        private string GetText(object source)
        {
            var txt = source as TextBox;
            if (txt != null)
                return txt.Text;
            var txtArea = source as TextArea;
            if (txtArea != null)
                return txtArea.Selection.GetText();
            var txtEditor = source as TextEditor;
            if (txtEditor != null)
                return txtEditor.TextArea.Selection.GetText();
            return null;
        }

    }
}
