using NBitcoin;
using Metaco.Trader.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaco.Trader.Messages
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
