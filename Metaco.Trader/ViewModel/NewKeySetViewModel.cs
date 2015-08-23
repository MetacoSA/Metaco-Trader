using NBitcoin;
using QBitNinja.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PowerWallet.ViewModel
{
    public class NewKeySetViewModel : AsyncCommand
    {
        public class ExtPubkeyViewModel : PWViewModelBase
        {
            Network _Network;
            public ExtPubkeyViewModel(Network network)
            {
                _Network = network;
            }
            public AsyncCommand Generate
            {
                get
                {
                    return new AsyncCommand(_ =>
                    {
                        var key = new ExtKey().GetWif(_Network);
                        Clipboard.SetText(key.ToString());
                        _.Info("Private key copied in clipboard");
                        Value = key.ExtKey.Neuter().ToString(_Network);
                        return Task.FromResult(true);
                    })
                    .Notify(MessengerInstance);
                }
            }
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

            private string _FieldName;
            public string FieldName
            {
                get
                {
                    return _FieldName;
                }
                set
                {
                    if (value != _FieldName)
                    {
                        _FieldName = value;
                        OnPropertyChanged(() => this.FieldName);
                    }
                }
            }
        }
        private WalletViewModel _Parent;

        public NewKeySetViewModel(WalletViewModel walletViewModel, Network network)
        {
            _Network = network;
            _Parent = walletViewModel;
            KeyCount = 1;
            SignatureCount = 1;
            UpdateExtPubKeys();
        }
        Network _Network;

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

        private string _KeyPath;
        public string KeyPath
        {
            get
            {
                return _KeyPath;
            }
            set
            {
                if (value != _KeyPath)
                {
                    _KeyPath = value;
                    OnPropertyChanged(() => this.KeyPath);
                }
            }
        }

        private int _SignatureCount;
        public int SignatureCount
        {
            get
            {
                return _SignatureCount;
            }
            set
            {
                if (value != _SignatureCount)
                {
                    _SignatureCount = value;
                    OnPropertyChanged(() => this.SignatureCount);
                }
            }
        }


        private readonly ObservableCollection<ExtPubkeyViewModel> _ExtPubKeys = new ObservableCollection<ExtPubkeyViewModel>();
        public ObservableCollection<ExtPubkeyViewModel> ExtPubKeys
        {
            get
            {
                return _ExtPubKeys;
            }
        }

        private int _KeyCount;
        public int KeyCount
        {
            get
            {
                return _KeyCount;
            }
            set
            {
                if (value != _KeyCount)
                {
                    _KeyCount = value;
                    OnPropertyChanged(() => this.KeyCount);
                    UpdateExtPubKeys();
                }
            }
        }

        private void UpdateExtPubKeys()
        {
            int i = 0;
            for (i = 0 ; i < KeyCount ; i++)
            {
                if (ExtPubKeys.Count < i + 1)
                {
                    ExtPubKeys.Add(new ExtPubkeyViewModel(_Network)
                    {
                        FieldName = "Key " + (i + 1),
                    });
                }
            }
            while (ExtPubKeys.Count != KeyCount)
            {
                ExtPubKeys.RemoveAt(ExtPubKeys.Count - 1);
            }
        }

        protected override async Task CreateTask(System.Threading.CancellationToken cancelation)
        {
            HDKeySet keyset = new HDKeySet();
            keyset.Name = Name;
            keyset.Path = String.IsNullOrWhiteSpace(KeyPath) ? null : new KeyPath(KeyPath);
            keyset.SignatureCount = SignatureCount;
            keyset.ExtPubKeys
                =
                ExtPubKeys
                .Select(_ => new BitcoinExtPubKey(_.Value, _Network))
                .ToArray();

            var client = _Parent._Parent.ClientFactory.CreateClient();
            await client.CreateKeySet(_Parent.Name, keyset);
            Info("Keyset created");
            _Parent.Refresh.Execute(false);
        }


    }
}
