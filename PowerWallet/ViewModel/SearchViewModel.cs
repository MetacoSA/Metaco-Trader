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
                           if (String.IsNullOrWhiteSpace(search))
                           {
                               Content = "";
                               return;
                           }
                           var client = App.Locator.Resolve<RapidBaseClientFactory>().CreateClient();
                           try
                           {
                               if (search.Length < 520 * 2)
                               {
                                   if (search.EndsWith("-json") || search.EndsWith("-raw"))
                                   {
                                       var format = search.Substring(search.LastIndexOf("-") + 1);
                                       search = search.Substring(0, search.Length - format.Length - 1);
                                       var tx = await client.Get<byte[]>("transactions/" + search + "?format=" + format);
                                       if (tx != null)
                                       {
                                           Content = ToString(tx, format);
                                           return;
                                       }

                                       var block = await client.Get<byte[]>("blocks/" + search + "?format=" + format);
                                       if (block != null)
                                       {
                                           Content = ToString(block, format);
                                       }
                                       return;
                                   }
                                   var result = await client.Get<string>("whatisit/" + search);
                                   Content = result;
                                   return;
                               }
                           }
                           catch
                           {
                           }
                           try
                           {
                               Transaction transaction = new Transaction(search);
                               Content = transaction.ToString();
                               return;
                           }
                           catch
                           {
                               Content = "Good question holmes!";
                           }
                       }).Notify(MessengerInstance);
                }

                return _Search;
            }
        }

        private string ToString(byte[] bytes, string format)
        {
            if (format == "raw")
                return Encoders.Hex.EncodeData(bytes);
            if (format == "json")
                return Encoding.UTF8.GetString(bytes);
            throw new NotSupportedException("unsupported format " + format);
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
