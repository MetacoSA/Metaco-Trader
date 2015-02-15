using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.ViewModel
{
    public class MainViewModel : PWViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(CoinsViewModel coins, StatusMainViewModel status)
        {
            if (coins == null)
                throw new ArgumentNullException("coins");
            if (status == null)
                throw new ArgumentNullException("status");
            _Coins = coins;
            _Status = status;
        }

        private readonly CoinsViewModel _Coins;
        public CoinsViewModel Coins
        {
            get
            {
                return _Coins;
            }
        }

        private readonly StatusMainViewModel _Status;
        public StatusMainViewModel Status
        {
            get
            {
                return _Status;
            }
        }
    }
}