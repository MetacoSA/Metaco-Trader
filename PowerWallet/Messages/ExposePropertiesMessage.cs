using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.Messages
{
    public class ExposePropertiesMessage
    {
        public ExposePropertiesMessage(IHasProperties target)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _Target = target;
        }
        private readonly IHasProperties _Target;
        public IHasProperties Target
        {
            get
            {
                return _Target;
            }
        }
    }

    public interface IHasProperties
    {
        object ForPropertyGrid();
    }
}
