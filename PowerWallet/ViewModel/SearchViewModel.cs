using NBitcoin;
using NBitcoin.DataEncoders;
using PowerWallet.Messages;
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
            MessengerInstance.Register<SearchMessage>(this, m =>
            {
                if (SearchedTerm != m.Term)
                {
                    SearchedTerm = m.Term;
                    Search.Execute();
                }
            });
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
                                   var url = "";
                                   if (search.EndsWith("-json") || search.EndsWith("-raw"))
                                   {
                                       var format = search.Substring(search.LastIndexOf("-") + 1);
                                       search = search.Substring(0, search.Length - format.Length - 1);
                                       url = "transactions/" + search + "?format=" + format;
                                       var tx = await client.Get<byte[]>(url);
                                       t.Info(url);
                                       if (tx != null)
                                       {
                                           Content = ToString(tx, format);
                                           return;
                                       }

                                       url = "blocks/" + search + "?format=" + format;
                                       var block = await client.Get<byte[]>(url);
                                       t.Info(url);
                                       if (block != null)
                                       {
                                           Content = ToString(block, format);
                                       }
                                       return;
                                   }
                                   url = "whatisit/" + search;
                                   var result = await client.Get<string>(url);
                                   t.Info(url);
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
