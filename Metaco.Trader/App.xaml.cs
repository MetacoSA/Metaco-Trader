using NBitcoin;
using Metaco.Trader.Modules;
using Metaco.Trader.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using Metaco.Trader.Controls;
using System.Diagnostics;

namespace Metaco.Trader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var network = e.Args.Contains("-testnet") ? Network.TestNet : null;
            network = network ?? (e.Args.Contains("-mainnet") ? Network.Main : null);
            network = network ?? GetDefaultNetwork();
            
            _Network = network;

            var window = new MainWindow();
            SetLogo(window);
            MainWindow = window;

            AssemblyCatalog catalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            InitializationContext ctx = new InitializationContext(window);
            ctx.Container.RegisterInstance<Network>(network);
            foreach (var module in Modules)
            {
                module.Initialize(ctx);
            }
            Locator = new ViewModelLocator(ctx.Container.Build());
            window.ModuleInitialized();
            window.LoadLayout();
            window.Show();
            base.OnStartup(e);
        }

        private Network GetDefaultNetwork()
        {
            var storage = new LocalStorage("App");
            var network = storage.Get<string>("DefaultNetwork").Result;
            if (network == "mainnet")
                return Network.Main;
            if (network == "testnet")
                return Network.TestNet;
            return Network.Main;
        }

        internal static void Restart(Network network)
        {
            App.Current.MainWindow.Close();

            var storage = new LocalStorage("App");
            string networkStr = network == Network.Main ? "mainnest" : "testnet";
            storage.Put("DefaultNetwork", networkStr).Wait();
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void SetLogo(MainWindow window)
        {
            Image image = new Image();
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/Metaco.Trader;component/Images/BC_Logo_.png");
            logo.EndInit();
            image.Source = logo;

            image.Width = 40;
            image.Height = image.Width;
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            if (Network == Network.TestNet)
            {
                image.Effect = new ChangeHueEffect()
                {
                    HueShift = 0.2
                };
            }
            window.Icon = CreateBitmap(image, false);
        }

        //http://www.nerdparadise.com/tech/csharp/rendercontrolasbitmap/
        public BitmapSource CreateBitmap(FrameworkElement element, bool isInUiTree)
        {
            if (!isInUiTree)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                element.Arrange(new Rect(new Point(0, 0), element.DesiredSize));
            }

            int width = (int)Math.Ceiling(element.ActualWidth);
            int height = (int)Math.Ceiling(element.ActualHeight);

            width = width == 0 ? 1 : width;
            height = height == 0 ? 1 : height;

            RenderTargetBitmap rtbmp = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Default);
            rtbmp.Render(element);
            return rtbmp;
        }

        [ImportMany]
        public IEnumerable<IModule> Modules
        {
            get;
            set;
        }

        public static ViewModelLocator Locator
        {
            get;
            private set;
        }

        public static string Caption
        {
            get
            {
                return typeof(App).Assembly.GetName().Version.ToString() + " by Nicolas Dorier (" + (Network == Network.TestNet ? "Testnet" : "Mainnet") + ")";
            }
        }
        static Network _Network;
        static Network Network
        {
            get
            {
                return _Network ?? Network.Main;
            }
        }
    }
}
