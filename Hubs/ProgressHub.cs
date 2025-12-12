using Corno.Web.Models;

namespace Corno.Web.Hubs;

public class ProgressHub : BaseProgressHub
{
    /*public void SendProgress(int progress)
    {
        Clients.All.receiveProgress(progress);
    }*/

    public void SendProgress(ProgressModel progressModel)
    {
        Clients.All.receiveProgress(progressModel);
    }
}