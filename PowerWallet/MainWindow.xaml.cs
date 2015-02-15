using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using PowerWallet.Controls;
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
        ViewModelLocator locator;
        public MainWindow()
        {
            InitializeComponent();
            locator = new ViewModelLocator();
            root.DataContext = locator.Resolve<MainViewModel>();
            coins.DataContext = locator.Resolve<CoinsViewModel>();
            grid.SelectionChanged += grid_SelectionChanged;
        }

        public MainViewModel ViewModel
        {
            get
            {
                return root.DataContext as MainViewModel;
            }
        }

        void grid_SelectionChanged(object sender, Xceed.Wpf.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            if (grid.SelectedItems.Count == 1)
            {
                var coin = grid.SelectedItems[0] as CoinViewModel;
                if (coin != null)
                {
                    propertyGrid.SelectedObject = coin.ForPropertyGrid();
                }
            }
        }

        private async void Search_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var txt = GetText(e.OriginalSource);
            if (txt != null)
            {

                var client = locator.Resolve<RapidBaseClientFactory>().CreateClient();
                var result = await client.Get<string>("whatisit/" + txt);

                var doc = new LayoutDocument();
                doc.Title = txt;
                doc.Content = new TextEditor()
                {
                    Text = result,
                    InputBindings = 
                    {
                        new InputBinding(NavigationCommands.Search, NavigationCommands.Search.InputGestures[0])
                    },
                    ContextMenu = new ContextMenu()
                    {
                        Items = 
                        {
                          new MenuItem()
                          {
                              Command = NavigationCommands.Search
                          }
                        }
                    }
                };
                doc.IsActive = true;
                documents.Children.Add(doc);
            }
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
