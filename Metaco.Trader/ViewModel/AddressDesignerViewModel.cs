using NBitcoin;
using PowerWallet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.ViewModel
{
    public class AddressDesignerViewModel : PWViewModelBase
    {
        public AddressDesignerViewModel(Network network)
        {
            _Network = network;
            MessengerInstance.Register<SearchMessage>(this, m =>
            {
                try
                {
                    QrCode = m.Term;
                }
                catch
                {
                }
            });
        }
        Network _Network;
        private string _QrCode;
        public string QrCode
        {
            get
            {
                return _QrCode;
            }
            set
            {
                if (value != _QrCode)
                {
                    try
                    {
                        var address = BitcoinAddress.Create(value, _Network);
                        IsColored = false;
                        Address = address;
                    }
                    catch (FormatException)
                    {
                        try
                        {
                            var colored = new BitcoinColoredAddress(value, _Network);
                            IsColored = true;
                            Address = colored.Address;
                        }
                        catch (FormatException)
                        {
                            throw;
                        }
                    }
                    _QrCode = value;
                    OnPropertyChanged(() => this.QrCode);
                }
            }
        }

        private BitcoinAddress _Address;
        public BitcoinAddress Address
        {
            get
            {
                return _Address;
            }
            set
            {
                if (value != _Address)
                {
                    _Address = value;
                    OnPropertyChanged(() => this.Address);
                    UpdateQrCore();
                }
            }
        }

        private void UpdateQrCore()
        {
            if (_Address == null)
            {
                _QrCode = null;
                OnPropertyChanged(() => this.QrCode);
            }
            else
            {
                _QrCode = IsColored ? Address.ToColoredAddress().ToString() : Address.ToString();
                OnPropertyChanged(() => this.QrCode);
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
                    UpdateQrCore();
                }
            }
        }
    }
}
