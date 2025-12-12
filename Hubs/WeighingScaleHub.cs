using System;
using Microsoft.AspNet.SignalR;

namespace Corno.Web.Hubs;

public class WeighingScaleHub : Hub
{
    readonly Random _random = new Random();

    public void SendWeight()
    {
        var value = _random.Next(1, 100000);
        Clients.Caller.receiveWeight(value);
    }
}