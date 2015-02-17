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
                return typeof(App).Assembly.GetName().Version.ToString() + " by Nicolas Dorier (" + Network.ToString() + ")";
            }
        }

        public static Network Network
        {
            get
            {
                return NBitcoin.Network.Main;
            }
        }
    }
}
