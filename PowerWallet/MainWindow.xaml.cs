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
using System.Reflection;
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
using Xceed.Wpf.AvalonDock.Controls;
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
        }

        internal void ModuleInitialized()
        {
            statusBar.DataContext = App.Locator.Resolve<StatusMainViewModel>();
        }

        public MenuItem ViewMenu
        {
            get
            {
                return viewMenu;
            }
        }


        public void RegisterAnchorable<T>(string viewName, int defaultWidth = 250, int defaultHeight = 400) where T : new()
        {
            _Contents.Add(viewName, new Lazy<object>(() => new T()));
            _ShowViews.Add(viewName, () =>
            {
                var anchorable = Find<LayoutAnchorable>(viewName);
                if (anchorable == null)
                {
                    anchorable = new LayoutAnchorable();
                    anchorable.Title = viewName;
                    anchorable.ContentId = viewName;
                    anchorable.FloatingWidth = defaultWidth;
                    anchorable.FloatingHeight = defaultHeight;
                    LayoutFloatingWindow layoutFloatingWindow = new LayoutAnchorableFloatingWindow
                    {
                        RootPanel = new LayoutAnchorablePaneGroup(new LayoutAnchorablePane(anchorable)
                        {
                            //DockMinHeight = layoutPositionableElement.DockMinHeight,
                            //DockMinWidth = layoutPositionableElement.DockMinWidth,
                            //FloatingLeft = layoutPositionableElement.FloatingLeft,
                            //FloatingTop = layoutPositionableElement.FloatingTop,
                            FloatingWidth = defaultWidth,
                            FloatingHeight = defaultHeight,
                        })
                    };
                    dockManager.Layout.FloatingWindows.Add(layoutFloatingWindow);
                    AttachToUI(layoutFloatingWindow, anchorable);
                }
                if (anchorable.Content == null)
                {
                    anchorable.Content = FindContent(viewName);
                }
                if (anchorable.IsHidden)
                    anchorable.Show();

                anchorable.IsActive = true;
            });

            MenuItem subMenu = new MenuItem();
            subMenu.Header = viewName;
            viewMenu.Items.Add(subMenu);
            subMenu.Click += (s, a) => ShowView(viewName);
        }

        public void RegisterDocument<T>(string viewName) where T : new()
        {
            _Contents.Add(viewName, new Lazy<object>(() => new T()));
            _ShowViews.Add(viewName, () =>
            {
                var doc = Find<LayoutDocument>(viewName);
                if (doc == null)
                {
                    doc = new LayoutDocument();
                    doc.Title = viewName;
                    doc.ContentId = viewName;
                    GetDocumentPane().Children.Add(doc);
                }
                if (doc.Content == null)
                {
                    doc.Content = FindContent(viewName);
                }
                doc.IsActive = true;
            });


            MenuItem subMenu = new MenuItem();
            subMenu.Header = viewName;
            viewMenu.Items.Add(subMenu);
            subMenu.Click += (s, a) => ShowView(viewName);
        }

        Dictionary<string, Action> _ShowViews = new Dictionary<string, Action>();
        Dictionary<string, Lazy<object>> _Contents = new Dictionary<string, Lazy<object>>();
        private object FindContent(string contentId)
        {
            Lazy<object> val;
            _Contents.TryGetValue(contentId, out val);
            return val.Value;
        }

        /// <summary>
        /// Big hack to make this freaking windows appear
        /// </summary>
        /// <param name="window"></param>
        private void AttachToUI(LayoutFloatingWindow layoutFloatingWindow, LayoutAnchorable contentModel)
        {
            var ctor = typeof(LayoutAnchorableFloatingWindowControl).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();

            var fwc = (LayoutAnchorableFloatingWindowControl)ctor.Invoke(new object[] { layoutFloatingWindow });

            fwc.Width = contentModel.FloatingWidth;
            fwc.Height = contentModel.FloatingHeight;
            fwc.Left = this.Left + (this.Width / 2) - (fwc.Width / 2);
            fwc.Top = this.Top + (this.Height / 2) - (fwc.Height / 2);

            var floatings = (List<LayoutFloatingWindowControl>)
                typeof(DockingManager)
                .GetField("_fwList", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(dockManager);

            floatings.Add(fwc);
            fwc.Show();
        }


        private T Find<T>()
        {
            return dockManager.Layout.Descendents().OfType<T>().FirstOrDefault();
        }

        private T Find<T>(string viewName) where T : LayoutContent
        {
            return dockManager.Layout.Descendents().OfType<T>().Where(t => t.ContentId == viewName).FirstOrDefault();
        }

        public void LoadLayout()
        {
            try
            {
                var localStorage = App.Locator.Resolve<IStorage>();
                var layout = localStorage.Get<string>(LAYOUT_KEY).Result;
                if (layout != null)
                {

                    XmlLayoutSerializer seria = new XmlLayoutSerializer(dockManager);
                    seria.LayoutSerializationCallback += LayoutDeserialization;
                    seria.Deserialize(new StringReader(layout));
                }
            }
            catch (Exception ex)
            {
                PWTrace.Error("Exception when restoring layout", ex);
            }
        }

        void LayoutDeserialization(object sender, LayoutSerializationCallbackEventArgs e)
        {
            if (e.Content == null)
            {
                e.Content = FindContent(e.Model.ContentId);
            }
        }


        const string LAYOUT_KEY = "Layout-Data";


        private void Search_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var txt = GetText(e.OriginalSource);
            if (txt != null)
            {
                var search = App.Locator.Resolve<SearchViewModel>();
                search.SearchedTerm = txt;
                search.Search.Execute();
                ShowView("Search");
            }
        }

        private void ShowView(string viewName)
        {
            _ShowViews[viewName]();
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

        private void ResetLayout_Click(object sender, RoutedEventArgs e)
        {
            XmlLayoutSerializer seria = new XmlLayoutSerializer(dockManager);
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PowerWallet.DefaultLayout.xml"))
            {
                seria.LayoutSerializationCallback += LayoutDeserialization;
                seria.Deserialize(stream);
            }
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
