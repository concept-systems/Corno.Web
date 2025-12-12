using Corno.Web.Services.Interfaces;
using Eneter.Messaging.MessagingSystems.MessagingSystemBase;

namespace Corno.Web.Services;

public class AndroidService : IAndroidService
{
    #region -- Constructors --

    public AndroidService(/*IEneterServerService eneterServerService*/)
    {
        //_eneterServerService = eneterServerService;
        //_operationService = null;
        //_settingService = Bootstrapper.Get<ISettingService>();
    }
    #endregion

    #region -- Data  Members --
    //private readonly IEneterServerService _eneterServerService;
    //private IBaseOperationService _operationService;
    //private readonly ISettingService _settingService;
    #endregion

    #region -- Prperties --

    #endregion

    #region -- Private Methods --
    
    #endregion

    

    #region -- Eneter Events --
    private void OnClientConnected(object sender, ResponseReceiverEventArgs e)
    {
        // Add the client id to the listbox.
        // Note: we can directly access the listbox because we set threading mode of
        //       InputChannelThreading to the main UI thread.
        //BeginInvoke(new Action(() =>
        //{
        //    txtStatus.AppendText("Client Connected :" + e.ResponseReceiverId + Environment.NewLine);
        //}));
    }

    private void OnClientDisconnected(object sender, ResponseReceiverEventArgs e)
    {
        //// Remove the client from the listbox.
        //// Note: we can directly access the listbox because we set threading mode of
        ////       InputChannelThreading to the main UI thread.
        //BeginInvoke(new Action(() =>
        //{
        //    txtStatus.AppendText("Client Disconnected :" + e.ResponseReceiverId + Environment.NewLine);
        //}));
    }

    #endregion
}