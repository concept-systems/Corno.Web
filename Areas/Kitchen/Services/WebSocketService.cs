using Corno.Web.Areas.Kitchen.Services.Interfaces;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Corno.Web.Logger;

namespace Corno.Web.Areas.Kitchen.Services;

public class WebSocketService : IWebSocketService
{
    #region -- Constructors --
    public WebSocketService()
    {
        //var ip = "172.26.58.88";
        //var port = 49666;
        var ip = "CONCEPT-VM002";
        var port = 8080;
        _url = $"ws://{ip}:{port}";
    }

    #endregion

    #region -- Data Members --

    private readonly string _url;
    #endregion

    #region -- Public Methods --
    public async Task SendRequest(ClientWebSocket ws, string request)
    {
        var sendBuffer = Encoding.UTF8.GetBytes(request);
        await ws.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
        LogHandler.LogInfo($"Request sent : {request}");
    }

    public async Task<string> ReceiveAcknowledgment(ClientWebSocket ws)
    {
        try
        {
            var receiveBuffer = new byte[1024];
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None).ConfigureAwait(false);
            var acknowledgment = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
            LogHandler.LogInfo($"Acknowledgment received: {acknowledgment}");
            return acknowledgment;
        }
        catch(Exception exception)
        {
            LogHandler.LogError(exception);
            throw;
        }
    }


    public async Task<bool> LedOperation(int ledNo, int value)
    {
        /*using var ws = new ClientWebSocket();
        await ws.ConnectAsync(new Uri(_url), CancellationToken.None);

        var message = Encoding.UTF8.GetBytes($"{ledNo},{value}");
        await ws.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);*/

        var serverUri = new Uri(_url);
        using var ws = new ClientWebSocket();
        LogHandler.LogInfo($"Connecting...: {_url}");
        await ws.ConnectAsync(serverUri, CancellationToken.None).ConfigureAwait(false);
        LogHandler.LogInfo("Connected Successfully");
        var request = $"{ledNo},{value}";
        await SendRequest(ws, request).ConfigureAwait(false);

        var acknowledgment = await ReceiveAcknowledgment(ws).ConfigureAwait(false);


        return true;
    }

    #endregion
}