using NBitcoin;
using PowerWallet.Controls;
using RapidBase.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.ViewModel
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        private readonly RapidBaseClientFactory _Factory;
        [System.ComponentModel.Browsable(false)]
        public RapidBaseClientFactory Factory
        {
            get
            {
                return _Factory;
            }
        }
        public ServerViewModel(RapidBaseClientFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            _Factory = factory;
            Observable
                .Interval(TimeSpan.FromSeconds(30.0))
                .ObserveHere()
                .Subscribe(Update);
            Update(0);
        }

        private async void Update(long ignored)
        {
            try
            {
                var factory = Factory.CreateClient();
                var block = await factory.GetBlock(new BlockFeature(SpecialFeature.Last), true);
                Tip = block.AdditionalInformation.BlockId;
                Height = block.AdditionalInformation.Height;
                Time = block.AdditionalInformation.BlockHeader.BlockTime.LocalDateTime;
            }
            catch (Exception ex)
            {
                PWTrace.Error("Error while updating rapidbase server information", ex);
            }
        }

        public string ServerName
        {
            get
            {
                return Factory.Uri.AbsoluteUri;
            }
            set
            {
                if (value != Factory.Uri.AbsoluteUri)
                {
                    Factory.Uri = new Uri(value);
                }
            }
        }

        private uint256 _Tip;
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        public uint256 Tip
        {
            get
            {
                return _Tip;
            }
            set
            {
                if (value != _Tip)
                {
                    _Tip = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Tip"));
                }
            }
        }

        private int _Height;
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                if (value != _Height)
                {
                    _Height = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Height"));
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private DateTime _Time;
        [Editor(typeof(ReadOnlyTextEditor), typeof(ReadOnlyTextEditor))]
        [DisplayName("Time (local)")]
        public DateTime Time
        {
            get
            {
                return _Time;
            }
            set
            {
                if (value != _Time)
                {
                    _Time = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Time"));
                }
            }
        }
    }
}
