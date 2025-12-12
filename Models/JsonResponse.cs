using Corno.Web.Globals;

namespace Corno.Web.Models;

public class JsonResponse
{
    public MessageType MessageType { get; set; }
    public string Message { get; set; }

    #region -- Methods --

    public static JsonResponse GetResponse(MessageType messageType, string message)
    {
        return new JsonResponse
        {
            MessageType = messageType,
            Message = message
        };
    }
    #endregion
}