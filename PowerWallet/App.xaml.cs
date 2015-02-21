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
            _Network = e.Args.Contains("-testnet") ? Network.TestNet : Network.Main;
            var window = new MainWindow();
            MainWindow = window;
            
            AssemblyCatalog catalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            InitializationContext ctx = new InitializationContext(window);
            foreach (var module in Modules)
            {
                module.Initialize(ctx);
            }
            AddStatic(ctx.Container);
            Locator = new ViewModelLocator(ctx.Container.Build());
            window.ModuleInitialized();
            window.LoadLayout();
            window.Show();
            base.OnStartup(e);           
        }

        private void AddStatic(ContainerBuilder ioc)
        {
            ioc.RegisterType<CoinsViewModel>().SingleInstance();
            ioc.RegisterType<StatusMainViewModel>().SingleInstance();
            ioc.RegisterType<RapidBaseClientFactory>().SingleInstance();
            ioc.RegisterType<ServerViewModel>().SingleInstance();
            ioc.Register<IStorage>((ctx) => new LocalStorage()).SingleInstance();
            ioc.Register<IMessenger>((ctx) => GalaSoft.MvvmLight.Messaging.Messenger.Default).SingleInstance();
        }

        [ImportMany]
        public IEnumerable<IModule> Modules
        {
            get;
            set;
        }

        //public static IContainer Container
        //{
        //    get;
        //    private set;
        //}

        
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
        public static Network Network
        {
            get
            {
                return _Network ?? Network.Main;
            }
        }
    }
}
