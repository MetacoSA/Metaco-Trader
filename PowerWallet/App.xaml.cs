using NBitcoin;
using PowerWallet.Modules;
using PowerWallet.ViewModel;
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

namespace PowerWallet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var network = e.Args.Contains("-testnet") ? Network.TestNet : Network.Main;
            _Network = network;
            var window = new MainWindow();
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
                return typeof(App).Assembly.GetName().Version.ToString() + " by Nicolas Dorier (" + Network.ToString() + "net)";
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
