using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using PowerWallet.Controls;
using PowerWallet.Messages;
using PowerWallet.ViewModel;
using RapidBase.Client;
using System;
using System.Collections.Generic;
using System.IO;
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
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
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
                propertyGrid.SelectedObject = m.Target;
            });
            LoadDefaultLayout();
            FillViewMenu();
            LoadLayout();
        }

        private void LoadDefaultLayout()
        {
            XmlLayoutSerializer seria = new XmlLayoutSerializer(dockManager);
            StringWriter writer = new StringWriter();
            seria.Serialize(writer);
            defaultLayout = writer.ToString();
        }

        private void FillViewMenu()
        {
            foreach (var view in dockManager
                .Layout
                .Descendents()
                .OfType<LayoutContent>()
                .Where(c => c.ContentId != null))
            {
                var localView = view;
                MenuItem subMenu = new MenuItem();
                subMenu.Header = localView.Title;
                viewMenu.Items.Add(subMenu);
                subMenu.Click += (s, a) =>
                {
                    var layout = dockManager.Layout.Hidden.FirstOrDefault(v => v.Content == localView.Content);
                    if (layout != null)
                    {
                        layout.Show();
                    }
                    var doc = localView as LayoutDocument;
                    if (doc != null)
                    {
                        var documents = GetDocumentPane();
                        if (!documents.Children.Contains(doc))
                            documents.Children.Add(doc);
                        doc.IsActive = true;
                    }
                };
            }
        }

        private void LoadLayout()
        {
            try
            {
                var localStorage = App.Locator.Resolve<IStorage>();
                var layout = localStorage.Get<string>(LAYOUT_KEY).Result;
                if (layout != null)
                {

                    XmlLayoutSerializer seria = new XmlLayoutSerializer(dockManager);
                    seria.Deserialize(new StringReader(layout));
                }
            }
            catch (Exception ex)
            {
                PWTrace.Error("Exception when restoring layout", ex);
            }
        }

        const string LAYOUT_KEY = "Layout-Data";


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
            var documents = GetDocumentPane();
            if (!documents.Children.Contains(search))
            {
                documents.Children.Add(search);
            }
            search.IsActive = true;
            return ((SearchView)(search.Content)).ViewModel;
        }

        private LayoutDocumentPane GetDocumentPane()
        {
            var documents = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();
            return documents;
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            try
            {
                var localStorage = App.Locator.Resolve<IStorage>();
                XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer);
                localStorage.Put(LAYOUT_KEY, writer.ToString()).Wait();
            }
            catch (Exception ex)
            {
                PWTrace.Error("Error when saving layout", ex);
            }
        }

        string defaultLayout;
        private void ResetLayout_Click(object sender, RoutedEventArgs e)
        {
            XmlLayoutSerializer seria = new XmlLayoutSerializer(dockManager);
            seria.Deserialize(new StringReader(defaultLayout));
        }

        private void NewWallet_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = App.Locator.Resolve<WalletsViewModel>().CreateNewWalletCommand();
            Show(new NewWalletWindow()
            {
                DataContext = command
            });
        }

        private void Show(ChildWindow win)
        {
            win.WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation.Center;
            windowsContainer.Children.Add(win);            
            win.Show();
        }

        private void OpenWallet_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = App.Locator.Resolve<WalletsViewModel>().CreateOpenWalletCommand();
            Show(new OpenWalletWindow()
            {
                DataContext = command
            });
        }
    }
}
