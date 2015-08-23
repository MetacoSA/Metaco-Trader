using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.Messages
{
    public class ExposePropertiesMessage
    {
        public ExposePropertiesMessage(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _Target = target;
        }
        private readonly object _Target;
        public object Target
        {
            get
            {
                return _Target;
            }
        }
    }
}
