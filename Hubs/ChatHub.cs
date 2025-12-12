using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Corno.Web.Hubs;

public class ChatHub : Hub
{
    public ChatHub()
    {

    }
        
    private static Dictionary<string, string> _userNames = new Dictionary<string, string>();

    public void Send(string name, string message)
    {
        // Call the addNewMessageToPage method to update clients.
        Clients.All.addNewMessageToPage(name, message);
    }

    public void Register(string userName)
    {
        if (!_userNames.ContainsKey(userName))
        {
            _userNames.Add(userName, Context.ConnectionId);
        }
        else
        {
            _userNames[userName] = Context.ConnectionId;
        }

        Clients.All.usersLoggedIn(userName);
    }

    public override Task OnDisconnected(bool stopCalled)
    {
        var userName = string.Empty;

        foreach (var key in _userNames.Keys)
        {
            if (_userNames[key] == Context.ConnectionId)
            {
                userName = key;
            }
        }

        _userNames.Remove(userName);
        Clients.All.usersLoggedOut(userName);

        return base.OnDisconnected(stopCalled);
    }
}