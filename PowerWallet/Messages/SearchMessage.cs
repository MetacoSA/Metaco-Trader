using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.Messages
{
    public class SearchMessage
    {
        public SearchMessage(string term)
        {
            Term = term;
        }

        public string Term
        {
            get;
            set;
        }
    }
}
