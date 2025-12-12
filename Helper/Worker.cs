using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Web;
using Corno.Web.Extensions;
using Corno.Web.Models;
using EasyModbus;

namespace Corno.Web.Helper;

public class Worker
{
    #region -- Worker --

    public Worker()
    {
        _machineStartStopRegister = 1001;
        _dataStartRegister = 1001;
        _dataCount = 2;
    }
    #endregion

    #region -- Data Members --
    private static ModbusClient _client;
    readonly string filePath = @"d:/test.txt";

    private readonly int _machineStartStopRegister;
    private readonly int _dataStartRegister;
    private readonly int _dataCount;
    #endregion

    private void ConnectToModbus()
    {
        var ipAddress = "127.0.0.1";
        //var ipAddress = "192.168.0.250";
        var port = 502;

        _client = (ModbusClient)HttpContext.Current.Application[@"ModbusClient"];
        if (null == _client)
        {
            _client = new ModbusClient();
            HttpContext.Current.Application[@"ModbusClient"] = _client;
        }

        if (!_client.Available(500))
            _client.Connect(ipAddress, port);
    }

    public bool StartMachine(bool bValue)
    {
        bool bResult = false;
        try
        {
            // Try connecting client. Use same connection if already connected.
            ConnectToModbus();

            if (_client.Connected)
            {
                _client.WriteSingleRegister(_machineStartStopRegister, bValue ? 1 : 0);
                bResult = true;
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            //throw;
        }

        return bResult;
    }

    public bool GetMachineStatus()
    {
        bool bStarted = false;
        try
        {
            // Try connecting client. Use same connection if already connected.
            ConnectToModbus();

            if (_client.Connected)
            {
                var data = _client.ReadHoldingRegisters(_machineStartStopRegister, 1);
                bStarted = data[0].ToBoolean();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            //throw;
        }

        return bStarted;
    }

    public List<SampleData> ReadData()
    {
        var dataList = new List<SampleData>();

        // Try connecting client. Use same connection if already connected.
        ConnectToModbus();

        try
        {
            var data = _client.ReadHoldingRegisters(_dataStartRegister, _dataCount);
            int counter = 1;
            foreach (var entry in data)
            {
                dataList.Add(new SampleData
                {
                    Parameter = counter++,
                    Value = entry
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
        //_client.Disconnect();
        return dataList;
    }

    public DimensionScale ReadDimensionsModbus()
    {
        var isInProcess = HttpContext.Current.Application[@"InProcess"].ToBoolean();
        if (isInProcess) return null;

        HttpContext.Current.Application[@"InProcess"] = true;
        var dimensionScale = new DimensionScale();

        // Try connecting client. Use same connection if already connected.
        ConnectToModbus();

        try
        {
            var data = _client.ReadHoldingRegisters(_dataStartRegister, _dataCount);
            dimensionScale.ActualLength = data[0].ToDouble();
            dimensionScale.ActualWidth = data[1].ToDouble();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }

        HttpContext.Current.Application[@"InProcess"] = false;
        //_client.Disconnect();
        return dimensionScale;
    }

    private bool IsIpPing(string ip)
    {
        var ping = new Ping();
        //change the following ip variable into the ip adress you are looking for
        var address = IPAddress.Parse(ip);
        var pong = ping.Send(address);
        return pong?.Status == IPStatus.Success;
    }

    public DimensionScale ReadDimensionsHttp()
    {
        return new DimensionScale {ActualLength = 200, ActualWidth = 200};

        //var dimensionScale = new DimensionScale();
        //try
        //{
        //    const string ip = "192.168.2.2";
        //    const double maxLength = 736;
        //    const double maxWidth = 790;
        //    if (!IsIpPing(ip))
        //        throw new Exception("IP Address not connected.");

        //    var isInProcess = HttpContext.Current.Application[@"InProcess"].ToBooleans();
        //    if (isInProcess)
        //        throw new Exception("Wait");
        //    HttpContext.Current.Application[@"InProcess"] = true;

        //    var baseUrl = "http://" + ip + @"/iolinkmaster/port[@portNo]/iolinkdevice/pdin/getdata";
        //    for (int portNo = 0; portNo < 2; portNo++)
        //    {
        //        var url = baseUrl.Replace("@portNo", (portNo + 1).ToString());
        //        using (var webClient = new System.Net.WebClient())
        //        {
        //            var jsonString = webClient.DownloadString(url);
        //            switch (portNo)
        //            {
        //                case 0:
        //                    dimensionScale.ActualLength =  maxLength - GetDataFromJson(jsonString);
        //                    break;
        //                case 1:
        //                    dimensionScale.ActualWidth = maxWidth - GetDataFromJson(jsonString);
        //                    break;
        //            }
        //        }
        //    }
        //}
        //catch (Exception exception)
        //{
        //    dimensionScale.ActualLength = -1;
        //    dimensionScale.ActualWidth = -1;
        //}

        //HttpContext.Current.Application[@"InProcess"] = false;
        //return dimensionScale;
    }

    //public double GetDataFromJson(string jsonString)
    //{
    //    // Now parse with JSON.Net
    //    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);
    //    if (null == jsonObject)
    //        return -1;

    //    var value = jsonObject.data.value.ToString().Replace("{", "");
    //    value = value.ToString().Replace("}", "");
    //    return Convert.ToInt32(value, 16).ToDouble();
    //}

    public void StartProcessing(CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (File.Create(filePath)) { }
            using (var file = new StreamWriter(filePath))
            {
                for (var index = 1; index <= 20; index++)
                {
                    //execute when task has been cancel  
                    cancellationToken.ThrowIfCancellationRequested();
                    file.WriteLine("Its Line number : " + index + "\n");
                    Thread.Sleep(1500);   // wait to 1.5 sec every time  
                }

                if (Directory.Exists(@"d:/done"))
                    Directory.Delete(@"d:/done");
                Directory.CreateDirectory(@"d:/done");
            }
        }
        catch (Exception ex)
        {
            ProcessCancellation();
            File.AppendAllText(filePath, @"Error Occurred : " + ex.GetType() + " : " + ex.Message);
        }
    }

    private void ProcessCancellation()
    {
        Thread.Sleep(10000);
        File.AppendAllText(filePath, @"Process Cancelled");
    }
}