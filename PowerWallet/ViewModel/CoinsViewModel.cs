using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RapidBase.Client;
using NBitcoin;
using System.Collections.ObjectModel;
using RapidBase.Models;
using System.Threading;
using System.ComponentModel;
using PowerWallet.Controls;
using PowerWallet.Messages;
using System.Windows.Input;
using System.Net;

namespace PowerWallet.ViewModel
{
    public class CoinsViewModel : PWViewModelBase
    {
        RapidBaseClientFactory _Factory;
        public CoinsViewModel(RapidBaseClientFactory factory)
        {
            _Factory = factory;
            this
                .ObservablePropertyChanged
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveHere()
                .Subscribe((_) =>
                {
                    Search.Execute(null);
                });
            SearchedCoins = "akSjSW57xhGp86K6JFXXroACfRCw7SPv637";
            //15sYbVpRh6dyWycZMwPdxJWD4xbfxReeHe (me)
            //akSjSW57xhGp86K6JFXXroACfRCw7SPv637 (colored)
        }

        ICommand _Search;
        public ICommand Search
        {
            get
            {
                if (_Search == null)
                    _Search = new AsyncCommand(async c =>
                    {
                        var client = _Factory.CreateClient();
                        BalanceModel balance = null;
                        try
                        {
                            balance = await client.GetBalance(BitcoinAddress.Create(SearchedCoins), true);
                        }
                        catch (FormatException)
                        {
                        }
                        if (balance == null)
                            try
                            {
                                balance = await client.GetBalance(new BitcoinColoredAddress(SearchedCoins), true);
                            }
                            catch (FormatException)
                            {
                            }
                        if (balance == null)
                            balance = await client.GetBalance(SearchedCoins, true);
                        if (balance == null)
                            throw new WebException("Error 404");
                        PopulateCoins(balance);
                    })
                    .Notify(MessengerInstance);
                return _Search;
            }
        }



        private string _SearchedCoins;
        public string SearchedCoins
        {
            get
            {
                return _SearchedCoins;
            }
            set
            {
                if (value != _SearchedCoins)
                {
                    _SearchedCoins = value;
                    OnPropertyChanged(() => this.SearchedCoins);
                }
            }
        }

        private readonly ObservableCollection<CoinViewModel> _Coins = new ObservableCollection<CoinViewModel>();
        public ObservableCollection<CoinViewModel> Coins
        {
            get
            {
                return _Coins;
            }
        }

        private void PopulateCoins(BalanceModel balance)
        {
            Coins.Clear();
            foreach (var op in balance.Operations)
            {
                foreach (var coin in op.ReceivedCoins)
                    Coins.Add(new CoinViewModel(coin, op));
            }
        }
    }

    public class CoinViewModel : PWViewModelBase, IHasProperties
    {
        public CoinViewModel(ICoin coin, BalanceOperation op)
        {
            var colored = coin as ColoredCoin;
            if (colored == null)
                Value = coin.Amount.ToUnit(MoneyUnit.BTC).ToString() + " BTC";
            else
            {
                Value = colored.Asset.Quantity.ToString() + " Assets";
                Type = colored.Asset.Id.GetWif(Network.Main).ToString();
            }
            Confirmations = op.Confirmations;
            Coin = coin;
            Op = op;
            var address = coin.TxOut.ScriptPubKey.GetDestinationAddress(Network.Main);
            if (address != null)
                Owner = address.ToString();
            else
                Owner = coin.TxOut.ScriptPubKey.ToString();
        }


        public string Type
        {
            get;
            set;
        }

        internal BalanceOperation Op;
        internal ICoin Coin;

        public string Value
        {
            get;
            set;
        }
        public int Confirmations
        {
            get;
            set;
        }

        public object ForPropertyGrid()
        {
            return new CoinPropertyViewModel(this);
        }

        public string Owner
        {
            get;
            set;
        }
    }

    [DisplayName("Coin")]
    public class CoinPropertyViewModel
    {
        public CoinPropertyViewModel(CoinViewModel coin)
        {
            Value = coin.Value;
            Confirmations = coin.Confirmations;
            BlockId = coin.Op.BlockId;
            TransactionId = coin.Op.TransactionId;
            Owner = coin.Owner;
        }
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        public string Value
        {
            get;
            set;
        }
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        public int Confirmations
        {
            get;
            set;
        }

        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        public string Owner
        {
            get;
            set;
        }

        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        public uint256 BlockId
        {
            get;
            set;
        }
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        public uint256 TransactionId
        {
            get;
            set;
        }
        public override string ToString()
        {
            return "Coin";
        }
    }
}
