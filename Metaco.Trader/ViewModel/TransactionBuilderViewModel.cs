using NBitcoin;
using Metaco.Trader.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Metaco.Trader.ViewModel
{
    public class TransactionBuilderViewModel : PWViewModelBase
    {
        public TransactionBuilderViewModel(Network network)
        {
            MessengerInstance.Register<SelectedCoinMessage>(this, OnCoinSelected);
            _Network = network;
        }


        Network _Network;
        public void OnCoinSelected(SelectedCoinMessage message)
        {
            if(InputCoins.All(c=>c.Coin.Outpoint != message.Coin.Coin.Outpoint))
                InputCoins.Add(message.Coin);
        }
        private readonly ObservableCollection<CoinViewModel> _InputCoins = new ObservableCollection<CoinViewModel>();
        public ObservableCollection<CoinViewModel> InputCoins
        {
            get
            {
                return _InputCoins;
            }
        }
    }
}
