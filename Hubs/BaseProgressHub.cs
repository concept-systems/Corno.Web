using Microsoft.AspNet.SignalR;

namespace Corno.Web.Hubs;

public class BaseProgressHub : Hub
{
    /*public async Task SendProgress(int progress)
    {
        await Clients.All.SendAsync("ReceiveProgress", progress);
    }*/

    public virtual void SendProgress(string message, int progress)
    {
        Clients.All.receiveProgress(progress);
    }
}