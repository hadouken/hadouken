using System;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal interface IAlertBus
    {
        void StartRead();

        void StopRead();

        void Subscribe<TAlert>(Action<TAlert> callback) where TAlert : Alert;
    }
}
