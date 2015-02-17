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
            Donate_Click(null, null);
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
                new AsyncCommand(async t =>
                {
                    var result = await ViewModel.Search(txt);
                    var doc = new LayoutDocument();
                    doc.Title = "Search";
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
                })
                .Notify(App.Locator.Resolve<IMessenger>())
                .Execute();
            }
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            var doc = new LayoutDocument();
            doc.Title = "Donation";
            doc.IsActive = true;
            doc.Content = new Donation();
            documents.Children.Add(doc);
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
