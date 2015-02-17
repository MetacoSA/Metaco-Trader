using NBitcoin;
using PowerWallet.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
            base.OnStartup(e);
        }
        private static ViewModelLocator _Locator;
        public static ViewModelLocator Locator
        {
            get
            {
                if (_Locator == null)
                    _Locator = new ViewModelLocator();
                return _Locator;
            }
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
