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
using NBitcoin.OpenAsset;

namespace PowerWallet.ViewModel
{
    public class CoinsViewModel : PWViewModelBase
    {
        RapidBaseClientFactory _Factory;
        IStorage _LocalStorage;

        public CoinsViewModel(RapidBaseClientFactory factory, IStorage localStorage)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (localStorage == null)
                throw new ArgumentNullException("localStorage");
            _Factory = factory;
            _LocalStorage = localStorage;                     


            //15sYbVpRh6dyWycZMwPdxJWD4xbfxReeHe (me)
            //akSjSW57xhGp86K6JFXXroACfRCw7SPv637 (colored)
            
            MessengerInstance.Register<ShowCoinsMessage>(this, _ =>
            {
                SearchedCoins = _.Container;
                Search.Execute(null);
            });

            var notrack = LoadCache();
        }

        const string CACHE_KEY = "Default-Coin-Search";

        private async Task LoadCache()
        {
            _SearchedCoins = await _LocalStorage.Get<string>(CACHE_KEY);
            if (_SearchedCoins != null)
            {
                OnPropertyChanged(() => this.SearchedCoins);
                Search.Execute(null);
            }
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
                    var notrack = _LocalStorage.Put(CACHE_KEY, value);
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

    public class CoinViewModel : PWViewModelBase
    {
        public CoinViewModel(ICoin coin, BalanceOperation op)
        {
            var colored = coin as ColoredCoin;
            BTCValue = coin.TxOut.Value.ToUnit(MoneyUnit.BTC).ToString() + " BTC";
            if (colored == null)
                Value = BTCValue;
            else
            {
                Value = colored.Asset.Quantity.ToString() + " Assets";
                Type = colored.Asset.Id.GetWif(App.Network).ToString();
            }
            Confirmations = op.Confirmations;
            Coin = coin;
            Op = op;
            var address = coin.TxOut.ScriptPubKey.GetDestinationAddress(App.Network);
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

        public string BTCValue
        {
            get;
            set;
        }
    }


    public class CoinsPropertyViewModel : PropertyViewModel
    {
        public CoinsPropertyViewModel(CoinViewModel[] coins)
        {
            var groups = coins
                .GroupBy(c => c.Type ?? "BTC")
                .Select(g => new
                {
                    Type = g.Key,
                    SumBTC = g.Select(c=>c.Coin.TxOut.Value).Sum(),
                    SumAssets = g.Select(c=>c.Coin).OfType<ColoredCoin>().Select(c=>c.Amount).Sum().Satoshi,
                    Count = g.Count()
                });
            foreach (var group in groups)
            {
                var category = new CategoryAttribute(group.Type);
                NewProperty()
                    .SetDisplay("Value")
                    .SetEditor(typeof(ReadOnlyTextEditor))
                    .AddAttributes(category)
                    .Commit()
                    .SetValue(group.SumBTC.ToUnit(MoneyUnit.BTC));

                if (group.SumAssets != 0)
                {
                    NewProperty()
                    .SetDisplay("Quantity")
                    .SetEditor(typeof(ReadOnlyTextEditor))
                    .AddAttributes(category)
                    .Commit()
                    .SetValue(group.SumAssets);
                }

                NewProperty()
                    .SetDisplay("Count")
                    .SetEditor(typeof(ReadOnlyTextEditor))
                    .AddAttributes(category)
                    .Commit()
                    .SetValue(group.Count);
            }
        }
    }
    
    public class CoinPropertyViewModel : PropertyViewModel
    {
        public CoinPropertyViewModel(CoinViewModel coin)
        {
            Value = coin.BTCValue;
            Confirmations = coin.Confirmations;
            BlockId = coin.Op.BlockId;
            TransactionId = coin.Op.TransactionId;
            Owner = coin.Owner;
            var cc = coin.Coin as ColoredCoin;
            if (cc != null)
            {
                NewProperty("Asset")
                .SetEditor(typeof(ReadOnlyTextEditor))
                .AddAttributes(new CategoryAttribute("Colored Coin"))
                .Commit()
                .SetValue(cc.Asset.Id.GetWif(App.Network).ToString());

                NewProperty("ColoredValue")                  
                .SetEditor(typeof(ReadOnlyTextEditor))
                .AddAttributes(new CategoryAttribute("Colored Coin"))
                .AddAttributes(new DisplayNameAttribute("Value"))
                .Commit()
                .SetValue(coin.Value);
            }
        }

        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("Coin")]
        public string Value
        {
            get;
            set;
        }
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("Coin")]
        public int Confirmations
        {
            get;
            set;
        }

        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("Coin")]
        public string Owner
        {
            get;
            set;
        }

        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("Coin")]
        public uint256 BlockId
        {
            get;
            set;
        }
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("Coin")]
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
