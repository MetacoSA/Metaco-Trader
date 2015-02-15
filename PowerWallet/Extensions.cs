using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Threading;

namespace PowerWallet
{
    public static class Extensions
    {
        public static IObservable<T> ObserveHere<T>(this IObservable<T> obs)
        {
            return obs.ObserveOn(SynchronizationContext.Current);
        }
    }
}
