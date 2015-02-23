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
using Newtonsoft.Json;
using System.Net.Http;
using RapidBase;
using System.Net.Security;

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
                if (SearchedCoins != _.Container)
                {
                    SearchedCoins = _.Container;
                    Search.Execute(null);
                }
            });

            var notrack = LoadCache();
        }
        public Network Network
        {
            get
            {
                return _Factory.Network;
            }
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
                        if (String.IsNullOrEmpty(SearchedCoins))
                            return;
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
                        {
                            client.Colored = true;
                            balance = await client.GetBalance(SearchedCoins, true);
                        }
                        if (balance == null)
                            throw new WebException("Error 404");
                        await PopulateCoins(balance);
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

        private async Task PopulateCoins(BalanceModel balance)
        {
            Coins.Clear();
            foreach (var op in balance.Operations)
            {
                foreach (var coin in op.ReceivedCoins)
                    Coins.Add(new CoinViewModel(Network, coin, op));
            }

            var colored =
                Coins
                .Where(c => c.Coin is ColoredCoin)
                .GroupBy(c => ((ColoredCoin)(c.Coin)).Asset.Id)
                .Select(g =>
                new
                {
                    Coins = g,
                    Metadata = DownloadMetadata(g),
                });

            await Task.WhenAll(colored.Select(c => c.Metadata).ToArray());
        }

        public class AssetDefinition
        {
            [JsonProperty("asset_id")]
            public BitcoinAssetId AssetId
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public int Divisibility
            {
                get;
                set;
            }
            public string Description
            {
                get;
                set;
            }

            [JsonProperty("name_short")]
            public string NameShort
            {
                get;
                set;
            }

            public decimal ToDecimal(ulong quantity)
            {
                var value = (decimal)quantity;
                return value / (decimal)Math.Pow(10, Divisibility);
            }
        }

        private async Task DownloadMetadata(IEnumerable<CoinViewModel> coins)
        {
            RemoteCertificateValidationCallback disableSSL = (a, b, c, d) => true;
            try
            {
                var assetId = ((ColoredCoin)coins.First().Coin).Asset.Id;
                try
                {
                    using (var client = new HttpClient())
                    {
                        var url = "https://api.coinprism.com/v1/assets/";
                        if (Network == Network.TestNet)
                        {
                            ServicePointManager.ServerCertificateValidationCallback += disableSSL;
                            url = "https://testnet.api.coinprism.com/v1/assets/";
                        }
                        var resp = await client.GetAsync(url + assetId.GetWif(Network));
                        resp.EnsureSuccessStatusCode();
                        var str = await resp.Content.ReadAsStringAsync();
                        var definition = Serializer.ToObject<AssetDefinition>(str, Network);
                        foreach (var coin in coins)
                            coin.AssetDefinition = definition;
                    }
                }
                catch
                {
                }
            }
            finally
            {
                if (Network == Network.TestNet)
                {
                    ServicePointManager.ServerCertificateValidationCallback -= disableSSL;
                }
            }
        }
    }

    public class CoinViewModel : PWViewModelBase
    {
        public CoinViewModel(Network network, ICoin coin, BalanceOperation op)
        {
            _Network = network;
            var colored = coin as ColoredCoin;
            BTCValue = coin.TxOut.Value.ToUnit(MoneyUnit.BTC).ToString() + " BTC";
            if (colored == null)
                Value = BTCValue;
            else
            {
                Value = colored.Asset.Quantity.ToString() + " Assets";
                Type = colored.Asset.Id.GetWif(network).ToString();
            }
            Confirmations = op.Confirmations;
            Coin = coin;
            Op = op;
            var address = coin.TxOut.ScriptPubKey.GetDestinationAddress(network);
            if (address != null)
                Owner = address.ToString();
            else
                Owner = coin.TxOut.ScriptPubKey.ToString();
        }

        Network _Network;
        public Network Network
        {
            get
            {
                return _Network;
            }
        }
        public string Type
        {
            get;
            set;
        }

        internal BalanceOperation Op;
        internal ICoin Coin;

        private string _Value;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    OnPropertyChanged(() => this.Value);
                }
            }
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

        private CoinsViewModel.AssetDefinition _AssetDefinition;
        public CoinsViewModel.AssetDefinition AssetDefinition
        {
            get
            {
                return _AssetDefinition;
            }
            set
            {
                if (value != _AssetDefinition)
                {
                    _AssetDefinition = value;
                    OnPropertyChanged(() => this.AssetDefinition);

                    if (value != null)
                    {
                        var q = ((ColoredCoin)Coin).Asset.Quantity;
                        Value = value.ToDecimal(q).ToString() + " " + value.NameShort;
                    }
                }
            }
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
                    SumBTC = g.Select(c => c.Coin.TxOut.Value).Sum(),
                    SumAssets = g.Select(c => c.Coin).OfType<ColoredCoin>().Select(c => c.Amount).Sum().Satoshi,
                    Count = g.Count(),
                    AssetDefinition = g.First().AssetDefinition
                });
            foreach (var group in groups)
            {
                var category = new CategoryAttribute(group.Type);
                if (group.AssetDefinition != null)
                {
                    category = new CategoryAttribute(group.AssetDefinition.Name + " (" + category.Category + ")");
                }
                NewProperty()
                    .SetDisplay("BTC")
                    .SetEditor(typeof(ReadOnlyTextEditor))
                    .AddAttributes(category)
                    .Commit()
                    .SetValue(group.SumBTC.ToUnit(MoneyUnit.BTC) + " BTC");

                if (group.SumAssets != 0)
                {
                    string assets = group.SumAssets + " Assets";
                    if (group.AssetDefinition != null)
                    {
                        assets = group.AssetDefinition
                                      .ToDecimal((ulong)group.SumAssets)
                                      .ToString() + " " + group.AssetDefinition.NameShort;
                    }
                    NewProperty()
                    .SetDisplay("Assets")
                    .SetEditor(typeof(ReadOnlyTextEditor))
                    .AddAttributes(category)
                    .Commit()
                    .SetValue(assets);
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
                .SetValue(cc.Asset.Id.GetWif(coin.Network).ToString());

                NewProperty("ColoredValue")
                .SetEditor(typeof(ReadOnlyTextEditor))
                .AddAttributes(new CategoryAttribute("Colored Coin"))
                .AddAttributes(new DisplayNameAttribute("Value"))
                .Commit()
                .SetValue(coin.Value);

                if (coin.AssetDefinition != null)
                {
                    NewProperty("Name")
                        .SetEditor(typeof(ReadOnlyTextEditor))
                        .SetCategory("Colored Coin")
                        .Commit()
                        .SetValue(coin.AssetDefinition.Name);

                    NewProperty("Description")
                        .SetEditor(typeof(ReadOnlyTextEditor))
                        .SetCategory("Colored Coin")
                        .Commit()
                        .SetValue(coin.AssetDefinition.Description);
                }
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
