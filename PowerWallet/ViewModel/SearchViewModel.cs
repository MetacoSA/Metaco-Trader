using NBitcoin;
using NBitcoin.DataEncoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.ViewModel
{
    public class SearchViewModel : PWViewModelBase
    {
        public SearchViewModel()
        {
            
        }
        private string _SearchedTerm;
        public string SearchedTerm
        {
            get
            {
                return _SearchedTerm;
            }
            set
            {
                if (value != _SearchedTerm)
                {
                    _SearchedTerm = value;
                    OnPropertyChanged(() => this.SearchedTerm);
                }
            }
        }

        private AsyncCommand _Search;
        public AsyncCommand Search
        {
            get
            {
                if (_Search == null)
                {
                    _Search = new AsyncCommand(async t =>
                       {
                           var search = SearchedTerm;
                           var client = App.Locator.Resolve<RapidBaseClientFactory>().CreateClient();
                           try
                           {
                               var result = await client.Get<string>("whatisit/" + search);
                               Content = result;
                               return;
                           }
                           catch (Exception)
                           {
                               try
                               {

                                   if (search.StartsWith("00000000"))
                                   {
                                       Block block = new Block();
                                       block.FromBytes(Encoders.Hex.DecodeData(search));
                                       Content = block.ToString();
                                       return;
                                   }
                                   Transaction transaction = new Transaction(search);
                                   Content = transaction.ToString();
                                   return;
                               }
                               catch (Exception)
                               {
                                   Content = "Good question holmes!";
                                   return;
                               }
                           }
                       }).Notify(MessengerInstance);
                }

                return _Search;
            }
        }

        private string _Content;
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (value != _Content)
                {
                    _Content = value;
                    OnPropertyChanged(() => this.Content);
                }
            }
        }
    }
}
