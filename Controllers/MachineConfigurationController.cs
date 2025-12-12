using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Models;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Controllers;

public class MachineConfigurationController : SuperController
{
    #region -- Constructors --
    public MachineConfigurationController(IMachineConfigurationService machineConfigurationService, IMiscMasterService miscMasterService)
    {
        _miscMasterService = miscMasterService;
        _machineConfigurationService = machineConfigurationService;
           
    }
    #endregion

    #region -- Data Members --
    private readonly IMiscMasterService _miscMasterService;
    // private readonly IMasterService<MachineConfiguration> _configurationService;
    private readonly IMachineConfigurationService _machineConfigurationService;
    #endregion

    #region -- Actions --
    public async Task<ActionResult> MachineConfiguration()
    {
        try
        {
            return await Task.FromResult(View(new MachineConfiguration())).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }


       

    [HttpPost]
    [ValidateInput(false)]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> MachineConfiguration(MachineConfiguration configuration)
    {
          
        if (!ModelState.IsValid)
            return View(configuration);
        try
        {

            await _machineConfigurationService.AddAsync(configuration);
            await _machineConfigurationService.SaveAsync();
            TempData["Success"] = " Added Successfully.";

            return RedirectToAction("MachineConfiguration");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(configuration);
    }

        

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _miscMasterService.Dispose(true);
        base.Dispose(disposing);
    }
    #endregion
}