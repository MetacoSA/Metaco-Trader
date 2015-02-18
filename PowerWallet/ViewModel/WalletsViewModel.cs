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

namespace PowerWallet.ViewModel
{
    public class WalletsViewModel : PWViewModelBase
    {
        public WalletsViewModel(RapidBaseClientFactory factory)
        {
            var wallet = new WalletViewModel("temp-1Nicolas Dorier", this);
            Wallets.Add(wallet);
            _ClientFactory = factory;
            wallet.Refresh.Execute();
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
