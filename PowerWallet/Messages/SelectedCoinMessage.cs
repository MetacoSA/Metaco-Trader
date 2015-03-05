using NBitcoin;
using PowerWallet.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.Messages
{
    public class SelectedCoinMessage
    {
        public SelectedCoinMessage(CoinViewModel coin)
        {
            Coin = coin;
        }

        public CoinViewModel Coin
        {
            get;
            set;
        }
    }
}
