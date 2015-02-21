using PowerWallet.Controls;
using PowerWallet.Messages;
using RapidBase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerWallet.ViewModel
{
    public class WalletsViewModel : PWViewModelBase
    {
        private IStorage _Storage;
        const string WALLETS_KEY = "Opened-Wallets";

        public WalletsViewModel(RapidBaseClientFactory factory, IStorage storage)
        {
            _ClientFactory = factory;
            _Storage = storage;
            var unused = LoadCache();
        }


        private async Task LoadCache()
        {
            var wallets = (await _Storage.Get<string[]>(WALLETS_KEY)) ?? new string[0];
            foreach (var wallet in wallets)
            {
                AddWallet(wallet, false);
            }
        }



        private readonly RapidBaseClientFactory _ClientFactory;
        public RapidBaseClientFactory ClientFactory
        {
            get
            {
                return _ClientFactory;
            }
        }

        private readonly ObservableCollection<WalletViewModel> _Wallets = new ObservableCollection<WalletViewModel>();
        public ObservableCollection<WalletViewModel> Wallets
        {
            get
            {
                return _Wallets;
            }
        }

        public class NewWalletCommand : AsyncCommand
        {
            private WalletsViewModel _Parent;
            public NewWalletCommand(WalletsViewModel parent)
            {
                this._Parent = parent;
            }

            private string _Name = "";
            public string Name
            {
                get
                {
                    return _Name;
                }
                set
                {
                    if (value != _Name)
                    {
                        _Name = value;
                        OnPropertyChanged(() => this.Name);
                    }
                }
            }

            private bool _IsColored;
            public bool IsColored
            {
                get
                {
                    return _IsColored;
                }
                set
                {
                    if (value != _IsColored)
                    {
                        _IsColored = value;
                        OnPropertyChanged(() => this.IsColored);
                    }
                }
            }

            protected override async Task CreateTask(System.Threading.CancellationToken cancelation)
            {
                var client = _Parent.ClientFactory.CreateClient();
                _Parent.AddWallet(await client.CreateWallet(Name), true);
            }
        }

        public ICommand CreateNewWalletCommand()
        {
            return new NewWalletCommand(this).Notify(MessengerInstance);
        }

        internal ICommand CreateOpenWalletCommand()
        {
            return new OpenWalletCommand(this).Notify(MessengerInstance);
        }

        private void AddWallet(string wallet, bool save)
        {
            AddWallet(new WalletModel()
            {
                Name = wallet
            }, save);
        }
        internal void AddWallet(WalletModel model, bool save)
        {
            if (!this.Wallets.Any(w => w.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var wallet = new WalletViewModel(model.Name, this);
                this.Wallets.Add(wallet);
                wallet.Refresh.Execute();
                if (save)
                {
                    var unused = _Storage.Put(WALLETS_KEY, Wallets.Select(w => w.Name).ToArray());
                }
            }
        }
    }
    public class OpenWalletCommand : AsyncCommand
    {
        private WalletsViewModel _Parent;

        public OpenWalletCommand(WalletsViewModel parent)
        {
            this._Parent = parent;
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    OnPropertyChanged(() => this.Name);
                }
            }
        }

        protected override async Task CreateTask(System.Threading.CancellationToken cancelation)
        {
            var client = _Parent.ClientFactory.CreateClient();
            var model = await client.GetWallet(Name);
            if (model == null)
            {
                Error("Wallet does not exist");
            }
            else
            {
                _Parent.AddWallet(model, true);
            }
        }
    }
    public class WalletViewModel : PWViewModelBase
    {
        WalletsViewModel _Parent;
        public WalletViewModel(string name,
                               WalletsViewModel parent)
        {
            _Name = name;
            _Parent = parent;
        }

        private readonly string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        private readonly ObservableCollection<KeySetViewModel> _KeySets = new ObservableCollection<KeySetViewModel>();
        public ObservableCollection<KeySetViewModel> KeySets
        {
            get
            {
                return _KeySets;
            }
        }

        private AsyncCommand _Refresh;
        public AsyncCommand Refresh
        {
            get
            {
                if (_Refresh == null)
                {
                    _Refresh = new AsyncCommand(async _ =>
                    {
                        var rb = _Parent.ClientFactory.CreateClient();
                        var keysets = await rb.GetWalletClient(Name).GetKeySets();
                        KeySets.Clear();
                        foreach (var keyset in keysets)
                        {
                            KeySets.Add(new KeySetViewModel(keyset));
                        }
                    })
                    .Notify(MessengerInstance);
                }
                return _Refresh;
            }
        }

        public void Select()
        {
            MessengerInstance.Send(new ShowCoinsMessage(_Name));
        }
    }

    public class KeySetViewModel : PWViewModelBase
    {
        KeySetData _Keyset;
        public KeySetViewModel(KeySetData keyset)
        {
            _Keyset = keyset;
            _Name = keyset.KeySet.Name;
        }
        private readonly string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public KeySetPropertyViewModel ForPropertyGrid()
        {
            return new KeySetPropertyViewModel(_Keyset);
        }

        public void Select()
        {
            MessengerInstance.Send<ExposePropertiesMessage>(new ExposePropertiesMessage(ForPropertyGrid()));
        }
    }

    public class KeySetPropertyViewModel : PropertyViewModel
    {

        public KeySetPropertyViewModel(KeySetData keyset)
        {
            Name = keyset.KeySet.Name;
            Path = keyset.KeySet.Path.ToString();
            SignatureCount = keyset.KeySet.SignatureCount;
            CurrentPath = keyset.State.CurrentPath;
            int keyIndex = 0;
            foreach (var key in keyset.KeySet.ExtPubKeys.Select(k => k.ToString()))
            {
                keyIndex++;
                NewProperty()
                    .SetDisplay("Key " + keyIndex)
                    .SetEditor(typeof(ReadOnlyTextEditor))
                    .SetCategory("General")
                    .Commit()
                    .SetValue(key);
            }
        }

        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("General")]
        public string Name
        {
            get;
            set;
        }
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("General")]
        public string Path
        {
            get;
            set;
        }
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("General")]
        public int SignatureCount
        {
            get;
            set;
        }

        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [Category("Current State")]
        public NBitcoin.KeyPath CurrentPath
        {
            get;
            set;
        }
    }
}
