using System;
using System.Net.Http.Json;
using System.Web.Helpers;
using Corno.Concept.Portal.Services.Interfaces;
using Corno.Logger;
using Corno.Services.Windsor;

namespace Corno.Concept.Portal.Services;

public class BaseApiService : IBaseApiService
{
    #region -- Constructors --

    public BaseApiService()
    {
        _settingService = Bootstrapper.Get<ISettingService>();

        Initialize();
    }
    #endregion

    #region -- Data  Members --

    private readonly ISettingService _settingService;
    #endregion

    #region -- Prperties --

    private string _otpBaseUrl;
    private string _coreBaseUrl;
    //public string BaseUrl { get; set; }
    #endregion

    #region -- Private Methods --

    private void Initialize()
    {
        //LogHandler.LogInfo($"Before get server url");
        if (string.IsNullOrEmpty(_otpBaseUrl))
            _otpBaseUrl = _settingService.Get(SettingConstants.Api, "OtpApiUrl");
        //_otpBaseUrl = HttpContext.Current.Request.Url.ToString();
        if (string.IsNullOrEmpty(_coreBaseUrl))
            _coreBaseUrl = _settingService.Get(SettingConstants.Api, "CoreApiUrl");
        //LogHandler.LogInfo($"Base Url : {BaseUrl}");
    }

    #endregion

    #region -- Protected Methods --
    protected virtual string GetBaseUrl(ApiName apiName)
    {
        switch (apiName)
        {
            case ApiName.Otp:
                return _otpBaseUrl;
            case ApiName.Core:
                return _coreBaseUrl;
            default:
                throw new Exception("Invalid Api Name.");
        }
    }
    #endregion


    #region -- Public Methods --

    public object Get(string action, object value, ApiName apiName)
    {
        var baseUrl = GetBaseUrl(apiName);
        if (string.IsNullOrEmpty(baseUrl))
            throw new Exception($"Invalid base url for {apiName}.");

        using (var client = new System.Net.Http.HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);

            // Get Methods 
            var getUri = new Uri($"{baseUrl}/api/{apiName}/{action}/");
            if (null != value)
                getUri = new Uri($"{baseUrl}/{apiName}/{action}/{value}");
            var getTask = client.GetAsync(getUri);
            // Use Task.Run to avoid deadlock in synchronous method
            var result = Task.Run(async () => await getTask.ConfigureAwait(false))
                .GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                var response = Task.Run(async () => await result.Content.ReadAsStringAsync().ConfigureAwait(false))
                    .GetAwaiter().GetResult();
                return response;
            }

            throw new Exception($"Error : {result.ReasonPhrase}");
        }
    }

    public object Post(string action, object value, ApiName apiName)
    {
        var baseUrl = GetBaseUrl(apiName);
        if (string.IsNullOrEmpty(baseUrl))
            throw new Exception($"Invalid url for {apiName}.");

        using (var client = new System.Net.Http.HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);

            //HTTP POST
            var postUri = new Uri($"{baseUrl}/api/{apiName}/{action}/");
            LogHandler.LogInfo($"Connecting to server : {postUri}");
            var postTask = client.PostAsJsonAsync(postUri, value);
            // Use Task.Run to avoid deadlock in synchronous method
            var result = Task.Run(async () => await postTask.ConfigureAwait(false))
                .GetAwaiter().GetResult();
            var contentResult = Task.Run(async () => await result.Content.ReadAsStringAsync().ConfigureAwait(false))
                .GetAwaiter().GetResult();
            //LogHandler.LogInfo($"Result : {contentResult}");
            if (!result.IsSuccessStatusCode)
            {
                var errorResult = Json.Decode(contentResult);
                throw new Exception(errorResult?.ExceptionMessage?.ToString());
            }

            //var stream = result.Content.ReadAsStreamAsync();
            //using (JsonReader jsonReader = new JsonTextReader(new System.IO.StreamReader(stream)))
            //{
            //    var serializer = new JsonSerializer();
            //    return serializer.Deserialize<Newtonsoft.Json.Linq.JObject>(jsonReader);
            //}

            //var contentResult1 = result as OkNegotiatedContentResult<ExamLinkDetail>;
            //var examLinkDetail = JsonConvert.DeserializeObject<ExamLinkViewModel>(contentResult);

            //var returnObject = JsonConvert.DeserializeObject(contentResult);

            //return result.Content;
            return contentResult;
        }
    }
    #endregion
}